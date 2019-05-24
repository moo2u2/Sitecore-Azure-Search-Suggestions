using Sitecore.ContentSearch.Azure.Exceptions;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Http.Exceptions.BadRequestException
    public class BadRequestException : CloudSearchIndexException
    {
        public BadRequestException(string indexName, string message, AzureSearchServiceRESTCallException innerException) : base(indexName, message, innerException)
        {
        }

        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException()
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}