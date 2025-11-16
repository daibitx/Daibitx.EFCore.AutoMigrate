using Daibitx.EFCore.AutoMigrate.Core;

namespace Daibitx.EFCore.AutoMigrate.Abstraction
{
    public interface IMigrationRunner
    {
        Task Execute(AutoMigrationOptions options);
    }
}