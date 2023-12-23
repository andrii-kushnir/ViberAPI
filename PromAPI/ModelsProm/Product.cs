using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromAPI.ModelsProm
{
    public class Product
    {
        public int id { get; set; }
        public string external_id { get; set; }
        public string name { get; set; }
        public string sku { get; set; }
        public string keywords { get; set; }
        public string presence { get; set; }
        public float price { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public Group group { get; set; }
        public Category category { get; set; }
        public string main_image { get; set; }
        public string selling_type { get; set; }
        public string status { get; set; }
        public int quantity_in_stock { get; set; }
        public string measure_unit { get; set; }
        public bool is_variation { get; set; }
        public DateTime date_modified { get; set; }
        public bool in_stock { get; set; }
        public NameMultilang name_multilang { get; set; }
        public NameMultilang description_multilang { get; set; }
    }

    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public NameMultilang name_multilang { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string caption { get; set; }
    }
}