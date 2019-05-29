using Microsoft.Azure.Search.Models;

namespace Sitecore.HabitatHome.Foundation.Search
{
    public interface ISearchServiceDocumentOperationsProvider : ContentSearch.Azure.Http.ISearchServiceDocumentOperationsProvider
    {
        DocumentSuggestResult<Document> Suggest(AzureSuggestQuery q, string suggester, SuggestParameters options);

        AutocompleteResult Autocomplete(AzureSuggestQuery q, string suggester, AutocompleteParameters options);
    }
}
