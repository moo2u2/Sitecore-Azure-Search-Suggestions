namespace Sitecore.HabitatHome.Foundation.Search
{
    // See Sitecore.ContentSearch.SolrNetExtension.SolrSuggestQuery
    public class AzureSuggestQuery
    {
        public string Query { get; set; }

        public AzureSuggestQuery(string query)
        {
            Query = query;
        }

        public static implicit operator AzureSuggestQuery(string value)
        {
            return new AzureSuggestQuery(value);
        }
    }
}