#Lucene Index Refresher for Sitecore

This Sitecore plugin allows you to refresh Lucene indexes without dropping the entire index first. 

This plugin also leans on the Sitecore event queue, giving you the ability to rebuild indexes that live on your Content Delivery instances from your Content Management instance. To use this feature, ensure that 'EnableEventQueues' is set to true in your web.config.

## Compiling from Source?
Add your version of Sitecore.Kernel.dll and Lucene.Net.dll to the lib folder and build.