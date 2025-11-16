using Daibitx.EFCore.AutoMigrate.Core;
using Xunit;

namespace Daibitx.EFCore.AutoMigrate.Tests
{
    /// <summary>
    /// this library has been used in my project many many times, and it works well.
    /// so I don't wang to write the fucking unit test, but I will write some test cases to ensure the correctness of the code.
    /// </summary>
    public class AutoMigrationOptionsTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithSafeDefaults()
        {
            // Arrange & Act
            var options = new AutoMigrationOptions();

            // Assert
            Assert.False(options.AllowDropTable);
            Assert.False(options.AllowDropColumn);
            Assert.False(options.AllowDropIndex);
            Assert.False(options.AllowDropForeignKey);
            Assert.False(options.AllowDropPrimaryKey);
            Assert.False(options.AllowAlterColumn);
            Assert.False(options.AllowRenameTable);
            Assert.False(options.AllowRenameColumn);
            Assert.True(options.AllowCreateIndex);
            Assert.True(options.AllowCreateForeignKey);
            Assert.True(options.UseTransactions);
            Assert.Equal(30, options.CommandTimeout);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AllowDropTable_ShouldSetCorrectValue(bool value)
        {
            // Arrange
            var options = new AutoMigrationOptions();

            // Act
            options.AllowDropTable = value;

            // Assert
            Assert.Equal(value, options.AllowDropTable);
        }

        [Fact]
        public void AsSafeMode_ShouldBlockAllDestructiveOperations()
        {
            // Arrange
            var options = new AutoMigrationOptions();

            // Act
            options.AsSafeMode();

            // Assert
            Assert.False(options.AllowDropTable);
            Assert.False(options.AllowDropColumn);
            Assert.False(options.AllowDropIndex);
            Assert.False(options.AllowDropForeignKey);
            Assert.False(options.AllowDropPrimaryKey);
            Assert.False(options.AllowAlterColumn);
            Assert.False(options.AllowRenameTable);
            Assert.False(options.AllowRenameColumn);
            Assert.True(options.AllowCreateIndex);
            Assert.True(options.AllowCreateForeignKey);
        }

        [Fact]
        public void AsFullMode_ShouldAllowAllOperations()
        {
            // Arrange
            var options = new AutoMigrationOptions();

            // Act
            options.AsFullMode();

            // Assert
            Assert.True(options.AllowDropTable);
            Assert.True(options.AllowDropColumn);
            Assert.True(options.AllowDropIndex);
            Assert.True(options.AllowDropForeignKey);
            Assert.True(options.AllowDropPrimaryKey);
            Assert.True(options.AllowAlterColumn);
            Assert.True(options.AllowRenameTable);
            Assert.True(options.AllowRenameColumn);
            Assert.True(options.AllowCreateIndex);
            Assert.True(options.AllowCreateForeignKey);
        }
    }
}