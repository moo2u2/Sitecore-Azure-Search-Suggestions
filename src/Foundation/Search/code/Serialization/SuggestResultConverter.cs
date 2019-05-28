using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.HabitatHome.Foundation.Search.Models;
using System;
using System.Reflection;

namespace Sitecore.HabitatHome.Foundation.Search.Serialization
{
    public class SuggestResultConverter<T> : JsonConverter where T : class
    {
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(SuggestResult<T>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jobject = serializer.Deserialize<JObject>(reader);
            JToken jtoken = jobject["@search.text"];
            JTokenReader jtokenReader = new JTokenReader(jobject);
            return new SuggestResult<T>(serializer.Deserialize<T>(jtokenReader), jtoken.Value<string>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}