using Daibitx.EFCore.AutoMigrate.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Daibitx.EFCore.AutoMigrate.Abstraction;

namespace Daibitx.EFCore.AutoMigrate.Core
{
    public class DesignTimeService<TContext> : IDesignTimeService where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly Action<IServiceCollection> _configure;
        private IServiceProvider _provider;
        private IDesignTimeModel _designTimeModel;

        public DesignTimeService(TContext dbContext, Action<IServiceCollection> configure)
        {
            _dbContext = dbContext;
            _configure = configure;
            Build();
        }

        private void Build()
        {
            try
            {
                var infraProvider = ((IInfrastructure<IServiceProvider>)_dbContext).Instance;
                _designTimeModel = infraProvider.GetRequiredService<IDesignTimeModel>();

                var services = new ServiceCollection();

                services.AddEntityFrameworkDesignTimeServices();

                _configure?.Invoke(services);

                services.AddSingleton(_dbContext.Model);
                services.AddSingleton(_designTimeModel);

                _provider = services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                throw new DesignTimeServiceException("Failed to initialize design-time services", ex);
            }
        }
        public IMigrationsScaffolder MigrationsScaffolder
       => _provider.GetRequiredService<IMigrationsScaffolder>();

        public IDatabaseModelFactory DatabaseModelFactory
            => _provider.GetRequiredService<IDatabaseModelFactory>();

        public IScaffoldingModelFactory ScaffoldingModelFactory
            => _provider.GetRequiredService<IScaffoldingModelFactory>();

        public IMigrationsModelDiffer ModelDiffer
            => _dbContext.GetInfrastructure().GetRequiredService<IMigrationsModelDiffer>();

        public IMigrationsSqlGenerator MigrationsSqlGenerator
            => _dbContext.GetInfrastructure().GetRequiredService<IMigrationsSqlGenerator>();

        public IModel Model => _designTimeModel.Model;

        public IRelationalModel CodeModel => _designTimeModel.Model.GetRelationalModel();
    }
}
