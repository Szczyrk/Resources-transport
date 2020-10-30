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
        public Order(Shop orderShop, List<Tuple<string, double>> orderedProtuctList)
        {
            shop = orderShop;
            orderedProducts = orderedProtuctList;
        }

        public string Name { get { return shop.Name; } }
        public List<Tuple<string, double>> OrderedProducts { get { return orderedProducts; } }
        public Shop Shop { get { return shop; } }

        private Shop shop;
        private List<Tuple<string, double>> orderedProducts;

    }
}
