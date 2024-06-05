using Microsoft.AspNetCore.Mvc;
using SV20T1080048.BusinessLayer;
using SV20T1080048.BusinessLayers;
using SV20T1080048.DomainModels;
using SV20T1080048.Web.AddCodes;
using SV20T1080048.Web.Models;
using System;
using System.Linq;

namespace SV20T1080048.Web.Controllers
{
    public class SanPhamController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string PRODUCT_SEARCH = "Product_Search";
       

        

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                };
            }

            return View(input);
        }

        public async Task<IActionResult> Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(
                            input.Page,
                            input.PageSize,
                            input.SearchValue ?? "",
                            input.CategoryID ?? 0,
                            input.SupplierID ?? 0,
                            0,
                            0,
                            out rowCount
                        );

            var model = new PaginationSearchProduct()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu dữ liệu vào Redis Cache
            await ApplicationContext.SetCacheDataAsync(PRODUCT_SEARCH, model, TimeSpan.FromMinutes(60));

            return View(model);
        }

        public IActionResult ChiTietSanPham(int id, int categoryId)
        {
            var product = ProductDataService.GetProduct(id);
            var photos = ProductDataService.ListPhotos(id);
            var attributes = ProductDataService.ListAttributes(id);
            var similarProduct = ProductDataService.ListProducts(categoryId);
            // Lấy danh sách đánh giá sản phẩm từ DataService
            var reviews = ReviewDataService.ListReview(id);


            var model = new ProductDetailViewModel
            {
                Product = product,
                Photos = photos,
                Attributes = attributes,
                SimilarProducts = similarProduct,
                Reviews = (List<Review>)reviews

            };

            return View(model);
        }
        //AddReview
        [HttpPost]
        public JsonResult AddReview(int productId, int rating, string comment, string customerName)
        {
            try
            {
                var review = new Review
                {
                    ProductID = productId,
                    Rating = rating,
                    Comment = ProfanityFilter.FilterProfanity(comment),
                    CustomerName = customerName,
                    Date = DateTime.Now
                };

                // Lưu đánh giá vào database
                ReviewDataService.AddReview(review);

                return Json(new { success = true, review });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
