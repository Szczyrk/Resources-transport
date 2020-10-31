using System.IO.Packaging;

namespace vvcx
{
    public class Product
    {
        public string Name { get; set; }
        public double Weight { get; set; }

        public Product(string name, double weight){
            Name = name;
            Weight = weight;
        }
        public string ProductToBD()
        {
            return string.Format("{0},{1}", Name, Weight);
        }
    }
}