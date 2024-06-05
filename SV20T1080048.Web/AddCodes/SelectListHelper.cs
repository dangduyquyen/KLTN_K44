using SV20T1080048.BusinessLayers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SV20T1080048.Web
{
    public class SelectListHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Province()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = " -- Chọn tỉnh thành --"
            });
            foreach (var item in CommonDataService.ListOfProvinces())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName,
                });
            }
            return list;
        }
        public static List<SelectListItem> appointment()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = " -- Chọn trạng thái --"
            });
            foreach (var item in AppointmentDataService.ListAppointments())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.Status.ToString(),
                    Text = item.StatusDescription
                });
            }
            return list;
        }

        public static List<SelectListItem> categories()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Loại hàng--"
            });
            foreach (var item in CommonDataService.ListOfCategories())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        public static List<SelectListItem> categoryService()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Loại Dịch Vụ--"
            });
            foreach (var item in CommonDataService.ListOfCategoryServices())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryServiceID.ToString(),
                    Text = item.CategoryServiceName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> suppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Nhà cung cấp--"
            });
            foreach (var item in CommonDataService.ListOfSuppliers())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }

        public static List<SelectListItem> Shippers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Chọn người giao hàng--"
            });
            foreach (var item in CommonDataService.ListOfShippers())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ShipperId.ToString(),
                    Text = item.ShipperName
                });
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Customers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Chọn khách hàng--"
            });
            foreach (var item in CommonDataService.ListOfCustomers())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CustomerID.ToString(),
                    Text = item.CustomerName
                });
            }
            return list;
        }

        public static List<SelectListItem> Employees()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--Chọn nhân viên--"
            });
            foreach (var item in CommonDataService.ListOfEmployees())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.EmployeeID.ToString(),
                    Text = item.FullName
                });
            }
            return list;
        }


    }
}
