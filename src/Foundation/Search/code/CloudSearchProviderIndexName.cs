using Newtonsoft.Json.Linq;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Azure.Http;
using Sitecore.ContentSearch.Azure.Models;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.HabitatHome.Foundation.Search.Services;
using Sitecore.StringExtensions;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using static Sitecore.ContentSearch.Azure.CloudSearchProviderIndexName;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // Pretty much straight from Sitecore.ContentSearch.Azure.CloudSearchProviderIndexName except Initialize
    public class CloudSearchProviderIndexName : ICloudSearchProviderIndexName
    {
        private readonly IFactoryWrapper factoryWrapper;
        private SearchServiceClient _searchServiceClient;
        private const string _indexName = "index-catalog";

        public CloudSearchProviderIndexName(IFactoryWrapper wrapper)
        {
            factoryWrapper = wrapper;
        }

        public void Initialize(string connectionStringName)
        {
            _searchServiceClient = factoryWrapper.CreateObject<SearchServiceClient>("contentSearch/searchServiceClient", true);
            _searchServiceClient.Observer = factoryWrapper.CreateObject<ISearchServiceAvailabilityManager>("contentSearch/availabilityManager", true) as IHttpMessageObserver;
            if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
                return;
            string connectionString1 = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            foreach (string connectionString2 in connectionString1.Split(new char[1] { '|' }))
            {
                try
                {
                    AzureSearchServiceClient assc = _searchServiceClient as AzureSearchServiceClient;
                    if (assc != null)
                        assc.Initialize(_indexName, connectionString2);
                    else
                        _searchServiceClient.Initialize(_indexName, connectionString2);

                    if (_searchServiceClient.IndexExists())
                        break;
                    _searchServiceClient.CreateIndex(new IndexDefinition(null, new IndexedField[4]
                    {
                        new IndexedField("key", "Edm.String", true, false, true, false, true, false),
                        new IndexedField("sitecorename", "Edm.String", false, false, true, false, true, false),
                        new IndexedField("indextype", "Edm.String", false, false, true, false, true, false),
                        new IndexedField("servicename", "Edm.String", false, false, true, false, true, false)
                    }));
                    break;
                }
                catch (Exception ex)
                {
                    SearchLog.Log.Error("Failed to initialize CloudSearchProviderIndexName", ex);
                }
            }
        }

        public string GetIndexName(string sitecoreIndexName, OperationalType type)
        {
            IndexProps document = GetDocument(sitecoreIndexName, type);
            if (document == null)
                return string.Empty;
            return document.ServiceName;
        }

        public void SetIndexName(string sitecoreIndexName, OperationalType type, string serviceIndexName)
        {
            _searchServiceClient.PostDocuments(string.Format(CultureInfo.InvariantCulture, "{{\n                    \"value\": [\n                        {{\n                            \"@search.action\": \"mergeOrUpload\",\n                            \"key\": \"{0}\",\n                            \"servicename\": \"{1}\",\n                            \"indextype\": \"{2}\",\n                            \"sitecorename\": \"{3}\"\n                        }}\n                    ]\n                }}", $"{sitecoreIndexName.ToUpperInvariant()}-{type.ToString().ToUpperInvariant()}", serviceIndexName, type.ToString().ToUpperInvariant(), sitecoreIndexName));
        }

        public string GenerateIndexName(string originalName)
        {
            return $"{originalName}-{new Random().Next(1000)}".Replace('_', '-');
        }

        private IndexProps GetDocument(string name, OperationalType type)
        {
            return DeserializeDocument(_searchServiceClient.Search($"&$filter=sitecorename eq '{name}' and indextype eq '{type.ToString().ToUpperInvariant()}'"));
        }

        private static IndexProps DeserializeDocument(string serviceResponse)
        {
            if (serviceResponse.IsNullOrEmpty())
                return null;
            return JObject.Parse(serviceResponse)["value"].Children().FirstOrDefault()?.ToObject<IndexProps>();
        }
    }
}