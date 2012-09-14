using Sitecore.Data;

namespace Velir.SitecoreLibrary.LuceneRefresher.Interfaces
{
    public interface IPruner
    {
        bool StillExists(ItemUri uri);
    }
}
