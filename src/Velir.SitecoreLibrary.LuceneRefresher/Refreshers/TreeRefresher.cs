using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Search;
using Sitecore.Search.Crawlers;
using Velir.SitecoreLibrary.LuceneRefresher.Interfaces;
using Velir.SitecoreLibrary.LuceneRefresher.Pruners;
using Velir.SitecoreLibrary.LuceneRefresher.Util;

namespace Velir.SitecoreLibrary.LuceneRefresher.Refreshers
{
    public class TreeRefresher : IRefresher
    {
        private readonly Index _index;
        private readonly Dictionary<ItemUri, bool> _foundItems; 

        public TreeRefresher(Index index)
        {
            _index = index;
            _foundItems = new Dictionary<ItemUri, bool>();
        }

        private IEnumerable<ICrawler> Crawlers
        {
            get { return ReflectionUtil.GetCrawler(_index); }
        }

        public void Refresh()
        {
            try
            {
                RefreshInternal();
            }
            finally
            {
                // after we refresh the index, optimize the index and commit the changes
                using (IndexUpdateContext updateContext = _index.CreateUpdateContext())
                {
                    updateContext.Optimize();
                    updateContext.Commit();
                }
            }
        }

        private void RefreshInternal()
        {
            bool fullyRefreshed = false;
            try
            {
                foreach (DatabaseCrawler crawler in Crawlers)
                {
                    // pull in the database the crawler is going to be using
                    Database db = Factory.GetDatabase(crawler.Database);

                    // get the root item for the crawler
                    Item rootItem = db.GetItem(crawler.Root);

                    // refresh based on that root item and crawler
                    Refresh(crawler, rootItem);
                }

                fullyRefreshed = true;
            }
            finally
            {
                PruneResults(_index, _foundItems, fullyRefreshed);
            }
        }

        private static void PruneResults(Index index, IDictionary<ItemUri, bool> crawledItems, bool fullCrawl)
        {
            IPruner pruner;
            if (fullCrawl)
            {
                pruner = new DictionaryPruner(crawledItems);
            }
            else
            {
                pruner = new SitecorePruner();
            }

            IndexSearchContext searchContext = index.CreateSearchContext();
            IndexReader reader = searchContext.Searcher.Reader;

            List<int> itemsToDelete = new List<int>();

            try
            {
                // first get how many documents are in the index
                int numberOfDocuments = reader.NumDocs();

                // interate over all of the documents
                for (int i = 0; i < numberOfDocuments; i++)
                {
                    // check to see if the document is deleted
                    if (reader.IsDeleted(i))
                    {
                        continue;
                    }

                    // get the actual document from the index 
                    Document doc = reader.Document(i);

                    // get the url field from the index which contains item id, version, database, etc.
                    Field field = doc.GetField(BuiltinFields.Url);
                    string path = field.StringValue();

                    // parse out the values from the index
                    ItemUri uri = new ItemUri(path);

                    if (!pruner.StillExists(uri))
                    {
                        itemsToDelete.Add(i);
                    }
                }
            }
            finally
            {
                // clean up after ourselves
                reader.Close();
                searchContext.Searcher.Close();

                if (itemsToDelete.Any())
                {
                    using (IndexDeleteContext context = index.CreateDeleteContext())
                    {
                        context.DeleteDocuments(itemsToDelete);
                        context.Commit();
                    }
                }
            }
        }

        private void Refresh(DatabaseCrawler crawler, Item current)
        {
            if (ReflectionUtil.IsMatch(crawler, current))
            {
                Update(crawler, current);
            }

            // iterate all children and call this same method with each child
            foreach (Item child in current.Children)
            {
                Refresh(crawler, child);
            }
        }

        private void Update(DatabaseCrawler crawler, Item current)
        {
            _foundItems[current.Uri] = true;

            // check to see if the current item is a match
            crawler.UpdateItem(current);
        }
    }
}
