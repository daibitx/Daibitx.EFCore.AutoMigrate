using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Daibitx.EFCore.AutoMigrate.Abstraction
{
    public interface IDesignTimeService
    {
        IRelationalModel CodeModel { get; }
        IDatabaseModelFactory DatabaseModelFactory { get; }
        IMigrationsScaffolder MigrationsScaffolder { get; }
        IMigrationsSqlGenerator MigrationsSqlGenerator { get; }
        IModel Model { get; }
        IMigrationsModelDiffer ModelDiffer { get; }
        IScaffoldingModelFactory ScaffoldingModelFactory { get; }
    }
}