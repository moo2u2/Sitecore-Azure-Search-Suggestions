using System;
using System.Collections.Generic;
using System.Text;

namespace Sitecore.HabitatHome.Foundation.Search.Exceptions
{
    // Straight from Sitecore.ContentSearch.Azure.Http.Exceptions.PostFailedForSomeDocumentsException
    public class PostFailedForSomeDocumentsException : AzureSearchServiceRESTCallException
    {
        public PostFailedForSomeDocumentsException(string indexName, string message) : base(indexName, message)
        {
        }

        public PostFailedForSomeDocumentsException(string indexName, string message, IEnumerable<MultiStatusResponseDocument> documents, Exception innerException)
            : base(indexName, message, innerException)
        {
            Documents = documents;
        }

        public IEnumerable<MultiStatusResponseDocument> Documents { get; }

        public string RawResponse { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Message);
            if (Documents == null)
                return stringBuilder.ToString();
            stringBuilder.AppendLine("Failed documents:");
            foreach (MultiStatusResponseDocument document in Documents)
                stringBuilder.AppendLine($"Key: {document.Key}, message {document.Message}");
            return stringBuilder.ToString();
        }

        public PostFailedForSomeDocumentsException()
        {
        }

        public PostFailedForSomeDocumentsException(string message) : base(message)
        {
        }

        public PostFailedForSomeDocumentsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}