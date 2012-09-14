using System;
using Sitecore.Events.Hooks;

namespace Velir.SitecoreLibrary.LuceneRefresher.RemoteEvents
{
    public class LuceneRefreshHook : IHook
    {
        #region Implementation of IHook

        public void Initialize()
        {
            Sitecore.Eventing.EventManager.Subscribe(new Action<LuceneRefreshRemoteEvent>(RefreshHandler.Run));
        }

        #endregion
    }
}
