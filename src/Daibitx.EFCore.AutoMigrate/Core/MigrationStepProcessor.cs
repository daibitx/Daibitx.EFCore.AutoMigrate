using Daibitx.EFCore.AutoMigrate.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Daibitx.EFCore.AutoMigrate.Core
{
    public class MigrationStepProcessor : IMigrationStepProcessor
    {
        private readonly DbContext _dbContext;

        public MigrationStepProcessor(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string JoinCommands(List<MigrationCommand> operations)
        {
            var providerName = _dbContext.Database.ProviderName;
            var sqls = operations.Select(o => o.CommandText);

            return providerName switch
            {
                "Pomelo.EntityFrameworkCore.MySql" => string.Join("", sqls),
                _ => string.Join(";", sqls)
            };
        }

        public List<MigrationOperation> FilterMigrationOperations(
       List<MigrationOperation> operations,
       AutoMigrationOptions options)
        {
            return operations
                .Where(op => op switch
                {
                    // ========== ALLOW / BLOCK CREATE ==========
                    CreateIndexOperation => options.AllowCreateIndex,
                    AddForeignKeyOperation => options.AllowCreateForeignKey,

                    // ========== DROP ==========
                    DropTableOperation => options.AllowDropTable,
                    DropColumnOperation => options.AllowDropColumn,
                    DropIndexOperation => options.AllowDropIndex,
                    DropForeignKeyOperation => options.AllowDropForeignKey,
                    DropPrimaryKeyOperation => options.AllowDropPrimaryKey,

                    // ========== ALTER ==========
                    AlterColumnOperation => options.AllowAlterColumn,

                    // ========== RENAME ==========
                    RenameTableOperation => options.AllowRenameTable,
                    RenameColumnOperation => options.AllowRenameColumn,

                    // ========== Alaways block operations ==========

                    AddCheckConstraintOperation => false,
                    AddPrimaryKeyOperation => false,
                    AddUniqueConstraintOperation => false,
                    DatabaseOperation => false,
                    DeleteDataOperation => false,
                    DropCheckConstraintOperation => false,
                    DropSchemaOperation => false,
                    DropSequenceOperation => false,
                    DropUniqueConstraintOperation => false,
                    EnsureSchemaOperation => false,
                    InsertDataOperation => false,
                    AlterTableOperation => false,
                    RenameIndexOperation => false,
                    AlterSequenceOperation => false,
                    RenameSequenceOperation => false,
                    RestartSequenceOperation => false,
                    SqlOperation => false,
                    UpdateDataOperation => false,
                    _ => true
                })
                .ToList();
        }

    }
}
