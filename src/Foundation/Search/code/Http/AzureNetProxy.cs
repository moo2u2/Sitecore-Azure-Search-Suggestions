using Microsoft.Azure.Search.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.Diagnostics;
using Sitecore.HabitatHome.Foundation.Search.Services;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // See Sitecore.ContentSearch.SolrProvider.SolrNetIntegration.SolrNetProxy
    public static class AzureNetProxy
    {
        public static DocumentSuggestResult<Document> Suggest(this IProviderSearchContext context, AzureSuggestQuery q, string suggeterName, SuggestParameters options)
        {
            ICloudSearchIndex index = context.Index as CloudSearchProviderSuggestionIndex;
            Assert.IsNotNull(index, "index");
            return ((CompositeSearchService)index.SearchService).Suggest(q, suggeterName, options);
        }
        public static AutocompleteResult AutoComplete(this IProviderSearchContext context, AzureSuggestQuery q, string suggeterName, AutocompleteParameters options)
        {
            ICloudSearchIndex index = context.Index as CloudSearchProviderSuggestionIndex;
            Assert.IsNotNull(index, "index");
            return ((CompositeSearchService)index.SearchService).Autocomplete(q, suggeterName, options);
        }
    }
}