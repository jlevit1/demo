using System.Collections;
using System.Collections.Generic;

namespace demo.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //relationship: One Brand - Many Products
        public ICollection<Product> Products { get; set; }
    }
}
