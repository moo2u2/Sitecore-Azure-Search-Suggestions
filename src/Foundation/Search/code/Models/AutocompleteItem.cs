using Newtonsoft.Json;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class AutocompleteItem
    {
        public AutocompleteItem()
        {
        }

        public AutocompleteItem(string text = null, string queryPlusText = null)
        {
            Text = text;
            QueryPlusText = queryPlusText;
        }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; private set; }

        [JsonProperty(PropertyName = "queryPlusText")]
        public string QueryPlusText { get; private set; }
    }
}