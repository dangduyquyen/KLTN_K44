using Microsoft.AspNetCore.Mvc;
using SV20T1080048.DomainModels;

namespace SV20T1080048.Web.Models
{
    public class PaginationSearchCategoryService : PaginationSearchBaseResult
    {
            public IList<CategoryService> Data { get; set; }
     
    }
}
