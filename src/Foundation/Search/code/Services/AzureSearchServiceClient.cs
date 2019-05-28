using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Azure.Http;
using Sitecore.ContentSearch.Azure.Http.Exceptions;
using Sitecore.Exceptions;
using Sitecore.HabitatHome.Foundation.Search.Exceptions;
using Sitecore.HabitatHome.Foundation.Search.Models;
using Sitecore.HabitatHome.Foundation.Search.Serialization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    public class AzureSearchServiceClient : SearchServiceClient, ISearchServiceDocumentOperationsProvider
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ICloudSearchRetryPolicy _retryPolicy;
        private readonly Http.QueryStringFormatter _queryStringFormatter;
        private string _apiKey;
        private string _apiVersion;
        private string _searchService;

        public AzureSearchServiceClient(IHttpClientFactory clientFactory, ICloudSearchRetryPolicy retryPolicy)
          : base(clientFactory, retryPolicy)
        {
            _clientFactory = clientFactory;
            _retryPolicy = retryPolicy;
            _queryStringFormatter = new Http.QueryStringFormatter();
        }

        public new void Initialize(string indexName, string connectionString)
        {
            base.Initialize(indexName, connectionString);

            CloudSearchServiceSettings searchServiceSettings = new CloudSearchServiceSettings(connectionString);
            if (!searchServiceSettings.Valid)
                throw new ConfigurationException($"The connection string for '{connectionString}' is incorrect.");
            _searchService = searchServiceSettings.SearchService;
            _apiKey = searchServiceSettings.ApiKey;
            _apiVersion = searchServiceSettings.ApiVersion;
        }

        public virtual DocumentSuggestResult<Document> Suggest(SuggestionRequest suggestionRequest)
        {
            IHttpClient client = GetClient();
            StreamContent streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(_queryStringFormatter.RenderJsonBody(suggestionRequest))));
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using (HttpResponseMessage response = client.Post(Http.SearchUrl.PostSuggestions(IndexName, _apiVersion), streamContent, null))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                EnsureSuccessStatusCode(response);
                return JsonConvert.DeserializeObject<DocumentSuggestResult<Document>>(response.Content?.ReadAsStringAsync().GetAwaiter().GetResult(), CreateDeserializerSettings());
            }
        }

        public virtual AutocompleteResult Autocomplete(AutocompleteRequest autocompleteRequest)
        {
            IHttpClient client = GetClient();
            StreamContent streamContent = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(_queryStringFormatter.RenderJsonBody(autocompleteRequest))));
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using (HttpResponseMessage response = client.Post(Http.SearchUrl.PostAutocomplete(IndexName, _apiVersion), streamContent, null))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                EnsureSuccessStatusCode(response);
                return JsonConvert.DeserializeObject<AutocompleteResult>(response.Content?.ReadAsStringAsync().GetAwaiter().GetResult());
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

        private void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode && response.StatusCode != (HttpStatusCode)207)
                return;
            string message = string.Empty;
            if (response.Content != null)
                message = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            AzureSearchServiceRESTCallException innerException = new AzureSearchServiceRESTCallException(IndexName, message);
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(IndexName, "Error in the request URI, headers, or body", innerException);
                case HttpStatusCode.Forbidden:
                    throw new AuthorizationFailedException(IndexName, "Authorization failed, please check API key value in connectionstrings.config", innerException);
                case HttpStatusCode.NotFound:
                    throw new NotFoundException(IndexName, "Search service or index not found, please check service name in connectionstrings.config and if index exists in the portal", innerException);
                case HttpStatusCode.RequestEntityTooLarge:
                    throw new RequestEntityTooLargeException(innerException);
                case (HttpStatusCode)429:
                    throw new AzureQuotaExceededException(IndexName, "Quota for number of indexes of documents per index exeeded, consider updrgading search service to next service tier", innerException);
                default:
                    throw new AzureSearchServiceRESTCallException(IndexName, "Error while search service call, see details in message", innerException);
            }
        }

        private IHttpClient GetClient()
        {
            if (Observer == null)
                throw new CloudSearchMissingImplementationException("Observer is null.", "IHttpMessageObserver");
            return _clientFactory.Get(_searchService, _apiKey, Observer, _retryPolicy);
        }

        private JsonSerializerSettings CreateDeserializerSettings()
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            };
            serializerSettings.Converters.Add(new DocumentConverter());
            serializerSettings.Converters.Add(new SuggestResultConverter<Document>());
            return serializerSettings;
        }
    }
}