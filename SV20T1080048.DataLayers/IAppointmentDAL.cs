using SV20T1080048.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DataLayers
{
     public interface IAppointmentDAL
    {
        IList<Appointment> List(int page = 1, int pageSize = 0, string searchValue = "", int customerID = 0);

        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue">Tên mặt hàng cần tìm (chuỗi rỗng nếu không tìm kiếm)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (0 nếu không tìm theo loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm (0 nếu không tìm theo nhà cung cấp)</param>       
        /// <returns></returns>
        int Count(string searchValue = "", int customerID = 0);

        /// <summary>
        /// Lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="Appointment"></param>
        /// <returns></returns>
        Appointment? Get(int appointmentID);

        /// <summary>
        /// Bổ sung mặt hàng mới (hàm trả về mã của mặt hàng được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(Appointment data);

        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Appointment data);

        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="AppointmentID"></param>
        /// <returns></returns>
        bool Delete(int appointmentID);

        /// <summary>
        /// Kiểm tra xem mặt hàng hiện có đơn hàng liên quan hay không?
        /// </summary>
        /// <param name="AppointmentID"></param>
        /// <returns></returns>
        bool InUsed(int appointmentID);

        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (sắp xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        
    }
}
