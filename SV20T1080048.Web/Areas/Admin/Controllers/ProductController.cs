using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SV20T1080048.BusinessLayer;
using SV20T1080048.BusinessLayers;
using SV20T1080048.DomainModels;
using SV20T1080048.Web;
using SV20T1080048.Web.AddCodes;
using SV20T1080048.Web.Models;
using System.Drawing.Printing;
using System.Reflection;

namespace SV20T1080048.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class ProductController : Controller
    {

        private const int PAGE_SIZE = 10;
        private const string PRODUCT_SEARCH = "Product_Search";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        public IActionResult Search(PaginationSearchInput input)
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

            ApplicationContext.SetSessionData(PRODUCT_SEARCH, model);

            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var data = new Product()
            {
                ProductID = 0
            };
            return View(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            Product product = ProductDataService.GetProduct(id);
            List<ProductAttribute> productAttributes = ProductDataService.ListAttributes(id);
            List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(id);
            if (product == null || productAttributes == null || productPhotos == null)
            {
                return RedirectToAction("Index");
            }
            SV20T1080048.Web.Models.ProductEdit data = new SV20T1080048.Web.Models.ProductEdit()
            {
                Product = product,
                ProductAttributes = productAttributes,
                ProductPhotos = productPhotos
            };
            return View(data);
        }


        public IActionResult Save(Product model, IFormFile? uploadPhoto)
        {
            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Employee
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }

            //Kiểm tra đầu vào của model
            if (!ModelState.IsValid)
                return Content("Có lỗi xảy ra");



            //return Json(model); // Kiểm tra dữ liệu nhập vào

            //Lưu dữ liệu (lưu model vào database)
            if (model.ProductID == 0)
            {
                ProductDataService.AddProduct(model);
            }
            else
            {
                ProductDataService.UpdateProduct(model);
            }
            return RedirectToAction("Index");


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = ProductDataService.DeleteProduct(id);
                if (!success)
                    TempData["ErrorMessage"] = "Không thể xóa loại hàng này";
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        /// 

        [Route("~/Admin/Product/Photo/{method?}/{productID}/{photoID?}")]
        public IActionResult Photo(string method = "add", int productID = 0, long photoID = 0)
        {
            if (productID < 0)
            {
                return RedirectToAction("Index");
            }
            ProductPhoto data = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    data = new ProductPhoto()
                    {
                        PhotoID = 0,
                        ProductID = productID
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    if (photoID < 0)
                    {
                        return RedirectToAction("Index");
                    }
                    data = ProductDataService.GetPhoto(photoID);
                    if (data == null)
                    {
                        return RedirectToAction("index");
                    }
                    return View(data);
                case "delete":
                    ProductDataService.DeletePhoto(photoID);
                    return RedirectToAction("Edit", new { id = productID }); // Chuyển hướng đến trang chỉnh sửa sản phẩm
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult SavePhoto(ProductPhoto model, IFormFile? uploadPhoto)
        {
            // Xử lý với ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }

            // Kiểm tra đầu vào của model
            if (!ModelState.IsValid)
                return Content("Có lỗi xảy ra");

            // Lưu dữ liệu (lưu model vào database)
            if (model.PhotoID == 0)
            {
                ProductDataService.AddPhoto(model);
            }
            else
            {
                ProductDataService.UpdatePhoto(model);
            }

            // Chuyển hướng đến trang chỉnh sửa sản phẩm
            return RedirectToAction("Edit", new { id = model.ProductID });
        }


        //===================================================================Attribute

        [Route("~/Admin/Product/Attribute/{method?}/{productID}/{attributeID?}")]
        public ActionResult Attribute(string method = "add", int productID = 0, int attributeID = 0)
        {
            if (productID <= 0)
            {
                return RedirectToAction("Index");
            }
            ProductAttribute data = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    data = new ProductAttribute()
                    {
                        AttributeID = 0,
                        ProductID = productID,
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    if (attributeID <= 0)
                    {
                        return RedirectToAction("Edit", new { productID = productID });
                    }
                    data = ProductDataService.GetAttribute(attributeID);
                    if (data == null)
                    {
                        return RedirectToAction("Edit", new { productID = productID });
                    }
                    return View(data);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeID);
                    return RedirectToAction("Edit", new { productID = productID });
                default:
                    return RedirectToAction("Edit", new { productID = productID });
            }
        }

        public IActionResult SaveAttribute(ProductAttribute data)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.AttributeName))
            {
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
            {
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            }

            if (data.DisplayOrder <= 0)
            {
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị thuộc tính phải > 0");
            }

            List<ProductAttribute> productAttributes = ProductDataService.ListAttributes(data.ProductID);
            bool isUsedDisplayOrder = productAttributes.Any(item => item.DisplayOrder == data.DisplayOrder && data.AttributeID != item.AttributeID);
            if (isUsedDisplayOrder)
            {
                ModelState.AddModelError(nameof(data.DisplayOrder), $"Thứ tự hiển thị {data.DisplayOrder} của thuộc tính đã được sử dụng trước đó");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                return View("Attribute", data);
            }

            // Thực hiện thêm hoặc cập nhật
            if (data.AttributeID == 0)
            {
                ProductDataService.AddAttribute(data);
            }
            else
            {
                ProductDataService.UpdateAttribute(data);
            }
            return RedirectToAction("Edit", new { productID = data.ProductID });
        }


    }
}