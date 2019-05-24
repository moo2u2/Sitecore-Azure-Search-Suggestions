using Sitecore.ContentSearch.Azure.Exceptions;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Http.Exceptions.AzureQuotaExceededException
    public class AzureQuotaExceededException : CloudSearchIndexException
    {
        public AzureQuotaExceededException(string indexName, string message, AzureSearchServiceRESTCallException innerException)
          : base(indexName, message, innerException)
        {
        }

        public AzureQuotaExceededException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AzureQuotaExceededException(string message) : base(message)
        {
        }

        public AzureQuotaExceededException()
        {
        }
    }
}