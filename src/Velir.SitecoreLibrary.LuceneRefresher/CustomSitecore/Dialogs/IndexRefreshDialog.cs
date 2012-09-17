using System;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Search;
using Sitecore.Shell.Framework;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Velir.SitecoreLibrary.LuceneRefresher.RemoteEvents;

namespace Velir.SitecoreLibrary.LuceneRefresher.CustomSitecore.Dialogs
{
    public class IndexRefreshDialog : DialogForm
    {
        private Combobox indexComboBox;
        private Checkbox triggerRemotesCheckbox;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Context.ClientPage.IsEvent)
            {
                LoadIndexList();
            }
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            ListItem selectedItem = indexComboBox.SelectedItem;

            if (selectedItem == null)
            {
                SheerResponse.Alert("Please select an index");
                return;
            }

            string indexName = selectedItem.Value;

            if (SearchManager.GetIndex(indexName) == null)
            {
                SheerResponse.Alert("Unable to find that index.");
                return;
            }

            bool rebuildGlobally = triggerRemotesCheckbox.Checked;
            RefreshHandler.Trigger(indexName, rebuildGlobally);

            SheerResponse.Alert("Refresh started!");

            Windows.Close(CloseMethod.CloseWindow);
        }

        protected override void OnCancel(object sender, EventArgs args)
        {
            Windows.Close(CloseMethod.CloseWindow);
        }

        private void LoadIndexList()
        {
            SearchConfiguration configuration =
                Factory.CreateObject("search/configuration", true) as SearchConfiguration;

            if (configuration == null)
            {
                SheerResponse.Alert("Search configuration is missing!");
                return;
            }

            string[] ignoredIndex = new[] {"system"};
            foreach (var index in configuration.Indexes)
            {
                if (ignoredIndex.Any(i => i == index.Key))
                {
                    continue;
                }

                ListItem item = new ListItem()
                                    {
                                        Header = index.Key,
                                        Value = index.Key,
                                        ID = Control.GetUniqueID("I")
                                    };
                indexComboBox.Controls.Add(item);
            }
        }

    }
}
