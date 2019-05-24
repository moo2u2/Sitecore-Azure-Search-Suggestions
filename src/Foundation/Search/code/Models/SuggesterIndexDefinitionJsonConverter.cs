using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.ContentSearch.Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class SuggesterIndexDefinitionJsonConverter : IndexDefinitionJsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            SuggesterIndexDefinition indexDefinition = value as SuggesterIndexDefinition;
            if (indexDefinition == null)
                return;
            JObject jobject = JObject.FromObject(indexDefinition.AnalyzerDefinitions, serializer) ?? new JObject();
            IEnumerable<JToken> jtokens1 = indexDefinition.Fields.Select(f => JToken.FromObject(f, serializer));
            IEnumerable<JToken> jtokens2 = indexDefinition.ScoringProfiles.Select(p => JToken.FromObject(p, serializer));
            IEnumerable<JToken> jtokens3 = indexDefinition.Suggesters.Select(p => JToken.FromObject(p, serializer));
            jobject.AddFirst(new JProperty("fields", new JArray(jtokens1)));
            jobject.Add(new JProperty("scoringProfiles", new JArray(jtokens2)));
            jobject.Add(new JProperty("suggesters", new JArray(jtokens3)));
            jobject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                return null;
            JObject jobject = JObject.Load(reader);
            if (jobject == null)
                return null;
            return new SuggesterIndexDefinition(jobject.ToObject<AnalyzerDefinitions>(serializer), 
                jobject["fields"]?.Children().Select(f => f.ToObject<IndexedField>(serializer)).ToList(),
                DeserializeScoringProfiles(jobject["scoringProfiles"], serializer), 
                jobject["suggesters"]?.Children().Select(f => f.ToObject<Suggester>(serializer)).ToList());
        }
    }
}