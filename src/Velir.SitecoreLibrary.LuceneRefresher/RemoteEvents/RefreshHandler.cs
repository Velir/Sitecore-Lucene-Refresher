using System;
using Sitecore.Eventing;
using Sitecore.Events;
using Sitecore.Search;
using Velir.SitecoreLibrary.LuceneRefresher.Interfaces;
using Velir.SitecoreLibrary.LuceneRefresher.Util;

namespace Velir.SitecoreLibrary.LuceneRefresher.RemoteEvents
{
    public class RefreshHandler
    {
        public void OnLuceneRefresh(object sender, EventArgs args)
        {
            string indexName = (string)Event.ExtractParameter(args, 0);

            if (!string.IsNullOrEmpty(indexName) && SearchManager.GetIndex(indexName) != null)
            {
                IRefresher refresher = RefresherUtil.GetRefresher(indexName);
                refresher.Refresh();
            }
        }

        public static void Run(LuceneRefreshRemoteEvent eventToProcess)
        {
            TriggerLocal(eventToProcess.IndexName);
        }

        public static void Trigger(string indexName, bool globally)
        {
            var inst = new LuceneRefreshRemoteEvent(indexName);
            EventManager.QueueEvent(inst, globally, true);
        }

        public static void TriggerLocal(string indexName)
        {
            Event.RaiseEvent("lucene:refresh:remote", new object[] { indexName });
        }
    }
}
