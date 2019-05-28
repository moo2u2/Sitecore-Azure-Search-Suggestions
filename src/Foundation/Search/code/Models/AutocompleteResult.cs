using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class AutocompleteResult
    {
        public AutocompleteResult()
        {
        }

        public AutocompleteResult(IList<AutocompleteItem> results = null)
        {
            Results = results;
        }

        [JsonProperty(PropertyName = "value")]
        public IList<AutocompleteItem> Results { get; private set; }
    }
}