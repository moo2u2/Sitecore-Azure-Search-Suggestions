using Sitecore.ContentSearch;
using Sitecore.Data.Items;

namespace Sitecore.HabitatHome.Foundation.Search.Services
{
    public interface IIndexResolver
    {
        ISearchIndex ResolveIndex();

        ISearchIndex ResolveIndex(Item contextItem);
    }
}
