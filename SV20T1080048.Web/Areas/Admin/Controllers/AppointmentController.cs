using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080048.BusinessLayers;
using SV20T1080048.DomainModels;
using SV20T1080048.Web.AddCodes;
using SV20T1080048.Web.Models;

namespace SV20T1080048.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminScheme")]
    public class AppointmentController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string APPOINTMENT_SEARCH = "Appointment_Search";

        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchAppointmentInput>(APPOINTMENT_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchAppointmentInput()
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
            var data = AppointmentDataService.ListAppointments(
                                            input.Page,
                                            input.PageSize,
                                            input.SearchValue ?? "",
                                            input.CustomerID ?? 0,
                                            out rowCount
                                            );
            var model = new PaginationSearchAppointment()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(APPOINTMENT_SEARCH, model);

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var model = AppointmentDataService.GetAppointment(id);

            // Kiểm tra giá trị trả về có null không và log nếu cần
            if (model == null)
            {
                // Log để kiểm tra
                Console.WriteLine($"Không tìm thấy Appointment với ID: {id}");
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Chỉnh sửa lịch hẹn";
            return View(model);
        }

        public IActionResult Save(Appointment data)
        {
            bool success = AppointmentDataService.UpdateAppointment(data);
            if (success)
            {
                return RedirectToAction("Index", "Appointment");
            }
            ViewBag.ErrorMessage = "Không cập nhật được dữ liệu";
            return View("Edit", data);
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
