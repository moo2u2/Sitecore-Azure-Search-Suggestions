using Sitecore.ContentSearch.Azure.Exceptions;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Http.Exceptions.AzureSearchServiceRESTCallException
    public class AzureSearchServiceRESTCallException : CloudSearchIndexException
    {
        public AzureSearchServiceRESTCallException(string indexName, string message) : this(indexName, message, null)
        {
        }

        public AzureSearchServiceRESTCallException( string indexName, string message, Exception innerException)
          : base(indexName, message, innerException)
        {
        }

        public AzureSearchServiceRESTCallException()
        {
        }

        public AzureSearchServiceRESTCallException(string message)
          : base(message)
        {
        }

        public AzureSearchServiceRESTCallException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}