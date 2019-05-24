using System.Net;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // Straight from Sitecore.ContentSearch.Azure.Http.MultiStatusResponseDocument
    public class MultiStatusResponseDocument
    {
        public string Key { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public bool Status { get; set; }
    }
}