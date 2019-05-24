using Sitecore.ContentSearch;

namespace Sitecore.HabitatHome.Foundation.Search
{
    // Straight from Sitecore.ContentSearch.Azure.ISwitchSearchIndexInitializable
    public interface ISwitchSearchIndexInitializable
    {
        void Initialize(ISearchIndex searchIndex, string indexName);
    }
}
