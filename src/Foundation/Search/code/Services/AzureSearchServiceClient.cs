using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Azure.Http;
using Sitecore.Exceptions;
using Sitecore.HabitatHome.Foundation.Search.Models;
using System;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    public class AzureSearchServiceClient : ContentSearch.Azure.Http.SearchServiceClient, ISearchServiceDocumentOperationsProvider
    {
        private string _apiKey;
        private string _searchService;

        public AzureSearchServiceClient(IHttpClientFactory clientFactory, ICloudSearchRetryPolicy retryPolicy)
          : base(clientFactory, retryPolicy)
        {

        }

        public new void Initialize(string indexName, string connectionString)
        {
            base.Initialize(indexName, connectionString);

            CloudSearchServiceSettings searchServiceSettings = new CloudSearchServiceSettings(connectionString);
            if (!searchServiceSettings.Valid)
                throw new ConfigurationException($"The connection string for '{connectionString}' is incorrect.");
            _searchService = searchServiceSettings.SearchService;
            _apiKey = searchServiceSettings.ApiKey;
        }

        public DocumentSuggestResult<Document> Suggest(AzureSuggestQuery q, string suggester, SuggestParameters options)
        {
            try
            {
                Microsoft.Azure.Search.SearchServiceClient _searchClient = new Microsoft.Azure.Search.SearchServiceClient(_searchService.Replace("https://","").Replace(".search.windows.net",""), new SearchCredentials(_apiKey));
                ISearchIndexClient _indexClient = _searchClient.Indexes.GetClient(IndexName);
                return _indexClient.Documents.Suggest(q.Query, suggester, options);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AutocompleteResult Autocomplete(AzureSuggestQuery q, string suggester, AutocompleteParameters options)
        {
            try
            {
                Microsoft.Azure.Search.SearchServiceClient _searchClient = new Microsoft.Azure.Search.SearchServiceClient(_searchService.Replace("https://", "").Replace(".search.windows.net", ""), new SearchCredentials(_apiKey));
                ISearchIndexClient _indexClient = _searchClient.Indexes.GetClient(IndexName);
                return _indexClient.Documents.Autocomplete(q.Query, suggester, options);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected override string SerializeIndexDefinition(ContentSearch.Azure.Models.IndexDefinition indexDefinition)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject((indexDefinition as SuggesterIndexDefinition) ?? indexDefinition, Formatting.Indented, settings);
        }

        protected override ContentSearch.Azure.Models.IndexDefinition DeserializeIndexDefinition(string serviceResponse)
        {
            return JsonConvert.DeserializeObject<SuggesterIndexDefinition>(serviceResponse);
        }
    }
}