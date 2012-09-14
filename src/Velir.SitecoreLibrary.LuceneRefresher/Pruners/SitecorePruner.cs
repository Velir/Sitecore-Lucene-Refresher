using Sitecore.Data;
using Sitecore.Data.Items;
using Velir.SitecoreLibrary.LuceneRefresher.Interfaces;

namespace Velir.SitecoreLibrary.LuceneRefresher.Pruners
{
    public class SitecorePruner : IPruner
    {
        #region Implementation of IPruner

        public bool StillExists(ItemUri uri)
        {
            Database db = Database.GetDatabase(uri.DatabaseName);

            Item foundItem = db.GetItem(uri.ToDataUri());

            return foundItem != null;
        }

        #endregion
    }

}
