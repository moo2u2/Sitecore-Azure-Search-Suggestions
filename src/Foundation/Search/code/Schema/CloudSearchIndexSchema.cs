using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Azure.Models;
using Sitecore.ContentSearch.Azure.Schema;
using System.Collections.Generic;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // Straight from Sitecore.ContentSearch.Azure.Schema.CloudSearchIndexSchema
    public class CloudSearchIndexSchema : ICloudSearchIndexSchema, ISearchIndexSchema
    {
        private readonly IDictionary<string, IndexedField> _allFields;

        public CloudSearchIndexSchema(IEnumerable<IndexedField> fields)
        {
            _allFields = new Dictionary<string, IndexedField>();
            foreach (IndexedField field in fields)
                _allFields.Add(field.Name, field);
        }

        public ICollection<string> AllFieldNames
        {
            get
            {
                return _allFields.Keys;
            }
        }

        public IEnumerable<IndexedField> AllFields
        {
            get
            {
                return _allFields.Values;
            }
        }

        public IndexedField GetFieldByCloudName(string cloudName)
        {
            IndexedField indexedField;
            if (!_allFields.TryGetValue(cloudName, out indexedField))
                return null;
            return indexedField;
        }
    }
}