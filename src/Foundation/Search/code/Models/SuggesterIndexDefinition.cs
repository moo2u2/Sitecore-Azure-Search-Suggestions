using Newtonsoft.Json;
using Sitecore.ContentSearch.Azure.Models;
using System.Collections.Generic;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    [JsonConverter(typeof(SuggesterIndexDefinitionJsonConverter))]
    public class SuggesterIndexDefinition : IndexDefinition
    {
        public IEnumerable<Suggester> Suggesters { get; set; }

        public SuggesterIndexDefinition(AnalyzerDefinitions analyzerDefinitions, IEnumerable<IndexedField> fields, IEnumerable<ScoringProfile> scoringProfiles, IEnumerable<Suggester> suggesters = null)
            : base(analyzerDefinitions, fields, scoringProfiles)
        {
            Suggesters = suggesters;
        }
    }
}