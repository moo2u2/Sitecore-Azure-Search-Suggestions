using Sitecore.HabitatHome.Foundation.Search.Models;

namespace Sitecore.HabitatHome.Foundation.Search
{
    public interface ISearchServiceDocumentOperationsProvider : ContentSearch.Azure.Http.ISearchServiceDocumentOperationsProvider
    {
        DocumentSuggestResult<Document> Suggest(SuggestionRequest options);

        AutocompleteResult Autocomplete(AutocompleteRequest options);
    }
}
