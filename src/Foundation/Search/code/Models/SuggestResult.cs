using Newtonsoft.Json;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class SuggestResult<T> where T : class
    {
        public SuggestResult()
        {
        }

        public SuggestResult(T document = null, string text = null)
        {
            Document = document;
            Text = text;
        }

        public T Document { get; private set; }

        [JsonProperty(PropertyName = "@search.text")]
        public string Text { get; private set; }
    }
}