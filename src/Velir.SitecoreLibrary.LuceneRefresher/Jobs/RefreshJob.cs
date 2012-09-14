using Velir.SitecoreLibrary.LuceneRefresher.RemoteEvents;

namespace Velir.SitecoreLibrary.LuceneRefresher.Jobs
{
    public class RefreshJob
    {
        public string IndexName { get; private set; }

        public RefreshJob(string indexName)
        {
            IndexName = indexName;
        }

        public void Refresh()
        {
            RefreshHandler.TriggerLocal(IndexName);
        }
    }
}
