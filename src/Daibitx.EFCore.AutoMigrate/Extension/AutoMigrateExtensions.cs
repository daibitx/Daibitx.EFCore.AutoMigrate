using Daibitx.EFCore.AutoMigrate.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Daibitx.EFCore.AutoMigrate.Extension
{
    /// <summary>
    /// Auto migration extension methods
    /// </summary>
    public static class AutoMigrateExtensions
    {
        #region DI Migration

        /// <summary>
        /// Perform automatic migration using dependency injection (synchronous)
        /// </summary>
        /// <typeparam name="TContext">DbContext type</typeparam>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="configureDesignTimeServices">Configure design-time services</param>
        /// <param name="configureOptions">Configure migration options</param>
        public static void AutoMigrate<TContext>(
            this IServiceProvider serviceProvider,
            Action<IServiceCollection> configureDesignTimeServices,
            Action<AutoMigrationOptions> configureOptions = null)
            where TContext : DbContext
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            var options = new AutoMigrationOptions();
            configureOptions?.Invoke(options);

            var builder = new MigrateBuilder<TContext>(dbContext)
                .WithOptions(options);

            if (configureDesignTimeServices != null)
            {
                builder.AddDesignTimeServices(configureDesignTimeServices);
            }

            builder.AutoMigrate();
        }

        /// <summary>
        /// Perform automatic migration using dependency injection (asynchronous)
        /// </summary>
        /// <typeparam name="TContext">DbContext type</typeparam>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="configureDesignTimeServices">Configure design-time services</param>
        /// <param name="configureOptions">Configure migration options</param>
        public static async void AutoMigrateAsync<TContext>(
            this IServiceProvider serviceProvider,
            Action<IServiceCollection> configureDesignTimeServices,
            Action<AutoMigrationOptions> configureOptions = null)
            where TContext : DbContext
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            var options = new AutoMigrationOptions();
            configureOptions?.Invoke(options);

            var builder = new MigrateBuilder<TContext>(dbContext)
                .WithOptions(options);

            if (configureDesignTimeServices != null)
            {
                builder.AddDesignTimeServices(configureDesignTimeServices);
            }

            builder.AutoMigrateAsync();
        }

        /// <summary>
        /// Build a migration runner using dependency injection
        /// </summary>
        /// <typeparam name="TContext">DbContext type</typeparam>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="configureDesignTimeServices">Configure design-time services</param>
        /// <param name="configureOptions">Configure migration options</param>
        /// <returns>Migration runner</returns>
        public static MigrationRunner<TContext> BuildMigrationRunner<TContext>(
            this IServiceProvider serviceProvider,
            Action<IServiceCollection> configureDesignTimeServices,
            Action<AutoMigrationOptions> configureOptions = null)
            where TContext : DbContext
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            var options = new AutoMigrationOptions();
            configureOptions?.Invoke(options);

            var builder = new MigrateBuilder<TContext>(dbContext)
                .WithOptions(options);

            if (configureDesignTimeServices != null)
            {
                builder.AddDesignTimeServices(configureDesignTimeServices);
            }

            return builder.Build();
        }

        #endregion

        #region Non-DI Migration

        /// <summary>
        /// Perform automatic migration directly on DbContext (synchronous)
        /// </summary>
        /// <typeparam name="TContext">DbContext type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="configureDesignTimeServices">Configure design-time services</param>
        /// <param name="configureOptions">Configure migration options</param>
        public static void AutoMigrate<TContext>(
            this TContext dbContext,
            Action<IServiceCollection> configureDesignTimeServices,
            Action<AutoMigrationOptions> configureOptions = null)
            where TContext : DbContext
        {
            var options = new AutoMigrationOptions();
            configureOptions?.Invoke(options);

            var builder = new MigrateBuilder<TContext>(dbContext)
                .WithOptions(options);

            if (configureDesignTimeServices != null)
            {
                builder.AddDesignTimeServices(configureDesignTimeServices);
            }

            builder.AutoMigrate();
        }

        /// <summary>
        /// Perform automatic migration directly on DbContext (asynchronous)
        /// </summary>
        /// <typeparam name="TContext">DbContext type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="configureDesignTimeServices">Configure design-time services</param>
        /// <param name="configureOptions">Configure migration options</param>
        public static async void AutoMigrateAsync<TContext>(
            this TContext dbContext,
            Action<IServiceCollection> configureDesignTimeServices,
            Action<AutoMigrationOptions> configureOptions = null)
            where TContext : DbContext
        {
            var options = new AutoMigrationOptions();
            configureOptions?.Invoke(options);

            var builder = new MigrateBuilder<TContext>(dbContext)
                .WithOptions(options);

            if (configureDesignTimeServices != null)
            {
                builder.AddDesignTimeServices(configureDesignTimeServices);
            }

            builder.AutoMigrateAsync();
        }

        /// <summary>
        /// Build a migration runner directly from DbContext
        /// </summary>
        /// <typeparam name="TContext">DbContext type</typeparam>
        /// <param name="dbContext">Database context</param>
        /// <param name="configureDesignTimeServices">Configure design-time services</param>
        /// <param name="configureOptions">Configure migration options</param>
        /// <returns>Migration runner</returns>
        public static MigrationRunner<TContext> BuildMigrationRunner<TContext>(
            this TContext dbContext,
            Action<IServiceCollection> configureDesignTimeServices,
            Action<AutoMigrationOptions> configureOptions = null)
            where TContext : DbContext
        {
            var options = new AutoMigrationOptions();
            configureOptions?.Invoke(options);

            var builder = new MigrateBuilder<TContext>(dbContext)
                .WithOptions(options);

            if (configureDesignTimeServices != null)
            {
                builder.AddDesignTimeServices(configureDesignTimeServices);
            }

            return builder.Build();
        }

        #endregion

        #region Convenience Configuration Methods

        /// <summary>
        /// Configure as Safe Mode (disallow all destructive operations)
        /// </summary>
        /// <param name="options">Migration options</param>
        /// <returns>Configured options</returns>
        public static AutoMigrationOptions AsSafeMode(this AutoMigrationOptions options)
        {
            options.AllowDropTable = false;
            options.AllowDropColumn = false;
            options.AllowDropIndex = false;
            options.AllowDropForeignKey = false;
            options.AllowDropPrimaryKey = false;
            options.AllowAlterColumn = false;
            options.AllowRenameTable = false;
            options.AllowRenameColumn = false;
            options.AllowCreateIndex = true;
            options.AllowCreateForeignKey = true;
            return options;
        }

        /// <summary>
        /// Configure as Full Mode (allow all operations)
        /// </summary>
        /// <param name="options">Migration options</param>
        /// <returns>Configured options</returns>
        public static AutoMigrationOptions AsFullMode(this AutoMigrationOptions options)
        {
            options.AllowDropTable = true;
            options.AllowDropColumn = true;
            options.AllowDropIndex = true;
            options.AllowDropForeignKey = true;
            options.AllowDropPrimaryKey = true;
            options.AllowAlterColumn = true;
            options.AllowRenameTable = true;
            options.AllowRenameColumn = true;
            options.AllowCreateIndex = true;
            options.AllowCreateForeignKey = true;
            return options;
        }

        /// <summary>
        /// Configure command timeout
        /// </summary>
        /// <param name="options">Migration options</param>
        /// <param name="timeoutInSeconds">Timeout (seconds)</param>
        /// <returns>Configured options</returns>
        public static AutoMigrationOptions WithCommandTimeout(this AutoMigrationOptions options, int timeoutInSeconds)
        {
            options.CommandTimeout = timeoutInSeconds;
            return options;
        }

        #endregion
    }
}
