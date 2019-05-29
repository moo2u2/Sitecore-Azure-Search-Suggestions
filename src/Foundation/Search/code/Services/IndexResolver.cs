using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    // From Sitecore.XA.Foundation.Search.Services.IndexResolver
    // Implement custom indexes as required
    public class IndexResolver : IIndexResolver
    {
        private const string MasterIndex = "sitecore_master_index";
        private const string WebIndex = "sitecore_web_index";

        public ISearchIndex ResolveIndex()
        {
            return ResolveIndex(null);
        }

        public ISearchIndex ResolveIndex(Item contextItem)
        {
            if (contextItem != null)
            {
                if(contextItem.Database.Name.ToLower().Contains("master")) return ContentSearchManager.GetIndex(MasterIndex);
                if(contextItem.Database.Name.ToLower().Contains("web")) return ContentSearchManager.GetIndex(WebIndex);
            }
            return ResolveDefautIndexes();
        }

        protected virtual ISearchIndex ResolveDefautIndexes()
        {
            if (Sitecore.Context.PageMode.IsNormal)
            {
                if (IndexExists(WebIndex))
                    return ContentSearchManager.GetIndex(WebIndex);
                return ResolveSitecoreIndex();
            }
            if (IndexExists(MasterIndex))
                return ContentSearchManager.GetIndex(MasterIndex);
            return ResolveSitecoreIndex();
        }

        protected virtual bool IndexExists(string indexId)
        {
            return Factory.GetConfigNode("contentSearch/configuration/indexes/index[@id='" + indexId + "']") != null;
        }

        protected virtual ISearchIndex ResolveSitecoreIndex()
        {
            return ContentSearchManager.GetIndex((SitecoreIndexableItem)Sitecore.Context.ContentDatabase.GetRootItem());
        }
    }
}