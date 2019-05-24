using Sitecore.ContentSearch.Azure.Exceptions;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Exception.CloudSearchCompositeSearchServiceException
    public class CloudSearchCompositeSearchServiceException : CloudSearchException
    {
        public CloudSearchCompositeSearchServiceException(string message) : base(message)
        {
        }

        public CloudSearchCompositeSearchServiceException()
        {
        }

        public CloudSearchCompositeSearchServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}