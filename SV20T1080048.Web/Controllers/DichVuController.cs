using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using SV20T1080048.BusinessLayers;
using SV20T1080048.DomainModels;
using SV20T1080048.Web.AddCodes;
using SV20T1080048.Web.Models;
using System.Drawing.Printing;

namespace SV20T1080048.Web.Controllers
{
    public class DichVuController : Controller
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
        public async Task<IActionResult> Search(PaginationSearchInput input)
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

            // Lưu dữ liệu vào Redis Cache
            await ApplicationContext.SetCacheDataAsync(SERVICE_SEARCH, model, TimeSpan.FromMinutes(60));

            return View(model);
        }

        // Hiển thị form đăng ký
        [HttpGet]
        public ActionResult BookService(string serviceName, int serviceID)
        {
            var userData = User.GetUserData(); // Giả sử bạn có một phương thức để lấy thông tin người dùng

            if (userData == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đặt lịch.";
                return RedirectToAction("Login", "UserAccount"); // Điều hướng đến trang đăng nhập
            }

            var booking = new Booking
            {
                ServiceName = serviceName,
                ServiceID = serviceID
            };
            return View(booking);
        }
        [HttpPost]
        public IActionResult AddBookService(int customerID, int serviceID, DateTime appointmentDate, TimeSpan appointmentTime, string notes)
        {
            int appointmentID = AppointmentDataService.AddAppointment(customerID, serviceID, appointmentDate, appointmentTime, notes);
            return RedirectToAction("QuanLyLichHen", "TaiKhoan");
        }

    }
}
