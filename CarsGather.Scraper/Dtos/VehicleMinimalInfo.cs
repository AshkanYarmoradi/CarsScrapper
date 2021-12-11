using System;
using System.Collections.Generic;

namespace CarsGather.Scraper.Dtos
{
    public class VehicleMinimalInfo
    {
        public Guid Id { get; set; }

        public string Model { get; set; }

        public int UsedMiles { get; set; }

        public int Price { get; set; }

        public string Dealer { get; set; }

        public List<string> Images { get; set; }
    }
}