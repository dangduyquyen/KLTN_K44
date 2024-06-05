using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080048.BusinessLayers;
using SV20T1080048.DomainModels;
using SV20T1080048.Web.AddCodes;
using SV20T1080048.Web.Models;
using System.Drawing.Printing;

namespace SV20T1080048.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class CategoryServiceController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string CATEGORYSERVICE_SEARCH = "CategoryService_Search";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORYSERVICE_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCategoryService(
                                            out rowCount,
                                            input.Page,
                                            input.PageSize,
                                            input.SearchValue ?? ""
                                            );
            var model = new PaginationSearchCategoryService()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(CATEGORYSERVICE_SEARCH, model);

            return View(model);
        }

        public IActionResult Save(CategoryService data)
        {

            ViewBag.Title = data.CategoryServiceID == 0 ? "Bổ sung loại dịch vụ" : "Cập nhật loại dịch vụ";

            if (string.IsNullOrWhiteSpace(data.CategoryServiceName))
            {
                ModelState.AddModelError(nameof(data.CategoryServiceName), "Tên loại dịch vụ không được rỗng");
            }
            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }


            if (data.CategoryServiceID == 0)
            {
                int categoryServiceID = CommonDataService.AddCategoryService(data);
                if (categoryServiceID > 0)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.ErrorMessage = "Không bổ sung được dữ liệu";
                return View("Create", data);
            }
            else
            {
                bool success = CommonDataService.UpdateCategoryService(data);
                if (success)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";
                return View("Create", data);
            }
        }

        public IActionResult Create()
        {
            var model = new CategoryService()
            {
                CategoryServiceID = 0
            };
            ViewBag.Title = "Bổ sung loại dịch vụ";
            return View(model);
        }
        public IActionResult Edit(int id = 0)
        {
            var model = CommonDataService.GetCategoryService(id);
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.Title = "Cập nhật loại dịch vụ";
            return View("Create", model);
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteCategoryService(id);
                if (!success)
                    TempData["ErrorMessage"] = "Không thể xóa dịch vụ này";
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCategoryService(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

    }
}
