using Sitecore.ContentSearch.Azure.Exceptions;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Http.Exceptions.NotFoundException
    public class NotFoundException : CloudSearchIndexException
    {
        public NotFoundException(string indexName, string message, Exception innerException) : base(indexName, message, innerException)
        {
        }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException()
        {
        }
    }
}