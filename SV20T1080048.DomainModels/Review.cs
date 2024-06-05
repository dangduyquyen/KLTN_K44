using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DomainModels
{
    public class Review
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName {  get; set; }
    }
}
