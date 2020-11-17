using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vvcx
{
    // Klasa powstała głównie do bindowania

    public class Order
    {
        // int - ilość danego produktu do zamówienia
        public Order(Shop orderShop, Tuple<Product, int> orderedProtuctList)
        {
            shop = orderShop;
            orderedProducts = orderedProtuctList;
        }

        public string Name { get { return shop.Name; } }
        public Tuple<Product, int> OrderedProducts { get { return orderedProducts; } }
        public Shop Shop { get { return shop; } }

        private Shop shop;
        private Tuple<Product, int> orderedProducts;
    }
}
