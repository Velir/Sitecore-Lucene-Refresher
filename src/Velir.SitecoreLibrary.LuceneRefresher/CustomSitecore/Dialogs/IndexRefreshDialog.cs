﻿using System;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Jobs;
using Sitecore.Search;
using Sitecore.Shell.Framework;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Velir.SitecoreLibrary.LuceneRefresher.Interfaces;
using Velir.SitecoreLibrary.LuceneRefresher.Util;

namespace Velir.SitecoreLibrary.LuceneRefresher.CustomSitecore.Dialogs
{
    public class IndexRefreshDialog : DialogForm
    {
        private Combobox indexComboBox;

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

            IRefresher refresher = RefresherUtil.GetRefresher(indexName);

            JobOptions options = new JobOptions("RefreshSearchIndex", "index", Client.Site.Name, refresher, "Refresh")
            {
                AfterLife = TimeSpan.FromMinutes(1.0)
            };

            JobManager.Start(options);

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