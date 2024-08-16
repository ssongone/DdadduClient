using ScraperDll.Entity;
using ScraperDll;
using System.Collections.Concurrent;

namespace DdadduBot.Service
{
    public class ScraperService
    {
        Scraper scraper;
        ExcelExporter excelExporter;
        private readonly bool _isBook;
        private readonly int _option;

        public ScraperService(bool isBook, int option)
        {
            _isBook = isBook;
            _option = option;
            ScrapePolicy policy = _isBook ? new BookPolicy() : new MagazinePolicy();
            scraper = new Scraper(policy);
            excelExporter = new ExcelExporter(isBook, option);
        }

        public async Task<List<PublicationSummary>> GetPublicationSummaries()
        {
            return await scraper.ScrapeRankingPage(_option);
        }
        public async Task<List<Publication>> GetPublications(List<PublicationSummary> summaries)
        {
            return await scraper.ScrapePublicationDetailParallel(summaries);
        }

        public void ExportPublications(List<Publication> publications)
        {
            excelExporter.RegisterAtOnce(publications);
        }

    }


}
