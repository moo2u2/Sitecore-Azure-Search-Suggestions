using Sitecore.ContentSearch.Azure.Exceptions;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Http.Exceptions.SearchServiceIsUnavailableException
    public class SearchServiceIsUnavailableException : CloudSearchIndexException
    {
        public SearchServiceIsUnavailableException(string indexName, string message, Exception innerException) : base(indexName, message, innerException)
        {
        }

        public SearchServiceIsUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SearchServiceIsUnavailableException()
        {
        }

        public SearchServiceIsUnavailableException(string message) : base(message)
        {
        }
    }
}