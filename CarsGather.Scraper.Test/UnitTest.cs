using System.Threading.Tasks;
using Xunit;

namespace CarsGather.Scraper.Test
{
    public class UnitTest
    {
        [Fact]
        public async Task Test()
        {
            var scraper = new Scraper();
            
            await scraper.GoToWebsite();

            await scraper.InitializeLogin();

            await scraper.Login();

            await scraper.InitializeSearching();
        }
    }
}