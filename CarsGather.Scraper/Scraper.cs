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
        private const string BaseUrl = "https://www.cars.com/";
        
        private readonly ChromeDriver _driver;

        public Scraper()
        {
            _driver = new ChromeDriver();
        }

        public async Task GoToWebsite()
        {
            _driver.Navigate().GoToUrl(BaseUrl);
            await _waitUntilElement(By.TagName("header"));
        }

        public async Task InitializeLogin()
        {
            // Open Menu
            _driver.FindElement(By.XPath("//*[@id=\"mobile-menu-button\"]")).Click();


            //Click On SignInButton
            _driver.FindElement(By.XPath("//*[@id=\"mobile-menu-section\"]/div/a[1]")).Click();

            await _waitUntilElement(By.XPath("//*[@id=\"ae-main-content\"]/section/header/h1"));
        }

        public async Task Login()
        {
            //Fill Login Form
            _driver.FindElement(By.Name("user[email]")).SendKeys("johngerson808@gmail.com");
            _driver.FindElement(By.Name("user[password]")).SendKeys("test8008");

            //Click On Login Button
            _driver
                .FindElement(By.XPath("//*[@id=\"ae-main-content\"]/section/div/div/div[1]/form/div[3]/div/button"))
                .Click();

            await _waitUntilUrl(BaseUrl);
            
            await _waitUntilElement(By.TagName("header"));
        }

        public async Task InitializeSearching(string model = "Model S")
        {
            // Select Used Cars
            var selectElement = _driver.FindElement(By.XPath("//*[@id=\"make-model-search-stocktype\"]"));
            var options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "Used cars").Click();

            // Select Company
            selectElement = _driver.FindElement(By.XPath("//*[@id=\"makes\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "Tesla").Click();
            
            
            // Select Model
            selectElement = _driver.FindElement(By.XPath("//*[@id=\"models\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == model).Click();
            
            // Select Price
            selectElement = _driver.FindElement(By.XPath("//*[@id=\"make-model-max-price\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "$100,000").Click();            
            
            // Select Distance
            selectElement = _driver.FindElement(By.XPath("//*[@id=\"make-model-maximum-distance\"]"));
            options = selectElement.FindElements(By.XPath("//option"));
            options.First(x => x.Text == "All miles from").Click();
            
            //Fill ZipCode
            _driver.FindElement(By.XPath("//*[@id=\"make-model-zip\"]")).SendKeys("94596");
            
            //Click On Search Button
            _driver
                .FindElement(By.XPath("//*[@id=\"by-make-tab\"]/div/div[7]/button"))
                .Click();
            
            await _waitUntilElement(By.XPath("//*[@id=\"search-live-content\"]/header/span"));
        }

        public List<VehicleMinimalInfo> GetVehiclesMinimalInfo()
        {
            //Get vehicles card
            var vehicles = _driver.FindElements(By.ClassName("vehicle-card"));
            
            // Parse them to our Dto Object
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

        public async Task<VehicleFullInfo> GetVehicleFullInfo(VehicleMinimalInfo vehicle)
        {
            _driver.Navigate().GoToUrl($"https://www.cars.com/vehicledetail/{vehicle.Id}/");
            
            await _waitUntilElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > h2"));
            
            var vehicleFullInfo = new VehicleFullInfo()
            {
                Id = vehicle.Id,
                Dealer = vehicle.Dealer,
                Model = vehicle.Model,
                Price = vehicle.Price,
                Images = vehicle.Images,
                UsedMiles = vehicle.UsedMiles,
                Engine = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(14)]")) ? _driver.FindElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(14)")).Text : string.Empty,
                Transmission = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(12)")) ? _driver.FindElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(12)")).Text: string.Empty,
                DriveTrain = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(6)")) ? _driver.FindElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(6)")).Text : string.Empty,
                ExteriorColor = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(2)")) ? _driver.FindElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(2)")).Text: string.Empty,
                FuelType = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(10)")) ? _driver.FindElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(10)")).Text: string.Empty,
                InteriorColor = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(4)")) ? _driver.FindElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(4)")).Text : string.Empty,
                MPG = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(8) > span > span > span")) ? _driver.FindElement(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.basics-section > dl > dd:nth-child(8) > span > span > span")).Text : string.Empty,
                Convenience = _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(2) > ul > li")) ? string.Join("\n",_driver.FindElements(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(2) > ul > li")).Select(x=>x.Text)) : string.Empty,
                Entertainment =  _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(4) > ul > li"), true) ? string.Join("\n",_driver.FindElements(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(4) > ul > li")).Select(x=>x.Text)) : string.Empty,
                Exterior =  _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(6) > ul > li"), true) ? string.Join("\n",_driver.FindElements(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(6) > ul > li")).Select(x=>x.Text)) : string.Empty,
                Safety =  _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(8) > ul > li"), true) ? string.Join("\n",_driver.FindElements(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(8) > ul > li")).Select(x=>x.Text)) : string.Empty,
                Seating =  _elementIsExist(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(10) > ul > li"), true) ? string.Join("\n",_driver.FindElements(By.CssSelector("#ae-main-content > div.vdp-content-wrapper > div.basics-content-wrapper > section.sds-page-section.features-section > dl > dd:nth-child(10) > ul > li")).Select(x=>x.Text)) : string.Empty
            };
            
            if (!_elementIsExist(By.XPath("//*[@id=\"ae-main-content\"]/div[5]/section/header/div[3]/div[2]/span")))
            {
                Console.WriteLine("Home Delivery Button not Found");
                return vehicleFullInfo;
            }
            
            _driver.FindElement(By.XPath("//*[@id=\"ae-main-content\"]/div[5]/section/header/div[3]/div[2]/span")).Click();

            vehicleFullInfo.FairDeal =
                _elementIsExist(By.XPath("//*[@id=\"sds-modal\"]/div/div[2]/ul/li[1]/div/p")) ?
                _driver.FindElement(By.XPath("//*[@id=\"sds-modal\"]/div/div[2]/ul/li[1]/div/p")).Text : string.Empty;

            vehicleFullInfo.HomeDelivery =
                _elementIsExist(By.XPath("//*[@id=\"sds-modal\"]/div/div[2]/ul/li[4]/div/p")) ? 
                _driver.FindElement(By.XPath("//*[@id=\"sds-modal\"]/div/div[2]/ul/li[4]/div/p")).Text : string.Empty;

            vehicleFullInfo.VirtualAppointment =
                _elementIsExist(By.XPath("//*[@id=\"sds-modal\"]/div/div[2]/ul/li[5]/div/p")) ?
                _driver.FindElement(By.XPath("//*[@id=\"sds-modal\"]/div/div[2]/ul/li[5]/div/p")).Text : string.Empty;

            return vehicleFullInfo;
        }

        public async Task GoToNextPage()
        {
            _driver.FindElement(By.XPath("//*[@id=\"next_paginate\"]")).Click();

            await _waitUntilElementDisappear(By.ClassName("loading"));
        }

        public void GetScreenshot(string imagePath)
        {
            ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile(imagePath, ScreenshotImageFormat.Png);
        }

        public void GetFullScreenShot(string imagePath)
        {
            Dictionary<string, Object> metrics = new Dictionary<string, Object>();
            metrics["width"] = _driver.ExecuteScript("return Math.max(window.innerWidth,document.body.scrollWidth,document.documentElement.scrollWidth)");
            metrics["height"] = _driver.ExecuteScript("return Math.max(window.innerHeight,document.body.scrollHeight,document.documentElement.scrollHeight)");
            metrics["deviceScaleFactor"] = _driver.ExecuteScript("return window.devicePixelRatio");
            metrics["mobile"] = _driver.ExecuteScript("return typeof window.orientation !== 'undefined'");
            _driver.ExecuteChromeCommand("Emulation.setDeviceMetricsOverride", metrics);

            _driver.GetScreenshot().SaveAsFile(imagePath, ScreenshotImageFormat.Png);
            
            _driver.ExecuteChromeCommand("Emulation.clearDeviceMetricsOverride", new Dictionary<string, Object>());
        }

        private bool _elementIsExist(By by, bool multiple = false)
        {
            try
            {
                if (multiple)
                {
                    var elements = _driver.FindElements(by);
                    if (elements.Count == 0)
                    {
                        throw new NoSuchFrameException();
                    }
                }
                else
                {
                    _driver.FindElement(by);
                }

                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch(NoSuchFrameException)
            {
                return false;
            }
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
        
        private async Task _waitUntilElementDisappear(By by, int checkingTime = 2, int pollingCount = 15)
        {
            for (var i = 0; i < pollingCount; i++)
            {
                try
                {
                    _driver.FindElement(by);
                    await Task.Delay(TimeSpan.FromSeconds(checkingTime));
                }
                catch (NoSuchElementException)
                {
                    break;
                }
            }
        }
    }
}