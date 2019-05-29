using Sitecore.ContentSearch;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch.Security;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using Sitecore.HabitatHome.Foundation.Search.Models;
using Sitecore.Abstractions;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    // See Sitecore.XA.Foundation.VersionSpecific.Services.SolrSuggester
    public class AzureSearchSuggester : ISuggester
    {
        protected const int SuggestionsCount = 5;

        protected IIndexResolver IndexResolver { get; }

        protected BaseSiteContextFactory SiteContextFactory { get; }

        public AzureSearchSuggester(IIndexResolver indexResolver, BaseSiteContextFactory siteContextFactory)
        {
            IndexResolver = indexResolver;
            SiteContextFactory = siteContextFactory;
        }

        public IEnumerable<Suggestion> GetSuggestions(SuggesterModel model, out Timer queryTimer, out string indexName)
        {
            // https://docs.microsoft.com/en-us/rest/api/searchservice/suggestions#request
            bool fuzzy = true; // Finds suggestions even if there is a substituted or missing character in the search text
            bool highlights = true; // Adds extra HTML below to highlight matches

            ISearchIndex searchIndex = IndexResolver.ResolveIndex(!model.ContextItemID.IsNull ? Data.Database.GetDatabase(GetSitecoreSite().Database).GetItem(model.ContextItemID) : null);
            indexName = searchIndex.Name;
            searchIndex.Initialize();
            using (IProviderSearchContext searchContext = searchIndex.CreateSearchContext(SearchSecurityOptions.Default))
            {
                SuggestParameters sp = new SuggestParameters() { UseFuzzyMatching = fuzzy, Top = 5 };
                if (highlights)
                {
                    sp.HighlightPreTag = "<em>";
                    sp.HighlightPostTag = "</em>";
                }

                AzureSuggestQuery term = model.Term;
                DocumentSuggestResult<Document> handlerQueryResults;
                using (queryTimer = new Timer())
                    handlerQueryResults = searchContext.Suggest(term, Configuration.Settings.GetSetting("AzureSearchSuggesterName"), sp);
                return handlerQueryResults.Results.Select(a => new Suggestion()
                {
                    Term = a.Text,
                    Payload = JsonConvert.SerializeObject(a.Document)
                });
            }
        }

        public IEnumerable<Suggestion> GetAutocomplete(SuggesterModel model, out Timer queryTimer, out string indexName)
        {
            // https://docs.microsoft.com/en-us/rest/api/searchservice/suggestions#request
            bool fuzzy = true; // Finds suggestions even if there is a substituted or missing character in the search text
            bool highlights = true; // Adds extra HTML below to highlight matches

            ISearchIndex searchIndex = IndexResolver.ResolveIndex(!model.ContextItemID.IsNull ? Data.Database.GetDatabase(GetSitecoreSite().Database).GetItem(model.ContextItemID) : null);
            indexName = searchIndex.Name;
            searchIndex.Initialize();
            using (IProviderSearchContext searchContext = searchIndex.CreateSearchContext(SearchSecurityOptions.Default))
            {
                AutocompleteParameters ap = new AutocompleteParameters() { UseFuzzyMatching = fuzzy, Top = 5 };
                if (highlights)
                {
                    ap.HighlightPreTag = "<em>";
                    ap.HighlightPostTag = "</em>";
                }

                AzureSuggestQuery term = model.Term;
                AutocompleteResult handlerQueryResults;
                using (queryTimer = new Timer())
                    handlerQueryResults = searchContext.AutoComplete(term, Configuration.Settings.GetSetting("AzureSearchSuggesterName"), ap);
                return handlerQueryResults.Results.Select(a => new Suggestion()
                {
                    Term = a.Text,
                    Payload = JsonConvert.SerializeObject(a.QueryPlusText)
                });
            }
        }

        public Web.SiteInfo GetSitecoreSite()
        {
            var url = System.Web.HttpContext.Current.Request.Url;
            var query = SiteContextFactory.GetSites().Where(n => n.HostName.Contains(url.Host));

            if (!query.Any()) return SiteContextFactory.GetSites().Where(n => n.Name == "website").First();
            if (query.Count() == 1) return query.First();

            return
                query.FirstOrDefault(n => url.AbsolutePath.Contains(n.PhysicalFolder)) ??
                query.FirstOrDefault(n => n.PhysicalFolder == "/");
        }
    }
}