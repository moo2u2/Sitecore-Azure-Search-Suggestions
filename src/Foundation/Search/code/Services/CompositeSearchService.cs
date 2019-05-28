using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Azure.Http;
using Sitecore.ContentSearch.Azure.Http.Model;
using Sitecore.ContentSearch.Azure.Models;
using Sitecore.ContentSearch.Azure.Schema;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.HabitatHome.Foundation.Search.Exceptions;
using Sitecore.HabitatHome.Foundation.Search.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    // Mostly straight from Sitecore.ContentSearch.Azure.Http.CompositeSearchService
    public class CompositeSearchService : ISearchService, ISearchIndexInitializable, ISwitchSearchIndexInitializable
    {
        private readonly ServiceCollectionClient _serviceCollectionClient;
        private readonly IList<ISearchService> _innerServices;

        public CompositeSearchService()
          : this(new ServiceCollectionClient())
        {
        }

        public CompositeSearchService(ServiceCollectionClient serviceCollectionClient)
        {
            _serviceCollectionClient = serviceCollectionClient;
            _innerServices = new List<ISearchService>();
        }

        public string Name { get; private set; }

        public ICloudSearchIndexSchema Schema { get; private set; }

        internal IEnumerable<ISearchService> InnerServices
        {
            get
            {
                return _innerServices;
            }
        }

        public IndexStatistics GetStatistics()
        {
            IndexStatistics[] result = Task.WhenAll(_innerServices.Select(x => Task.Run(() => x.GetStatistics()))).Result;
            return new IndexStatistics()
            {
                DocumentCount = result != null ? ((IEnumerable<IndexStatistics>)result).Min(x =>
                {
                    if (x == null)
                        return 0;
                    return x.DocumentCount;
                }) : 0L,
                StorageSize = result != null ? ((IEnumerable<IndexStatistics>)result).Min(x =>
                {
                    if (x == null)
                        return 0;
                    return x.StorageSize;
                }) : 0L
            };
        }

        public void PostDocuments(ICloudBatch batch)
        {
            Parallel.ForEach(_innerServices, searchService =>
            {
                IProvideAvailabilityManager availabilityManager = searchService as IProvideAvailabilityManager;
                int num;
                if (availabilityManager == null)
                {
                    num = 0;
                }
                else
                {
                    bool? canWrite = availabilityManager.AvailabilityManager?.CanWrite;
                    bool flag = false;
                    num = canWrite.GetValueOrDefault() == flag ? (canWrite.HasValue ? 1 : 0) : 0;
                }
                if (num != 0)
                    SearchLog.Log.Warn($"Service ${searchService.Name} is not available. Data from the commit can be lost on this search service.", null);
                else
                    searchService.PostDocuments(batch);
            });
        }

        [Obsolete("Use Search(SearchRequest searchRequest)")]
        public string Search(string expression)
        {
            ISearchService availableSearchService = GetAvailableSearchService();
            if (availableSearchService != null)
                return availableSearchService.Search(expression);
            SearchLog.Log.Error("Search request cannot be processed. No search services are available. Please try again later.", null);
            return null;
        }

        public string Search(SearchRequest searchRequest)
        {
            ISearchService availableSearchService = GetAvailableSearchService();
            if (availableSearchService != null)
                return availableSearchService.Search(searchRequest);
            SearchLog.Log.Error("Search request cannot be processed. No search services are available. Please try again later.", null);
            return null;
        }

        public DocumentSuggestResult<Document> Suggest(SuggestionRequest suggestionRequest)
        {
            ISearchService availableSearchService = GetAvailableSearchService();
            if (availableSearchService != null)
                return ((SearchService)availableSearchService).Suggest(suggestionRequest);
            SearchLog.Log.Error("Search request cannot be processed. No search services are available. Please try again later.", null);
            return null;
        }

        public AutocompleteResult Autocomplete(AutocompleteRequest autocompleteRequest)
        {
            ISearchService availableSearchService = GetAvailableSearchService();
            if (availableSearchService != null)
                return ((SearchService)availableSearchService).Autocomplete(autocompleteRequest);
            SearchLog.Log.Error("Search request cannot be processed. No search services are available. Please try again later.", null);
            return null;
        }

        public void Cleanup()
        {
            Parallel.ForEach(_innerServices, searchService => searchService.Cleanup());
        }

        [Obsolete("Use Initialize(ISearchIndex index, string indexName)")]
        public void Initialize(ISearchIndex index)
        {
            Initialize(index, null);
        }

        public void Initialize(ISearchIndex index, string indexName)
        {
            CloudSearchProviderSuggestionIndex searchProviderIndex = index as CloudSearchProviderSuggestionIndex;
            if (searchProviderIndex == null)
                throw new NotSupportedException($"Only {typeof(CloudSearchProviderIndex).Name} is supported");
            ConnectionStringSettings connectionString1 = ConfigurationManager.ConnectionStrings[searchProviderIndex.ConnectionStringName];
            if (connectionString1 == null)
                throw new Sitecore.Exceptions.ConfigurationException($"The connection string for '{searchProviderIndex.ConnectionStringName}' is not provided.");
            string[] strArray = connectionString1.ConnectionString.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            IFactoryWrapper instance = _serviceCollectionClient.GetInstance<IFactoryWrapper>();
            foreach (string connectionString2 in strArray)
            {
                try
                {
                    ISearchService searchService = instance.CreateObject<ISearchService>("contentSearch/searchService", true);
                    (searchService as ISearchIndexInitializable)?.Initialize(searchProviderIndex);

                    var ssci = searchService as ISearchServiceConnectionInitializable;
                    if (ssci != null)
                    {
                        if (indexName != null)
                            ssci.Initialize(indexName, connectionString2);
                        else
                        {
                            ssci.Initialize(searchProviderIndex.SearchCloudIndexName, connectionString2);
                        }
                    }

                    if (searchService is ISearchServiceSchemaSyncNotification syncNotification)
                        syncNotification.SchemaSynced += new EventHandler(SearchServiceOnSchemaSynced);
                    _innerServices.Add(searchService);
                }
                catch (Exception ex)
                {
                    SearchLog.Log.Info("Search service connection string initialization failed.", ex);
                }
            }
            if (!_innerServices.Any())
                throw new CloudSearchCompositeSearchServiceException("There is no search service available.");
            Name = string.Join(";", _innerServices.Select(x => x.Name));
            Schema = new CloudSearchIndexSchema(ExtractSchemaSnapshot());
        }

        private ISearchService GetAvailableSearchService()
        {
            foreach (ISearchService innerService in _innerServices)
            {
                IProvideAvailabilityManager availabilityManager = innerService as IProvideAvailabilityManager;
                int num;
                if (availabilityManager == null)
                {
                    num = 0;
                }
                else
                {
                    bool? canRead = availabilityManager.AvailabilityManager?.CanRead;
                    bool flag = false;
                    num = canRead.GetValueOrDefault() == flag ? (canRead.HasValue ? 1 : 0) : 0;
                }
                if (num == 0)
                    return innerService;
                SearchLog.Log.Debug($"Search service ${innerService.Name} is not available. Tring to use another search service...", null);
            }
            return null;
        }

        private IEnumerable<IndexedField> ExtractSchemaSnapshot()
        {
            IEnumerable<IndexedField> first = null;
            foreach (ISearchService innerService in _innerServices)
                first = first == null ? innerService.Schema.AllFields : first.Intersect(innerService.Schema.AllFields, new IndexedFieldNameEqualityComparer());
            return first ?? new List<IndexedField>();
        }

        private void SearchServiceOnSchemaSynced(object sender, EventArgs eventArgs)
        {
            Schema = new CloudSearchIndexSchema(ExtractSchemaSnapshot());
        }

        private class IndexedFieldNameEqualityComparer : IEqualityComparer<IndexedField>
        {
            public bool Equals(IndexedField x, IndexedField y)
            {
                return string.Equals(x.Name, y.Name, StringComparison.Ordinal);
            }

            public int GetHashCode(IndexedField obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}