using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManager.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public virtual System.Collections.Generic.ICollection<Product> Products {  get; set; }
    }
}
