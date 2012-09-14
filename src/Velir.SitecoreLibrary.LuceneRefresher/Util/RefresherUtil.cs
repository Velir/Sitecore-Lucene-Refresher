using Sitecore.Search;
using Velir.SitecoreLibrary.LuceneRefresher.Interfaces;
using Velir.SitecoreLibrary.LuceneRefresher.Refreshers;

namespace Velir.SitecoreLibrary.LuceneRefresher.Util
{
    public static class RefresherUtil
    {
        public static IRefresher GetRefresher(string indexName)
        {
            Index index = SearchManager.GetIndex(indexName);
            return GetRefresher(index);
        }

        public static IRefresher GetRefresher(Index index)
        {
            return new TreeRefresher(index);
        }
    }
}
