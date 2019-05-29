using Sitecore.HabitatHome.Foundation.Search.Models;
using System.Collections.Generic;

namespace Sitecore.HabitatHome.Feature.Search.Models
{
    public class SuggestionsSet
    {
        public long TotalTime { get; set; }

        public long QueryTime { get; set; }

        public string Signature { get; set; }

        public string Index { get; set; }

        public IList<Suggestion> Results { get; set; } = new List<Suggestion>();
    }
}