using System;
using System.Collections.Generic;
using System.Reflection;
using Sitecore.Data.Items;
using Sitecore.Search;
using Sitecore.Search.Crawlers;
using System.Linq;

namespace Velir.SitecoreLibrary.LuceneRefresher.Util
{
    public static class ReflectionUtil
    {
        public static List<ICrawler> GetCrawler(Index index)
        {
            FieldInfo info = index.GetType().GetField("_crawlers", BindingFlags.NonPublic | BindingFlags.Instance);
            return info.GetValue(index) as List<ICrawler>;
        }

        public static bool IsMatch(DatabaseCrawler crawler, Item item)
        {
            IEnumerable<MethodInfo> methods = crawler.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo isMatchMethod = methods.First(x => x.Name == "IsMatch");
            return (bool)isMatchMethod.Invoke(crawler, new object[] { item });
        }
    }
}
