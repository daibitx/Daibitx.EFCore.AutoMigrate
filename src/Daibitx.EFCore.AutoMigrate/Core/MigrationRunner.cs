using Daibitx.EFCore.AutoMigrate.Abstraction;
using Daibitx.EFCore.AutoMigrate.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace Daibitx.EFCore.AutoMigrate.Core
{
    public class MigrationRunner<TContext> : IMigrationRunner where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly DesignTimeService<TContext> _design;
        private readonly MigrationStepProcessor _processor;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public MigrationRunner(TContext dbContext, Action<IServiceCollection> designConfig)
        {
            _dbContext = dbContext;
            _design = new DesignTimeService<TContext>(dbContext, designConfig);
            _processor = new MigrationStepProcessor(dbContext);
        }

        public async Task Execute(AutoMigrationOptions options)
        {
            await semaphore.WaitAsync();
            try
            {
                var database = _dbContext.Database;
                var connection = database.GetDbConnection();
                if (!await database.CanConnectAsync())
                {
                    Console.WriteLine($"Database '{connection.Database}' does not exist. Creating...");
                    await database.EnsureCreatedAsync();
                    return;
                }
                IRelationalModel databaseModel = null;
                try
                {
                    var tableInfo = _dbContext.Model.GetEntityTypes()
                        .Where(e => !e.ClrType.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase))
                        .Select(e => (Schema: e.GetSchema(), Table: e.GetTableName()))
                        .Where(t => !string.IsNullOrWhiteSpace(t.Table))
                        .Distinct()
                        .ToList();

                    var tableNames = tableInfo
                        .Select(t => t.Table)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    var databaseModelFactory = _design.DatabaseModelFactory;
                    var databaseModelOption = new DatabaseModelFactoryOptions(tableNames);
                    var dbModel = databaseModelFactory.Create(connection, databaseModelOption);
                    var scaffoldingModelFactory = _design.ScaffoldingModelFactory;
                    databaseModel = scaffoldingModelFactory.Create(dbModel, new ModelReverseEngineerOptions() { UseDatabaseNames = true }).GetRelationalModel();
                }
                catch (Exception ex)
                {
                    throw new AutoMigrationException($"Failed to read database model, assuming empty database", ex);
                }
                var codeModel = _design.CodeModel;
                var modelDiffer = _design.ModelDiffer;
                var operations = modelDiffer.GetDifferences(databaseModel, codeModel);
                operations = _processor.FilterMigrationOperations(operations.ToList(), options);
                if (!operations.Any())
                {
                    return;
                }
                var commands = _design.MigrationsSqlGenerator.Generate(operations, _design.Model);
                using var transaction = await database.BeginTransactionAsync();
                try
                {
                    var command = _processor.JoinCommands(commands.ToList());
                    if (string.IsNullOrEmpty(command))
                    {
                        return;
                    }
                    await database.ExecuteSqlRawAsync(command);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new AutoMigrationException($"Migration transaction failed", ex);
                }
            }
            catch (Exception ex)
            {
                throw new AutoMigrationException($"Migration failed", ex);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
