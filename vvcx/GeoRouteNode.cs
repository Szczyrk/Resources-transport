using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vvcx
{
    // pozostalosc po pierwotnym założeniu, chcesz to dowolnie to zmodyfikuj/zastąp czymś innym.

    public class GeoRouteNode
    {
        public GeoRouteNode(Shop from, Shop to, double routeLength)
        {
            From = from;
            To = to;
            RouteLength = routeLength;
        }

        public Shop From;
        public Shop To;
        public double RouteLength;
    }
}
