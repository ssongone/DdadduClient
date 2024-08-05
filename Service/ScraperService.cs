using ScraperDll.Entity;
using ScraperDll;
using System.Collections.Concurrent;

namespace DdadduBot.Service
{
    public class ScraperServiceManager
    {
        private static readonly ConcurrentDictionary<(bool, int), ScraperService> _services = new();

        public static ScraperService GetService(bool isBook, int option)
        {
            var key = (isBook, option);
            return _services.GetOrAdd(key, _ => new ScraperService(isBook, option));
        }

        public static bool ServiceExists(bool isBook, int option)
        {
            return _services.ContainsKey((isBook, option));
        }


        public static void RemoveService(bool isBook, int option)
        {
            var key = (isBook, option);
            _services.TryRemove(key, out _);
        }

    }

    public class ScraperService
    {
        private readonly bool _isBook;
        private readonly int _option;

        public ScraperService(bool isBook, int option)
        {
            _isBook = isBook;
            _option = option;
        }

        public async Task<List<PublicationSummary>> GetPublicationSummaries()
        {
            ListUrlPolicy policy = _isBook ? new BookUrlPolicy() : new MagazineUrlPolicy();
            Scraper scraper = new Scraper(policy);
            return await scraper.ScrapeRankingPage(_option);
        }
    }


}
