namespace FishingSpot.PWA.Exceptions
{
    /// <summary>
    /// Exception levée lors d'erreurs de service (réseau, API, etc.)
    /// </summary>
    public class ServiceException : Exception
    {
        public string ServiceName { get; }
        public int? StatusCode { get; }

        public ServiceException(string message, string serviceName = "Unknown", Exception? innerException = null)
            : base(message, innerException)
        {
            ServiceName = serviceName;
        }

        public ServiceException(string message, int statusCode, string serviceName = "Unknown", Exception? innerException = null)
            : base(message, innerException)
        {
            ServiceName = serviceName;
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Exception levée lors d'erreurs de validation des données
    /// </summary>
    public class DataValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public DataValidationException(string message, Dictionary<string, string[]>? errors = null)
            : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }

    /// <summary>
    /// Exception levée lors d'erreurs de parsing/désérialisation
    /// </summary>
    public class DataException : Exception
    {
        public DataException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception levée lors d'erreurs d'authentification
    /// </summary>
    public class AuthenticationException : Exception
    {
        public bool IsTokenExpired { get; }

        public AuthenticationException(string message, bool isTokenExpired = false, Exception? innerException = null)
            : base(message, innerException)
        {
            IsTokenExpired = isTokenExpired;
        }
    }

    /// <summary>
    /// Exception levée lors d'erreurs de synchronisation offline
    /// </summary>
    public class SyncException : Exception
    {
        public string ItemId { get; }
        public string Operation { get; }

        public SyncException(string message, string itemId, string operation, Exception? innerException = null)
            : base(message, innerException)
        {
            ItemId = itemId;
            Operation = operation;
        }
    }
}
