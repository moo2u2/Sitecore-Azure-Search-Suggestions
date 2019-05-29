using Sitecore.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Mvc;
using Sitecore.HabitatHome.Feature.Search.Models;
using Sitecore.HabitatHome.Foundation.Search.Services;
using Sitecore.HabitatHome.Foundation.Search.Models;

namespace Sitecore.HabitatHome.Feature.Search
{
    // See Sitecore.XA.Feature.Search.Controllers.SearchController
    public class SearchController : Controller
    {
        [ActionName("Suggestions")]
        public ActionResult GetSuggestions(Guid itemID, string query)
        {
            return Json(SearchSuggestions(new SuggesterModel()
            {
                ContextItemID = Data.ID.Parse(itemID),
                Term = query
            }), JsonRequestBehavior.AllowGet);
        }

        [ActionName("Autocomplete")]
        public ActionResult GetAutocomplete(Guid itemID, string query)
        {
            return Json(Autocomplete(new SuggesterModel()
            {
                ContextItemID = Data.ID.Parse(itemID),
                Term = query
            }), JsonRequestBehavior.AllowGet);
        }

        protected virtual SuggestionsSet SearchSuggestions(SuggesterModel model)
        {
            Foundation.Search.Timer timer;
            SuggestionsSet suggestionsSet;
            Foundation.Search.Timer queryTimer;
            string indexName;
            using (timer = new Foundation.Search.Timer())
            {
                suggestionsSet = new SuggestionsSet();
                foreach (Suggestion suggestion in ServiceLocator.ServiceProvider.GetService<ISuggester>().GetSuggestions(model, out queryTimer, out indexName))
                {
                    suggestionsSet.Results.Add(suggestion);
                }
            }
            suggestionsSet.TotalTime = timer.Msec;
            suggestionsSet.QueryTime = queryTimer.Msec;
            suggestionsSet.Index = indexName;
            return suggestionsSet;
        }

        protected virtual SuggestionsSet Autocomplete(SuggesterModel model)
        {
            Foundation.Search.Timer timer;
            SuggestionsSet suggestionsSet;
            Foundation.Search.Timer queryTimer;
            string indexName;
            using (timer = new Foundation.Search.Timer())
            {
                suggestionsSet = new SuggestionsSet();
                foreach (Suggestion suggestion in ServiceLocator.ServiceProvider.GetService<ISuggester>().GetAutocomplete(model, out queryTimer, out indexName))
                {
                    suggestionsSet.Results.Add(suggestion);
                }
            }
            suggestionsSet.TotalTime = timer.Msec;
            suggestionsSet.QueryTime = queryTimer.Msec;
            suggestionsSet.Index = indexName;
            return suggestionsSet;
        }
    }
}