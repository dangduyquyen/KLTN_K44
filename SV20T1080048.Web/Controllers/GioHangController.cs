using Microsoft.AspNetCore.Mvc;
using SV20T1080048.BusinessLayers;
using SV20T1080048.DomainModels;
using SV20T1080048.Web.AddCodes;
using SV20T1080048.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SV20T1080048.Web.Controllers
{
    public class GioHangController : Controller
    {
        private const string ORDER_SEARCH = "Order_Search";
        private const int PAGE_SIZE = 10;
        private const string CART = "CART";
        private const string ERROR_MESSAGE = "Error_Message";
        private const string CUSTOMERID = "CustomerId";

        public IActionResult Index()
        {
            var model = GetCart();
            ViewBag.ErrorMessage = TempData[ERROR_MESSAGE] ?? "";
            return View(model);
        }

        public IActionResult Search()
        {
            return View();
        }

        public IActionResult ShowCart()
        {
            var model = GetCart();
            return View(model);
        }

        public ActionResult AddToCart(CartItem data)
        {
            try
            {
                var cart = GetCart();
                int index = cart.FindIndex(m => m.ProductId == data.ProductId);
                if (index < 0)
                {
                    cart.Add(data);
                }
                else
                {
                    cart[index].Price = data.Price;
                    cart[index].Quantity += data.Quantity;
                }

                ApplicationContext.SetSessionData(CART, cart);

                var response = new
                {
                    success = true,
                    message = $"Sản phẩm {data.ProductName} đã được thêm vào giỏ hàng."
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    success = false,
                    message = ex.Message
                };

                return Json(response);
            }
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            int index = cart.FindIndex(m => m.ProductId == id);
            if (index >= 0)
                cart.RemoveAt(index);
            ApplicationContext.SetSessionData(CART, cart);
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            var cart = GetCart();
            cart.Clear();
            ApplicationContext.SetSessionData(CART, cart);
            return RedirectToAction("Home");
        }

        public ActionResult UpdateCartItem(int productId, int newQuantity)
        {
            try
            {
                var cart = GetCart();
                var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity = newQuantity;
                    ApplicationContext.SetSessionData(CART, cart);

                    return Json(new { success = true, message = "Số lượng sản phẩm đã được cập nhật." });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private static IEnumerable<OrderDetail> ConvertCartToOrderDetails(List<CartItem> shoppingCart)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            foreach (var cartItem in shoppingCart)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    ProductID = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    Unit = cartItem.Unit,
                    Quantity = cartItem.Quantity,
                    Photo = cartItem.Photo,
                    SalePrice = cartItem.Price
                };

                orderDetails.Add(orderDetail);
            }

            return orderDetails;
        }

        private List<CartItem> GetCart()
        {
            var cart = ApplicationContext.GetSessionData<List<CartItem>>(CART);
            if (cart == null)
            {
                cart = new List<CartItem>();
                ApplicationContext.SetSessionData(CART, cart);
            }
            return cart;
        }

        [HttpPost]
        public ActionResult Init()
        {
            var userData = User.GetUserData(); // Giả sử bạn có một phương thức để lấy thông tin người dùng

            if (userData == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đặt hàng.";
                return RedirectToAction("Login", "UserAccount"); // Điều hướng đến trang đăng nhập
            }

            List<CartItem> shoppingCart = GetCart();
            TempData[CUSTOMERID] = userData.UserId;
            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                TempData[ERROR_MESSAGE] = "Không thể tạo đơn hàng với giỏ hàng trống";
                return RedirectToAction("Create");
            }

            int orderID = OrderDataService.InitOrder(int.Parse(userData.UserId), 33, DateTime.Now, ConvertCartToOrderDetails(shoppingCart));
            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            HttpContext.Session.Remove(CART); //Xóa giỏ hàng 

            return RedirectToAction("QuanLyDonHang", "TaiKhoan");
        }
    }
}
