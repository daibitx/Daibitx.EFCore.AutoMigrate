using Daibitx.EFCore.AutoMigrate.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Daibitx.EFCore.AutoMigrate.Core
{
    public class MigrateBuilder<TContext> : IMigrateBuilder<TContext> where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private Action<IServiceCollection> _designTimeServiceAction;
        private AutoMigrationOptions _options;
        public MigrateBuilder(TContext dbContext)
        {
            _dbContext = dbContext;
            _options = new AutoMigrationOptions();
        }

        public MigrateBuilder<TContext> AddDesignTimeServices(Action<IServiceCollection> action)
        {
            _designTimeServiceAction = action;
            return this;
        }

        public MigrateBuilder<TContext> WithOptions(AutoMigrationOptions options)
        {
            _options = options;
            return this;
        }

        public MigrationRunner<TContext> Build()
        {
            return new MigrationRunner<TContext>(_dbContext, _designTimeServiceAction);
        }

        public async void AutoMigrateAsync()
        {
            await Build().Execute(_options);
        }

        public void AutoMigrate()
        {
            Build().Execute(_options).Wait();
        }
    }
}
