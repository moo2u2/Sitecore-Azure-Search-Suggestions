using System.Text;

namespace Sitecore.HabitatHome.Foundation.Search.Http
{
    public static class SearchUrl
    {
        public static string GetSuggestions(string indexName, string apiVersion, string suggesterName, string search, 
            bool fuzzy = false, int top = 5, int minimumCoverage = 80, string highlightPreTag = null, string highlightPostTag = null, 
            string searchFields = null, string filter = null, string orderBy = null, string select = null)
        {
            StringBuilder sb = new StringBuilder($"indexes/{indexName}/docs/suggest?api-version={apiVersion}&suggesterName={suggesterName}&search={search}&fuzzy={fuzzy}&top={top}&minimumCoverage={minimumCoverage}");
            if (!string.IsNullOrEmpty(highlightPreTag))
            {
                sb.Append('&');
                sb.Append(highlightPreTag);
            }
            if (!string.IsNullOrEmpty(highlightPostTag))
            {
                sb.Append('&');
                sb.Append(highlightPostTag);
            }
            if (!string.IsNullOrEmpty(searchFields))
            {
                sb.Append('&');
                sb.Append(searchFields);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                sb.Append('&');
                sb.Append(filter);
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                sb.Append('&');
                sb.Append(orderBy);
            }
            if (!string.IsNullOrEmpty(select))
            {
                sb.Append('&');
                sb.Append(select);
            }
            return sb.ToString();
        }

        public static string PostSuggestions(string indexName, string apiVersion)
        {
            return $"indexes/{indexName}/docs/suggest?api-version={apiVersion}";
        }

        public static string GetAutocomplete(string indexName, string apiVersion, string suggesterName, string search, string autocompleteMode = "oneTerm", 
            bool fuzzy = false, int top = 5, int minimumCoverage = 80, string highlightPreTag = null, string highlightPostTag = null,
            string searchFields = null, string filter = null)
        {
            StringBuilder sb = new StringBuilder($"indexes/{indexName}/docs/suggest?api-version={apiVersion}&suggesterName={suggesterName}&search={search}&autocompleteMode={autocompleteMode}&fuzzy={fuzzy}&top={top}&minimumCoverage={minimumCoverage}");
            if (!string.IsNullOrEmpty(highlightPreTag))
            {
                sb.Append('&');
                sb.Append(highlightPreTag);
            }
            if (!string.IsNullOrEmpty(highlightPostTag))
            {
                sb.Append('&');
                sb.Append(highlightPostTag);
            }
            if (!string.IsNullOrEmpty(searchFields))
            {
                sb.Append('&');
                sb.Append(searchFields);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                sb.Append('&');
                sb.Append(filter);
            }
            return sb.ToString();
        }

        public static string PostAutocomplete(string indexName, string apiVersion)
        {
            return $"indexes/{indexName}/docs/autocomplete?api-version={apiVersion}";
        }
    }
}