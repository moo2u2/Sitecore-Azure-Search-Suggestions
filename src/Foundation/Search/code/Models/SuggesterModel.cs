using Sitecore.Data;

namespace Sitecore.HabitatHome.Foundation.Search.Models
{
    public class SuggesterModel
    {
        public string Term { get; set; }

        public ID ContextItemID { get; set; }
    }
}