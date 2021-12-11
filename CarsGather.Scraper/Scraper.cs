using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarsGather.Scraper.Dtos;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CarsGather.Scraper
{
    public class Scraper
    {
        private readonly IWebDriver _driver;
        private const string _baseUrl = "https://www.cars.com/";

        public Scraper()
        {
            this._driver = new ChromeDriver();
        }

        public async Task GoToWebsite()
        {
            this._driver.Navigate().GoToUrl(_baseUrl);
            await _waitUntilElement(By.TagName("header"));
        }

        public async Task InitializeLogin()
        {
            // Open Menu
            this._driver.FindElement(By.XPath("//*[@id=\"mobile-menu-button\"]")).Click();


            //Click On SignInButton
            this._driver.FindElement(By.XPath("//*[@id=\"mobile-menu-section\"]/div/a[1]")).Click();

            await _waitUntilElement(By.XPath("//*[@id=\"ae-main-content\"]/section/header/h1"));
        }

        public async Task Login()
        {
            //Fill Login Form
            this._driver.FindElement(By.Name("user[email]")).SendKeys("johngerson808@gmail.com");
            this._driver.FindElement(By.Name("user[password]")).SendKeys("test8008");

            //Click On Login Button
            this._driver
                .FindElement(By.XPath("//*[@id=\"ae-main-content\"]/section/div/div/div[1]/form/div[3]/div/button"))
                .Click();

            await _waitUntilUrl(_baseUrl);
            
            await _waitUntilElement(By.TagName("header"));
        }

        public async Task InitializeSearching()
        {
            // Select Used Cars
            var selectElement = this._driver.FindElement(By.XPath("//*[@id=\"make-model-search-stocktype\"]"));
            var options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "Used cars").Click();

            // Select Company
            selectElement = this._driver.FindElement(By.XPath("//*[@id=\"makes\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "Tesla").Click();
            
            
            // Select Model
            selectElement = this._driver.FindElement(By.XPath("//*[@id=\"models\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "Model S").Click();
            
            // Select Price
            selectElement = this._driver.FindElement(By.XPath("//*[@id=\"make-model-max-price\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "$100,000").Click();            
            
            // Select Distance
            selectElement = this._driver.FindElement(By.XPath("//*[@id=\"make-model-maximum-distance\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "All miles from").Click();
            
            //Fill ZipCode
            this._driver.FindElement(By.XPath("//*[@id=\"make-model-zip\"]")).SendKeys("94596");
            
            //Click On Search Button
            this._driver
                .FindElement(By.XPath("//*[@id=\"by-make-tab\"]/div/div[7]/button"))
                .Click();
            
            await _waitUntilElement(By.XPath("//*[@id=\"search-live-content\"]/header/span"));
        }

        public async Task<List<VehicleMinimalInfo>> GetVehiclesMinimalInfo()
        {
            var vehicles = this._driver.FindElements(By.ClassName("vehicle-card"));
            
            var vehiclesMinimalInfo = vehicles.Select(x => new VehicleMinimalInfo()
            {
                Id = Guid.Parse(x.GetAttribute("Id")),
                Model = x.FindElement(By.TagName("h2")).Text,
                UsedMiles = int.Parse(x.FindElement(By.ClassName("mileage")).Text.Replace(" mi.", string.Empty).Replace(",", string.Empty)),
                Price = int.Parse(x.FindElement(By.ClassName("primary-price")).Text.Replace("$", string.Empty).Replace(",", string.Empty)),
                Dealer = x.FindElement(By.ClassName("dealer-name")).FindElement(By.TagName("strong")).Text,
                Images = x.FindElements(By.TagName("img")).Select(y=>y.GetAttribute("data-src")).ToList()
            }).ToList();

            return vehiclesMinimalInfo;
        }

        private async Task _waitUntilUrl(string url, int checkingTime = 2, int pollingCount = 15)
        {
            for (var i = 0; i < pollingCount; i++)
            {
                if (_driver.Url == url)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(checkingTime));
            }
        }

        private async Task _waitUntilElement(By by, int checkingTime = 2, int pollingCount = 15)
        {
            for (var i = 0; i < pollingCount; i++)
            {
                try
                {
                    _driver.FindElement(by);
                    break;
                }
                catch (NoSuchElementException)
                {
                    await Task.Delay(TimeSpan.FromSeconds(checkingTime));
                }
            }
        }
    }
}