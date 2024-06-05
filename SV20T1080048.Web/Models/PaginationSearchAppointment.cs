using SV20T1080048.DomainModels;

namespace SV20T1080048.Web.Models
{
    public class PaginationSearchAppointment : PaginationSearchBaseResult
    {
        public IList<Appointment> Data { get; set; }
    }
}
