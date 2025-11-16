using System;
using System.Runtime.Serialization;

namespace Daibitx.EFCore.AutoMigrate.Exceptions
{
    /// <summary>
    /// Underlying exception occurred during automatic migration
    /// </summary>
    [Serializable]
    public class AutoMigrationException : Exception
    {
        /// <summary>
        /// Get the DbContext type that caused the exception
        /// </summary>
        public Type? DbContextType { get; }

        /// <summary>
        /// Get migration operation type
        /// </summary>
        public string? MigrationOperation { get; }

        public AutoMigrationException()
        {
        }

        public AutoMigrationException(string message)
            : base(message)
        {
        }

        public AutoMigrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AutoMigrationException(string message, Type? dbContextType = null, string? migrationOperation = null)
            : base(message)
        {
            DbContextType = dbContextType;
            MigrationOperation = migrationOperation;
        }

        public AutoMigrationException(string message, Exception innerException, Type? dbContextType = null, string? migrationOperation = null)
            : base(message, innerException)
        {
            DbContextType = dbContextType;
            MigrationOperation = migrationOperation;
        }

        protected AutoMigrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DbContextType = (Type?)info.GetValue(nameof(DbContextType), typeof(Type));
            MigrationOperation = info.GetString(nameof(MigrationOperation));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(DbContextType), DbContextType);
            info.AddValue(nameof(MigrationOperation), MigrationOperation);
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            if (DbContextType != null || !string.IsNullOrEmpty(MigrationOperation))
            {
                var contextInfo = DbContextType != null ? $"DbContext: {DbContextType.Name}" : "";
                var operationInfo = !string.IsNullOrEmpty(MigrationOperation) ? $"Operation: {MigrationOperation}" : "";
                var separator = !string.IsNullOrEmpty(contextInfo) && !string.IsNullOrEmpty(operationInfo) ? ", " : "";

                return $"{baseString} ({contextInfo}{separator}{operationInfo})";
            }

            return baseString;
        }
    }
}