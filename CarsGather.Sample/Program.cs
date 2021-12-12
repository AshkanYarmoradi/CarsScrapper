using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarsGather.Sample
{
    class Program
    {
        private static readonly string[] CarModels = new [] { "Model 3", "Model X" };
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start Scrapping Cars.com");
            
            var scraper = new Scraper.Scraper();
            
            Console.WriteLine("Go To Website Cars.com");
            
            await scraper.GoToWebsite();
            
            Console.WriteLine("Click on Login Button");

            await scraper.InitializeLogin();
            
            Console.WriteLine("Login Submit");

            await scraper.Login();
            
            Console.WriteLine("Try Create Directory for saving files in future");

            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Attachment/");

            foreach (var carModel in CarModels)
            {
                Console.WriteLine($"Go for fetching Tesla {carModel}");
                
                await scraper.GoToWebsite();
                
                Console.WriteLine("Filling Search property into cars.com");
                
                await scraper.InitializeSearching(carModel);
                
                Console.WriteLine("Fetch All vehicle data");

                var vehiclesMinimalInfo = scraper.GetVehiclesMinimalInfo();
                
                var screenShotName = $"/Attachment/{carModel}-{Guid.NewGuid()}.png";
                
                Console.WriteLine($"Save first page screen shot in: {screenShotName}");
            
                scraper.GetFullScreenShot(Directory.GetCurrentDirectory() + screenShotName);
                
                Console.WriteLine("Go to Next Page");

                await scraper.GoToNextPage();

                screenShotName = $"/Attachment/{carModel}-{Guid.NewGuid()}.png";
                
                Console.WriteLine($"Save second page screen shot in: {screenShotName}");
            
                scraper.GetFullScreenShot(Directory.GetCurrentDirectory() + screenShotName);
                
                Console.WriteLine("Fetch All vehicle data");
            
                vehiclesMinimalInfo.AddRange(scraper.GetVehiclesMinimalInfo());

                var jsonFileName = $"/Attachment/{carModel}-{Guid.NewGuid()}.json";
                
                Console.WriteLine($"Save Search Data in: {jsonFileName}");

                await using (var fileStream = new FileStream(Directory.GetCurrentDirectory() + jsonFileName, FileMode.Create))
                {
                    await JsonSerializer.SerializeAsync(fileStream, vehiclesMinimalInfo);
                    await fileStream.DisposeAsync();
                }

                var vehicleFull =
                    await scraper.GetVehicleFullInfo(vehiclesMinimalInfo.OrderBy(x => Guid.NewGuid()).First());
                
                jsonFileName = $"/Attachment/{carModel}-specific-{Guid.NewGuid()}.json";
                
                Console.WriteLine($"Save specific car Data in: {jsonFileName}");

                await using (var fileStream = new FileStream(Directory.GetCurrentDirectory() + jsonFileName, FileMode.Create))
                {
                    await JsonSerializer.SerializeAsync(fileStream, vehicleFull);
                    await fileStream.DisposeAsync();
                }
            }
            
            Console.WriteLine("Finish");

            Console.ReadKey();
        }
    }
}
