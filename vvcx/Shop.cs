using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vvcx
{
    public class Shop
    {
        public Shop(int id,string name, string city, string address, double latitude, double longitude, List<Product> products)
        {
            Name = name;
            City = city;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            Products = products;
            Id = id;
        }

        public Shop(string name, string city, string address, double latitude, double longitude, List<Product> products)
        {
            Name = name;
            City = city;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            Products = products;
        }

        public Shop(Shop shop)
        {
            Name = shop.Name;
            City = shop.City;
            Address = shop.Address;
            Latitude = shop.Latitude;
            Longitude = shop.Longitude;
            Products = new List<Product>(shop.Products);
        }

        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Id { get; set; }

        public List<Product> Products;

        public string ShopToBD()
        {
            return string.Format("{0},{1},{2},{3},{4}", Name, City, Address, Latitude.ToString(), Longitude.ToString());
        }
    }
}
