using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    // From Microsoft.Azure.Search.Models.DocumentSuggestResult
    public class DocumentSuggestResult<T> where T : class
    {
        public DocumentSuggestResult()
        {
        }

        public DocumentSuggestResult(IList<SuggestResult<T>> results = null, double? coverage = null)
        {
            Results = results;
            Coverage = coverage;
        }

        [JsonProperty(PropertyName = "value")]
        public IList<SuggestResult<T>> Results { get; private set; }

        [JsonProperty(PropertyName = "@search.coverage")]
        public double? Coverage { get; private set; }
    }
}