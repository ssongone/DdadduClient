using ScraperDll.Entity;
using ScraperDll;
using System.Collections.Concurrent;

namespace DdadduBot.Service
{
    public class ScraperService
    {
        Scraper scraper;
        private readonly bool _isBook;
        private readonly int _option;

        public ScraperService(bool isBook, int option)
        {
            _isBook = isBook;
            _option = option;
            ListUrlPolicy policy = _isBook ? new BookUrlPolicy() : new MagazineUrlPolicy();
            scraper = new Scraper(policy);
        }

        public async Task<List<PublicationSummary>> GetPublicationSummaries()
        {
            return await scraper.ScrapeRankingPage(_option);
        }
        public async Task<List<PublicationSummary>> GetPublicationInfo()
        {
            return await scraper.ScrapeRankingPage(_option);
        }
    }


}
