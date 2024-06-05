using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DomainModels
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceForCombo { get; set; } // Nullable type for PriceForCombo
        public string Photo { get; set; }
        public int CategoryServiceID { get; set; }
    }
}
