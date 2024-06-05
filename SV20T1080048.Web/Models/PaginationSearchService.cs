using SV20T1080048.DomainModels;

namespace SV20T1080048.Web.Models
{
    public class PaginationSearchService : PaginationSearchBaseResult
    {
        public IList<Service> Data { get; set; }
    }
}