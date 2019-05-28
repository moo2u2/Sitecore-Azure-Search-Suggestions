using Sitecore.ContentSearch;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;
using Sitecore.XA.Foundation.Search.Models;
using Sitecore.XA.Foundation.Search.Services;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch.Security;
using Newtonsoft.Json;
using Sitecore.HabitatHome.Foundation.Search.Models;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    // See Sitecore.XA.Foundation.VersionSpecific.Services.SolrSuggester
    public class AzureSearchSuggester : ISuggester
    {
        protected const int SuggestionsCount = 5;

        protected IIndexResolver IndexResolver { get; }

        protected IContentRepository ContentRepository { get; }

        public AzureSearchSuggester(IIndexResolver indexResolver, IContentRepository contentRepository)
        {
            IndexResolver = indexResolver;
            ContentRepository = contentRepository;
        }

        public IEnumerable<Suggestion> GetSuggestions(SuggesterModel model, out XA.Foundation.Search.Timer queryTimer, out string indexName)
        {
            bool fuzzy = Configuration.Settings.GetBoolSetting("AzureSearchSuggesterFuzzy", false);
            bool highlights = Configuration.Settings.GetBoolSetting("AzureSearchSuggesterHighlight", false);
            ISearchIndex searchIndex = IndexResolver.ResolveIndex(!model.ContextItemID.IsNull ? ContentRepository.GetItem(model.ContextItemID) : null);
            indexName = searchIndex.Name;
            searchIndex.Initialize();
            using (IProviderSearchContext searchContext = searchIndex.CreateSearchContext(SearchSecurityOptions.Default))
            {
                string tag = highlights ? Configuration.Settings.GetSetting("AzureSearchSuggesterHighlightTag") : null;
                DocumentSuggestResult<Document> handlerQueryResults;
                using (queryTimer = new XA.Foundation.Search.Timer())
                    handlerQueryResults = searchContext.Suggest(new SuggestionRequest {
                        Search = model.Term,
                        SuggesterName = Configuration.Settings.GetSetting("AzureSearchSuggesterName"),
                        Fuzzy = fuzzy,
                        Top = 5,
                        HighlightPreTag = highlights ? $"<{tag}>" : null,
                        HighlightPostTag = highlights ? $"</{tag}>" : null,
                        Select = "*"
                    });
                return handlerQueryResults.Results.Select(a => new Suggestion()
                {
                    Term = a.Text,
                    Payload = JsonConvert.SerializeObject(a.Document)
                });
            }
        }
    }
}