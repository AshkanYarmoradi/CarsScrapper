using System;
using System.IO;
using System.Linq;
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

            var vehiclesMinimalInfo = await scraper.GetVehiclesMinimalInfo();
            
            scraper.GetFullScreenShot(Path.GetTempFileName());

            await scraper.GoToNextPage();
            
            scraper.GetFullScreenShot(Path.GetTempFileName());
            
            vehiclesMinimalInfo.AddRange(await scraper.GetVehiclesMinimalInfo());

            var vehicleFull =
                await scraper.GetVehicleFullInfo(vehiclesMinimalInfo.OrderBy(x => Guid.NewGuid()).First());
        }
    }
}