using Daibitx.EFCore.AutoMigrate.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Daibitx.EFCore.AutoMigrate.Abstraction
{
    public interface IMigrateBuilder<TContext> where TContext : DbContext
    {
        MigrateBuilder<TContext> AddDesignTimeServices(Action<IServiceCollection> action);
        void AutoMigrate();
        void AutoMigrateAsync();
        MigrationRunner<TContext> Build();
        MigrateBuilder<TContext> WithOptions(AutoMigrationOptions options);
    }
}