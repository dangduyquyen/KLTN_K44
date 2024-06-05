using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080048.BusinessLayer;
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
    public class ServiceController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SERVICE_SEARCH = "Service_Search";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SERVICE_SEARCH);
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
            var data = ServiceDataService.ListServices(
                                            
                                            input.Page,
                                            input.PageSize,
                                            input.SearchValue ?? "",
                                            input.CategyrySerViceID ?? 0,
                                            out rowCount
                                            );
            var model = new PaginationSearchService()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(SERVICE_SEARCH, model);

            return View(model);
        }
        public IActionResult Create()
        {
            var model = new Service()
            {
                CategoryServiceID = 0
            };
            ViewBag.Title = "Bổ sung dịch vụ";
            return View(model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật dịch vụ:";
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            var data = ServiceDataService.GetService(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View("Create", data);
        }
        public IActionResult Save(Service model, IFormFile? uploadPhoto)
        {
            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Employee
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\service", fileName);
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

            //Lưu dữ liệu(lưu model vào database)
            if (model.ServiceID == 0)
            {
                ServiceDataService.AddService(model);
            }
            else
            {
                ServiceDataService.UpdateService(model);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = ServiceDataService.DeleteService(id);
                if (!success)
                    TempData["ErrorMessage"] = "Không thể xóa dịch vụ này";
                return RedirectToAction("Index");
            }
            var model = ServiceDataService.GetService(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
    }
}
