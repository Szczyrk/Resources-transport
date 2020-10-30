using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vvcx
{
    class Orders
    {
        public Orders()
        {
            orders = new List<Order>();
        }

        public void addOrder(Order order)
        {
            orders.Add(order);
        }

        public List<Order> OrdersList { get { return orders; } }
        private List<Order> orders;
    }
}
