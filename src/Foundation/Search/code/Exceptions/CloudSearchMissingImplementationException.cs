using Sitecore.ContentSearch.Azure.Exceptions;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Exception.CloudSearchMissingImplementationException
    public class CloudSearchMissingImplementationException : CloudSearchException
    {
        public CloudSearchMissingImplementationException(string message, string interfaceName) : base(message)
        {
            InterfaceName = interfaceName;
        }

        public string InterfaceName { get; }

        public override string ToString()
        {
            return $"Interface: {InterfaceName} {base.ToString()}";
        }

        public CloudSearchMissingImplementationException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        public CloudSearchMissingImplementationException(string message)
          : base(message)
        {
        }

        public CloudSearchMissingImplementationException()
        {
        }
    }
}