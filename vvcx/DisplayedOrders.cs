using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vvcx
{
    public class DisplayedOrders
    {
        public DisplayedOrders()
        {
            orders = new Dictionary<string, List<Tuple<Product, int>>>();
        }

        public Dictionary<string, List<Tuple<Product, int>>> Orders { get { return orders; } }

        public void addOrder(Order order)
        {
            if (orders.ContainsKey(order.Shop.Name))
            {
                orders[order.Shop.Name].Add(order.OrderedProducts);
            }
            else
            {
                orders.Add(order.Shop.Name, new List<Tuple<Product, int>>());
                orders[order.Shop.Name].Add(order.OrderedProducts);
            }
        }

        Dictionary<string, List<Tuple<Product, int>>> orders;
    }
}
