using SV20T1080048.DomainModels;

namespace SV20T1080048.Web.Models
{
    public class HomeIndexModel
    {
        public string TitleMessage {  get; set; }

        public List<Product> Product { get; set; }

        public List<ProductPhoto> ListOfProductPhoto { get; set; }

        public List<ProductAttribute> ListOfAttribute { get; set; }

    }
}
