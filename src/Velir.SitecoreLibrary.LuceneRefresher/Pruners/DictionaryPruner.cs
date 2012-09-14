using System.Collections.Generic;
using Sitecore.Data;
using Velir.SitecoreLibrary.LuceneRefresher.Interfaces;

namespace Velir.SitecoreLibrary.LuceneRefresher.Pruners
{
    public class DictionaryPruner : IPruner
    {
        private readonly IDictionary<ItemUri, bool> _crawledItems;

        public DictionaryPruner(IDictionary<ItemUri, bool> crawledItems)
        {
            _crawledItems = crawledItems;
        }

        #region Implementation of IPruner

        public bool StillExists(ItemUri uri)
        {
            return _crawledItems.ContainsKey(uri);
        }

        #endregion
    }
}
