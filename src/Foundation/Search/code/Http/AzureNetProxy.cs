using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Foundation.Search.Models;
using Sitecore.HabitatHome.Foundation.Search.Services;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // See Sitecore.ContentSearch.SolrProvider.SolrNetIntegration.SolrNetProxy
    public static class AzureNetProxy
    {
        public static DocumentSuggestResult<Document> Suggest(this IProviderSearchContext context, SuggestionRequest suggestionRequest)
        {
            ICloudSearchIndex index = context.Index as CloudSearchProviderSuggestionIndex;
            Assert.IsNotNull(index, "index");
            return ((CompositeSearchService)index.SearchService).Suggest(suggestionRequest);
        }
        public static AutocompleteResult AutoComplete(this IProviderSearchContext context, AutocompleteRequest autocompleteRequest)
        {
            ICloudSearchIndex index = context.Index as CloudSearchProviderSuggestionIndex;
            Assert.IsNotNull(index, "index");
            return ((CompositeSearchService)index.SearchService).Autocomplete(autocompleteRequest);
        }
    }
}