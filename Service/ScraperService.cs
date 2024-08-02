using ScraperDll.Entity;
using ScraperDll;

namespace DdadduBot.Service
{
    public class ScraperServiceManager
    {
        private static readonly Dictionary<(bool, int), ScraperService> _services = new();

        public static ScraperService GetService(bool isBook, int option)
        {
            var key = (isBook, option);

            if (!_services.ContainsKey(key))
            {
                _services[key] = new ScraperService(isBook, option);
            }

            return _services[key];
        }
        public static void RemoveService(bool isBook, int option)
        {
            var key = (isBook, option);
            if (_services.ContainsKey(key))
            {
                _services.Remove(key);
            }
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
