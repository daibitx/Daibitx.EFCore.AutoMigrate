namespace Daibitx.EFCore.AutoMigrate.Core
{
    public class AutoMigrationOptions
    {

        public bool AllowDropTable { get; set; } = false;
        public bool AllowDropColumn { get; set; } = false;
        public bool AllowDropIndex { get; set; } = false;
        public bool AllowDropForeignKey { get; set; } = false;
        public bool AllowDropPrimaryKey { get; set; } = false;
        public bool AllowAlterColumn { get; set; } = false;
        public bool AllowRenameTable { get; set; } = false;
        public bool AllowRenameColumn { get; set; } = false;
        public bool AllowCreateIndex { get; set; } = true;
        public bool AllowCreateForeignKey { get; set; } = true;
        public bool UseTransactions { get; set; } = true;
        public int CommandTimeout { get; set; } = 30;

        public AutoMigrationOptions AsSafeMode()
        {
            AllowDropTable = false;
            AllowDropColumn = false;
            AllowDropIndex = false;
            AllowDropForeignKey = false;
            AllowDropPrimaryKey = false;
            AllowAlterColumn = false;
            AllowRenameTable = false;
            AllowRenameColumn = false;
            AllowCreateIndex = true;
            AllowCreateForeignKey = true;
            UseTransactions = true;
            CommandTimeout = 30;
            return this;
        }

        public AutoMigrationOptions AsFullMode()
        {
            AllowDropTable = true;
            AllowDropColumn = true;
            AllowDropIndex = true;
            AllowDropForeignKey = true;
            AllowDropPrimaryKey = true;
            AllowAlterColumn = true;
            AllowRenameTable = true;
            AllowRenameColumn = true;
            AllowCreateIndex = true;
            AllowCreateForeignKey = true;
            return this;
        }
    }
}