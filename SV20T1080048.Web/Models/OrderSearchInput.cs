namespace SV20T1080048.Web.Models
{
    public class OrderSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// Tình trạng đơn hàng
        /// </summary>
        public int Status { get; set; }
    }
}
