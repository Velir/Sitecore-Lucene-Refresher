using System.Runtime.Serialization;

namespace Velir.SitecoreLibrary.LuceneRefresher.RemoteEvents
{
    [DataContract]
    public class LuceneRefreshRemoteEvent
    {
        [DataMember]
        public string IndexName { get; set; }

        public LuceneRefreshRemoteEvent(string indexName)
        {
            IndexName = indexName;
        }

        public LuceneRefreshRemoteEvent()
        {
            
        }
    }
}
