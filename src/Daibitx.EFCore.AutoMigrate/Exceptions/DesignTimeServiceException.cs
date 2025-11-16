using System.Runtime.Serialization;

namespace Daibitx.EFCore.AutoMigrate.Exceptions
{
    /// <summary>
    /// Exceptions that occur during service operations at design time
    /// </summary>
    [Serializable]
    public class DesignTimeServiceException : AutoMigrationException
    {
        /// <summary>
        /// Get design-time service type
        /// </summary>
        public string? ServiceType { get; }

        /// <summary>
        /// 获取操作名称
        /// </summary>
        public string? OperationName { get; }

        public DesignTimeServiceException()
        {
        }

        public DesignTimeServiceException(string message)
            : base(message)
        {
        }

        public DesignTimeServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DesignTimeServiceException(string message, string? serviceType = null, string? operationName = null, Type? dbContextType = null)
            : base(message, dbContextType, "DesignTimeService")
        {
            ServiceType = serviceType;
            OperationName = operationName;
        }

        public DesignTimeServiceException(string message, Exception innerException, string? serviceType = null, string? operationName = null, Type? dbContextType = null)
            : base(message, innerException, dbContextType, "DesignTimeService")
        {
            ServiceType = serviceType;
            OperationName = operationName;
        }

        protected DesignTimeServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ServiceType = info.GetString(nameof(ServiceType));
            OperationName = info.GetString(nameof(OperationName));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ServiceType), ServiceType);
            info.AddValue(nameof(OperationName), OperationName);
        }

        public override string ToString()
        {
            var baseString = base.ToString();
            if (!string.IsNullOrEmpty(ServiceType) || !string.IsNullOrEmpty(OperationName))
            {
                var serviceInfo = !string.IsNullOrEmpty(ServiceType) ? $"Service: {ServiceType}" : "";
                var operationInfo = !string.IsNullOrEmpty(OperationName) ? $"Operation: {OperationName}" : "";
                var separator = !string.IsNullOrEmpty(serviceInfo) && !string.IsNullOrEmpty(operationInfo) ? ", " : "";

                return $"{baseString} [DesignTimeService - {serviceInfo}{separator}{operationInfo}]";
            }

            return baseString;
        }
    }
}