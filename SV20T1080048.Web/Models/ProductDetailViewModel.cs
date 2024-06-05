using SV20T1080048.DomainModels;

namespace SV20T1080048.Web.Models
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }

        public List<ProductPhoto> Photos { get; set; }

        public List<ProductAttribute> Attributes { get; set; }

        public List<Product> SimilarProducts { get; set; }

        public List<Review> Reviews { get; set; }

       
    }
}
