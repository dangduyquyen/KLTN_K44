using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SV20T1080048.BusinessLayers;
using SV20T1080048.DomainModels;
using SV20T1080048.Web.Models;
using static SV20T1080048.BusinessLayers.UserAccountSevice;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using SV20T1080048.Web.AddCodes;
using System.Drawing.Printing;

namespace SV20T1080048.Web.Controllers
{
    [Authorize(AuthenticationSchemes = "UserScheme")]
    public class TaiKhoanController : Controller
    {
        private const string ORDER_SEARCH = "Order_Search";
        private const string APPOINTMENT_SEARCH = "Appointment_Search";
        private const string ERROR_MESSAGE = "Error_Message";
        private const int PAGE_SIZE = 5;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QuanLyTaiKhoan(int id)
        {
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public IActionResult QuanLyDonhang(PaginationSearchOrderInput input)
        {
            int rowCount = 0;
            var data = OrderDataService.ListOrders(
                                            input.Page,
                                            input.PageSize,
                                            input.Status,
                                            input.SearchValue ?? "",
                                            input.CustomerID ?? 0,
                                            out rowCount
                                            );
            var model = new PaginationSearchOrder()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(ORDER_SEARCH, model);

            return View(model);
        }

        public IActionResult Details(int id = 0)
        {
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            Order order = OrderDataService.GetOrder(id);
            List<OrderDetail> orderDetails = OrderDataService.ListOrderDetails(id);

            PaginationSearchOrderDetail result = new PaginationSearchOrderDetail()
            {
                Order = order,
                OrderDetails = orderDetails
            };
            return View(result);
        }

        public IActionResult Cancel(int id = 0)
        {
            if (id < 0)
            {
                return RedirectToAction("Index");
            }

            Order data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }

            bool isCanceled = OrderDataService.CancelOrder(id);
            if (!isCanceled)
            {
                TempData[ERROR_MESSAGE] = $"Hủy bỏ đơn hàng thất bại vì trạng thái đơn hàng hiện tại là: {data.StatusDescription}";
                return RedirectToAction("QuanLyDonHang");
            }
            return RedirectToAction("QuanLyDonHang");
        }

        public IActionResult QuanLyLichHen(PaginationSearchAppointmentInput input)
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

    }
}
