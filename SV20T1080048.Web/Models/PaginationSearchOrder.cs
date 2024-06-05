using SV20T1080048.DomainModels;

namespace SV20T1080048.Web.Models
{
    public class PaginationSearchOrder: PaginationSearchBaseResult
    {
        /// <summary>
        /// Dữ liệu đầu ra
        /// </summary>
        public List<Order> Data { get; set; }

        

    }
}
