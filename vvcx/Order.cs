using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vvcx
{
    // Klasa powstała głównie do bindowania

    class Order
    {
        // int - ilość danego produktu do zamówienia
        public Order(Shop orderShop, List<Tuple<Product, int>> orderedProtuctList)
        {
            shop = orderShop;
            orderedProducts = orderedProtuctList;
        }

        public string Name { get { return shop.Name; } }
        public List<Tuple<Product, int>> OrderedProducts { get { return orderedProducts; } }
        public Shop Shop { get { return shop; } }

        private Shop shop;
        private List<Tuple<Product, int>> orderedProducts;

    }
}
