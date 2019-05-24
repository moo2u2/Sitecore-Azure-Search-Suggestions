using Microsoft.Azure.Search.Models;

namespace Sitecore.HabitatHome.Foundation.Search
{
    public interface ISearchServiceDocumentOperationsProvider : ContentSearch.Azure.Http.ISearchServiceDocumentOperationsProvider
    {
        DocumentSuggestResult<Document> Suggest(AzureSuggestQuery q, string something, SuggestParameters options);

        AutocompleteResult Autocomplete(AzureSuggestQuery q, string something, AutocompleteParameters options);
    }
}
