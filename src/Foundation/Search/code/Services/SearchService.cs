using Microsoft.Azure.Search.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Azure.Config;
using Sitecore.ContentSearch.Azure.Http;
using Sitecore.ContentSearch.Azure.Http.Model;
using Sitecore.ContentSearch.Azure.Models;
using Sitecore.ContentSearch.Azure.Schema;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.HabitatHome.Foundation.Search.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    // Mostly straight from Sitecore.ContentSearch.Azure.Http.SearchService
    public class SearchService : ISearchService, IProvideAvailabilityManager, ISearchServiceConnectionInitializable, ISearchIndexInitializable, IDisposable, ISearchServiceSchemaSyncNotification
    {
        private const string IDFieldName = "_id";
        private CloudSearchProviderIndex _searchIndex;
        private System.Threading.Timer _timer;
        private ICloudSearchIndexSchema _schema;

        public event EventHandler SchemaSynced;

        public string Name { get; private set; }

        public ISearchServiceAvailabilityManager AvailabilityManager { get; set; }

        public ContentSearch.Azure.Http.ISearchServiceDocumentOperationsProvider DocumentOperations { get; set; }

        public ISearchServiceSchemaSynchronizer SchemaSynchronizer { get; set; }

        public ICloudSearchIndexSchema Schema
        {
            get
            {
                return _schema;
            }
            private set
            {
                _schema = value;
            }
        }

        public SearchService(ISearchServiceAvailabilityManager availabilityManager, ContentSearch.Azure.Http.ISearchServiceDocumentOperationsProvider documentOperations,
          ISearchServiceSchemaSynchronizer schemaSynchronizer, string schemaUpdateInterval)
        {
            AvailabilityManager = availabilityManager;
            DocumentOperations = documentOperations;
            SchemaSynchronizer = schemaSynchronizer;
            TimeSpan period = TimeSpan.Parse(schemaUpdateInterval, CultureInfo.InvariantCulture);
            if (!(period != TimeSpan.FromMilliseconds(-1.0)))
                return;
            _timer = new System.Threading.Timer(new TimerCallback(SyncSchema), this, TimeSpan.FromSeconds(2.0), period);
        }

        public SearchService(ISearchServiceAvailabilityManager availabilityManager, ISearchServiceDocumentOperationsProvider documentOperations,
         ISearchServiceSchemaSynchronizer schemaSynchronizer, string schemaUpdateInterval)
           : this(availabilityManager, (ContentSearch.Azure.Http.ISearchServiceDocumentOperationsProvider)documentOperations, schemaSynchronizer, schemaUpdateInterval)
        {

        }

        public IndexStatistics GetStatistics()
        {
            return SchemaSynchronizer.ManagmentOperations.GetIndexStatistics();
        }

        public void PostDocuments(ICloudBatch batch)
        {
            try
            {
                PostDocumentsImpl(batch);
            }
            catch (NotFoundException ex)
            {
                SchemaSynchronizer.RefreshLocalSchema();
                PostDocumentsImpl(batch);
            }
        }

        [Obsolete("Use Search(SearchRequest requestParameters)")]
        public string Search(string expression)
        {
            EnsureSearchServiceAvailable();
            return DocumentOperations.Search(expression);
        }

        public string Search(SearchRequest requestParameters)
        {
            EnsureSearchServiceAvailable();
            return DocumentOperations.Search(requestParameters);
        }

        public DocumentSuggestResult<Document> Suggest(AzureSuggestQuery q, string suggester, SuggestParameters options)
        {
            EnsureSearchServiceAvailable();
            return ((ISearchServiceDocumentOperationsProvider)DocumentOperations).Suggest(q, suggester, options);
        }

        public AutocompleteResult Autocomplete(AzureSuggestQuery q, string suggester, AutocompleteParameters options)
        {
            EnsureSearchServiceAvailable();
            return ((ISearchServiceDocumentOperationsProvider)DocumentOperations).Autocomplete(q, suggester, options);
        }

        public void Cleanup()
        {
            if (SchemaSynchronizer.ManagmentOperations.IndexExists())
                SchemaSynchronizer.ManagmentOperations.DeleteIndex();
            SchemaSynchronizer.CleaupLocalSchema();
        }

        public virtual void Initialize(ISearchIndex index)
        {
            _searchIndex = index as CloudSearchProviderIndex;
            if (_searchIndex == null)
                throw new NotSupportedException($"Only {typeof(CloudSearchProviderIndex).Name} is supported");
            DocumentOperations.Observer = AvailabilityManager as IHttpMessageObserver;
            SchemaSynchronizer.ManagmentOperations.Observer = AvailabilityManager as IHttpMessageObserver;
            SchemaSynchronizer.ScoringProfilesRepository = _searchIndex.CloudConfiguration.ScoringProfilesRepository;
        }

        public void Initialize(string indexName, string connectionString)
        {
            var custom = DocumentOperations as AzureSearchServiceClient;
            if (custom != null)
                custom.Initialize(indexName, connectionString);
            else
                (DocumentOperations as ISearchServiceConnectionInitializable)?.Initialize(indexName, connectionString);

            (SchemaSynchronizer as ISearchServiceConnectionInitializable)?.Initialize(indexName, connectionString);
            Name = new CloudSearchServiceSettings(connectionString).SearchService;
            SchemaSynchronizer.EnsureIsInitialized();
            Schema = new CloudSearchIndexSchema(SchemaSynchronizer.LocalSchemaSnapshot);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            _timer?.Dispose();
            if (SchemaSynced == null)
                return;
            foreach (EventHandler invocation in SchemaSynced.GetInvocationList())
                SchemaSynced -= invocation;
        }

        protected virtual void OnSchemaSynced(EventArgs args)
        {
            EventHandler schemaSynced = SchemaSynced;
            if (schemaSynced == null)
                return;
            schemaSynced(this, args);
        }

        private void PostDocumentsImpl(ICloudBatch batch)
        {
            ICloudSearchIndexSchema schema = _searchIndex.SchemaBuilder?.GetSchema();
            if (schema != null)
            {
                SchemaSynchronizer.EnsureIsInSync(schema.AllFields);
                Schema = new CloudSearchIndexSchema(SchemaSynchronizer.LocalSchemaSnapshot);
                OnSchemaSynced(EventArgs.Empty);
            }
            if (!AvailabilityManager.CanWrite)
                throw new SearchServiceIsUnavailableException(_searchIndex.CloudIndexName, $"The service ${Name} is not available for write operations", null);

            string json = batch.GetJson();
            try
            {
                DocumentOperations.PostDocuments(json);
            }
            catch (PostFailedForSomeDocumentsException ex)
            {
                StringBuilder stringBuilder = new StringBuilder("Post failed for some documents");
                foreach (MultiStatusResponseDocument document1 in ex.Documents)
                {
                    MultiStatusResponseDocument document = document1;
                    Dictionary<string, object> dictionary = batch.Documents.FirstOrDefault(d => string.Equals((string)d[CloudSearchConfig.VirtualFields.CloudUniqueId], document.Key, StringComparison.OrdinalIgnoreCase));
                    CloudSearchFieldConfiguration fieldConfiguration = _searchIndex.Configuration.FieldMap.GetFieldConfiguration(IDFieldName) as CloudSearchFieldConfiguration;
                    string str = fieldConfiguration == null ? document.Key : (dictionary == null || !dictionary.ContainsKey(fieldConfiguration.CloudFieldName) ? document.Key : dictionary[fieldConfiguration.CloudFieldName].ToString());
                    stringBuilder.AppendLine($"Document id: {str}, message: {document.Message}");
                }
                CrawlingLog.Log.Warn(stringBuilder.ToString(), null);
                CrawlingLog.Log.Debug(ex.RawResponse, null);
            }
        }

        private void EnsureSearchServiceAvailable()
        {
            if (!AvailabilityManager.CanRead)
                throw new SearchServiceIsUnavailableException(_searchIndex.CloudIndexName, $"The service ${Name} is not available for read operations", null);
        }

        private void SyncSchema(object state)
        {
            try
            {
                SchemaSynchronizer.RefreshLocalSchema();
                Interlocked.Exchange(ref _schema, new CloudSearchIndexSchema(SchemaSynchronizer.LocalSchemaSnapshot.ToList()));
                OnSchemaSynced(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                SearchLog.Log.Info("Schema synchronization failed", ex);
            }
        }
    }
}