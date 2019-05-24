using Sitecore.ContentSearch.Azure.Analyzers;
using Sitecore.ContentSearch.Azure.Http;
using Sitecore.ContentSearch.Azure.Models;
using Sitecore.ContentSearch.Azure.Utils.Retryer;
using Sitecore.HabitatHome.Foundation.Search.Models;
using System.Collections.Generic;
using System.Linq;
using IndexDefinition = Sitecore.HabitatHome.Foundation.Search.Models.SuggesterIndexDefinition;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    public class SearchServiceSchemaSynchronizer : ContentSearch.Azure.Schema.SearchServiceSchemaSynchronizer
    {
        public SearchServiceSchemaSynchronizer(ISearchServiceManagmentOperationsProvider managmentOperations, IRertyPolicy rertyPolicy,IAnalyzerRepository analyzerRepository) 
            : base(managmentOperations, rertyPolicy, analyzerRepository)
        {
        }

        /// <summary>
        /// Straight from the base class, however added Suggesters to the index definition
        /// </summary>
        /// <param name="sourceIndexDefinition">Source index definition</param>
        /// <param name="incomingFields">Incoming fields</param>
        /// <returns></returns>
        protected override ContentSearch.Azure.Models.IndexDefinition SyncRemoteService(ContentSearch.Azure.Models.IndexDefinition sourceIndexDefinition, IEnumerable<IndexedField> incomingFields)
        {
            IEnumerable<IndexedField> mainFields = sourceIndexDefinition?.Fields ?? new List<IndexedField>();
            incomingFields = incomingFields ?? new List<IndexedField>();
            bool isModified1;
            IEnumerable<IndexedField> indexedFields = MergeFields(mainFields, incomingFields, out isModified1);
            if (!isModified1 && sourceIndexDefinition != null)
                return new IndexDefinition(sourceIndexDefinition.AnalyzerDefinitions, indexedFields, sourceIndexDefinition.ScoringProfiles, (sourceIndexDefinition as IndexDefinition)?.Suggesters);
            ContentSearch.Azure.Models.IndexDefinition index = ManagmentOperations.GetIndex();
            if (index == null)
            {
                AnalyzerDefinitions analyzers = AnalyzerRepository.GetAnalyzers();
                IEnumerable<ScoringProfile> scoringProfiles = ScoringProfilesRepository?.ScoringProfiles;
                IndexDefinition indexDefinition = new IndexDefinition(analyzers, indexedFields, scoringProfiles, new Suggester[] { new Suggester { Name = Configuration.Settings.GetSetting("AzureSearchSuggesterName"), SearchMode = "analyzingInfixMatching", SourceFields = Configuration.Settings.GetSetting("AzureSearchSuggesterFields").Split(',').ToList() } });
                ManagmentOperations.CreateIndex(indexDefinition);
                return indexDefinition;
            }
            bool isModified2;
            IEnumerable<IndexedField> fields = MergeFields(index.Fields, indexedFields, out isModified2);
            IndexDefinition indexDefinition1 = new IndexDefinition(index.AnalyzerDefinitions, fields, index.ScoringProfiles, (index as IndexDefinition)?.Suggesters);
            if (isModified2)
                ManagmentOperations.UpdateIndex(indexDefinition1);
            return indexDefinition1;
        }
    }
}