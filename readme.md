#Lucene Index Refresher for Sitecore

This Sitecore plugin allows you to refresh Lucene indexes without dropping the entire index first. 

This plugin also leans on the Sitecore event queue, giving you the ability to rebuild indexes that live on your Content Delivery instances from your Content Management instance. To use this feature, ensure that 'EnableEventQueues' is set to true in your web.config.

##How it works
Once you select the index you would like to refresh, an event is triggered locally and depending on whether you have selected to rebuild remote instances, globally.

Once the event is received, the crawlers for the selected index are enumerated and the tree is crawled using the current crawler's configured root node. While the crawler is going through the tree and finds a match for the crawler, the ItemUri is stored in a dictionary and the UpdateItem() method is called on the current crawler. In 6.4, the behavior of UpdateItem is to delete the item and add it back to the index.

After the tree is fully crawled and all of the adds/updates are processed, we need to check for deleted items. To do this we iterate through all of the documents in the Lucene index and read the _url field and then using our dictionary we built out during the crawl, confirm that the dictionary contains that ItemUri. In the case that there may be an incomplete call (maybe an exception happened during the crawl) the refresher will fall back to using Sitecore to confirm that items still exist.

After everything is finished, the index is optimized to remove all of the deleted documents in the index.

## Compiling from Source?
Add your version of Sitecore.Kernel.dll and Lucene.Net.dll to the lib folder and build.