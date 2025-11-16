using Daibitx.EFCore.AutoMigrate.Core;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Daibitx.EFCore.AutoMigrate.Abstraction
{
    public interface IMigrationStepProcessor
    {
        List<MigrationOperation> FilterMigrationOperations(List<MigrationOperation> operations, AutoMigrationOptions options);
        string JoinCommands(List<MigrationCommand> operations);
    }
}