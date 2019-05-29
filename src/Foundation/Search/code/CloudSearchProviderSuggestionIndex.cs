using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure;
using Sitecore.ContentSearch.Azure.Events.RebuildEvents;
using Sitecore.ContentSearch.Azure.Factories;
using Sitecore.ContentSearch.Azure.Http;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using System;
using System.Threading;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // Pretty much straight from CloudSearchProviderIndex but SearchCloudIndexName and indexNameProvider weren't accessible
    public class CloudSearchProviderSuggestionIndex : CloudSearchProviderIndex
    {
        private string _searchCloudIndexName;
        private string _rebuildCloudIndexName;
        private readonly Settings _settings;

        private readonly ICloudSearchProviderIndexName indexNameProvider;

        //private readonly IList<Suggester> suggesters = new List<Suggester>();

        public virtual ISearchService RebuildService { get; set; }

        public string ConnectionStringName { get; set; }

        public virtual string SearchCloudIndexName
        {
            get
            {
                if (string.IsNullOrEmpty(_searchCloudIndexName))
                {
                    string text = indexNameProvider.GetIndexName(Name, ContentSearch.Azure.CloudSearchProviderIndexName.OperationalType.Search);
                    if (text.IsNullOrEmpty())
                    {
                        text = CloudIndexName;
                    }
                    SearchCloudIndexName = text;
                }
                return _searchCloudIndexName;
            }
            set
            {
                _searchCloudIndexName = value;
                indexNameProvider.SetIndexName(Name, ContentSearch.Azure.CloudSearchProviderIndexName.OperationalType.Search, value);
            }
        }

        public virtual string RebuildCloudIndexName
        {
            get
            {
                if (string.IsNullOrEmpty(_rebuildCloudIndexName))
                {
                    string indexName = indexNameProvider.GetIndexName(Name, ContentSearch.Azure.CloudSearchProviderIndexName.OperationalType.Rebuild);
                    if (indexName.IsNullOrEmpty())
                        indexName = indexNameProvider.GenerateIndexName(Name);
                    RebuildCloudIndexName = indexName;
                }
                return _rebuildCloudIndexName;
            }
            set
            {
                _rebuildCloudIndexName = value;
                indexNameProvider.SetIndexName(Name, ContentSearch.Azure.CloudSearchProviderIndexName.OperationalType.Rebuild, value);
            }
        }

        protected virtual bool SwitchOnRebuild
        {
            get
            {
                return _settings.SwitchOnRebuild;
            }
        }

        protected virtual TimeSpan OldIndexCleanUpDelay
        {
            get
            {
                return _settings.OldIndexCleanUpDelay;
            }
        }

        public CloudSearchProviderSuggestionIndex(string name, string connectionStringName, string totalParallelServices, IIndexPropertyStore propertyStore)
            : base(name, connectionStringName, totalParallelServices, propertyStore)
        {
            ConnectionStringName = connectionStringName;
            indexNameProvider = new CloudSearchProviderIndexName(ServiceCollectionClient.GetInstance<IFactoryWrapper>());// ServiceCollectionClient.GetInstance<ICloudSearchProviderIndexName>(Array.Empty<object>());
            _settings = ServiceCollectionClient.GetInstance<Settings>();
        }

        //public virtual void AddSuggester(Suggester suggester)
        //{
        //    Assert.ArgumentNotNull(suggester, "suggester cannot be null");
        //    suggesters.Add(suggester);
        //}

        public override void Initialize()
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionStringName))
                    ConnectionStringName = "cloud.search";

                // This needs to come before base due to FieldNameTranslator line causing other things to be called too early
                indexNameProvider.Initialize(ConnectionStringName);

                base.Initialize();
            }
            catch (Exception ex)
            {
                initialized = false;
                Log.Error($"Initialization is failed for index {CloudIndexName}", ex, this);
            }
        }

        #region Because they use SearchCloudIndexName above or ISwitchSearchIndexInitializable

        protected override IProviderUpdateContext CreateRebuildContext()
        {
            EnsureInitialized();
            RebuildService = ServiceCollectionClient.GetInstance<ISearchService>();
            var ourType = RebuildService as ISwitchSearchIndexInitializable;
            if (ourType != null)
                ourType.Initialize(this, RebuildCloudIndexName);
            else
                Type.GetType("Sitecore.ContentSearch.Azure.ISwitchSearchIndexInitializable, Sitecore.ContentSearch.Azure").GetMethod("Initialize").Invoke(RebuildService, new object[] { this, RebuildCloudIndexName });
            return ((AbstractCloudSearchContextFactory)ProviderContextFactory).CreateUpdateContext(this, RebuildService);
        }

        protected override void PerformRebuild(IndexingOptions indexingOptions, CancellationToken cancellationToken)
        {
            if (!ShouldStartIndexing(indexingOptions))
                return;
            if (SwitchOnRebuild)
            {
                DoRebuild(indexingOptions);
                ISearchService searchService = SearchService;
                EventRaiser.RaiseRebuildEndEvent(new SwitchOnRebuildEventRemote()
                {
                    IndexName = Name,
                    SearchCloudIndexName = SearchCloudIndexName,
                    RebuildCloudIndexName = RebuildCloudIndexName
                });
                Thread.Sleep(OldIndexCleanUpDelay);
                searchService.Cleanup();
            }
            else
            {
                Reset();
                DoRebuild(indexingOptions);
            }
        }

        internal void SwitchIndexes(string searchIndexName, string rebuildIndexName)
        {
            SearchCloudIndexName = rebuildIndexName;
            RebuildCloudIndexName = searchIndexName;
            ISearchService instance = ServiceCollectionClient.GetInstance<ISearchService>();
            if (!TryInitializeCloudIndexFromConnectionString(instance as ISwitchSearchIndexInitializable))
                return;
            SearchService = instance;
        }

        private bool TryInitializeCloudIndexFromConnectionString(ISwitchSearchIndexInitializable searchService)
        {
            Log.Info($"Initializing Cloud Content Search for index {CloudIndexName} from connection string...", this);
            try
            {
                searchService.Initialize(this, SearchCloudIndexName);
                Log.Info($"Cloud Content Search is initialized for index {CloudIndexName}", this);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Initialization is failed for index {CloudIndexName}", ex, this);
                return false;
            }
        }

        #endregion

    }
}