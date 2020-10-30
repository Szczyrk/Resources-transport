using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vvcx
{
    public class Shop
    {
        public Shop(string name, string city, string address, double latitude, double longitude)
        {
            Name = name;
            City = city;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            products = new List<string> { "cegła klinkierowa export 552", "piasek" }; // przykładowe produkty
        }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<string> products;
    }
}
