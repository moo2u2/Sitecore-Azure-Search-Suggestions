using Sitecore.XA.Feature.Search.Attributes;
using Sitecore.XA.Feature.Search.Binder;
using Sitecore.XA.Foundation.Search.Models;
using Sitecore.XA.Foundation.Search.Models.Binding;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Sitecore.HabitatHome.Feature.Search
{
    [JsonFormatter]
    public class SearchSuggestionsController : XA.Feature.Search.Controllers.SearchController
    {
        [ActionName("SuggestionsEx")]
        public SuggestionsSet GetSuggestionsEx([ModelBinder(BinderType = typeof(QueryModelBinder))] QueryModel model)
        {
            return SearchSuggestions(new SuggesterModel()
            {
                ContextItemID = model.ItemID,
                Term = model.Query
            });
        }

    }
}