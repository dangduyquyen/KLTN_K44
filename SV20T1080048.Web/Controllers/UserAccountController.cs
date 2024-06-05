using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using static SV20T1080048.BusinessLayers.UserAccountSevice;
using SV20T1080048.BusinessLayers;

namespace SV20T1080048.Web.Controllers
{
    public class UserAccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string userName = "", string password = "")
        {
            ViewBag.Username = userName;
            ViewBag.Password = password;

            var customerAccount = UserAccountSevice.Authorize(userName, password, TypeOfAccount.Customer);

            // Kiểm tra thông tin đăng nhập đúng hay sai
            //TODO: kiểm tra userName và password từ CSDL

            if (customerAccount != null)
            {
                // Đăng nhập thành công.
                //Tạo đối tượng lưu các thông tin của phiên đăng nhập người dùng
                WebUserData userData = new WebUserData()
                {
                    UserId = customerAccount.UserId,
                    UserName = customerAccount.UserName,
                    DisplayName = customerAccount.FullName,
                    Email = customerAccount.Email,
                    Photo = customerAccount.Photo,
                    ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    SessionId = HttpContext.Session.Id,
                    AdditionalData = "",
                    Roles = new List<string>() { WebUserRoles.Member }
                };
                //2.Thiết lập (ghi nhận) phiên đăng nhập
                await HttpContext.SignInAsync("UserScheme", userData.CreatePrincipal());
                //3. Quay lại trang chủ Admin
                return RedirectToAction("Index", "TaiKhoan");
            }
            else
            {
                //Đăng nhập không thành công
                ModelState.AddModelError("Error", "Đăng nhập không thành công");
                TempData["ErrorMessage"] = "Đăng nhập không thành công. Bạn nhập sai tài khoản hoạt mật khẩu";
                return View();
            }

        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("UserScheme");
            return RedirectToAction("Login", "UserAccount");
        }


    }
}
