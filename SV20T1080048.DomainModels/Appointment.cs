using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DomainModels
{
    public class Appointment
    {   
        public int AppointmentID { set; get; } 
        public int CustomerID { set; get; }
        public int ServiceID { set; get; }
        public DateTime AppointmentDate { set; get; }
        public TimeSpan AppointmentTime { set; get; }
        public int Status { set; get; }
        public string? Notes { set; get; } = "";
        public DateTime? CreatedAt { set; get; }
        public DateTime? UpdatedAt { set; get;}
        public string? CustomerName { get; set; } = "";
        public string? ServiceName { get; set; } = "";
        public string StatusDescription
        {
            get
            {
                switch (Status)
                {
                    case AppointmentStatus.INIT:
                        return "Lịch hẹn mới (chờ duyệt)";
                    case AppointmentStatus.ACCEPTED:
                        return "Lịch hẹn được duyệt";
                    case AppointmentStatus.FINISHED:
                        return "Lịch hẹn đã hoàn tất";
                    case AppointmentStatus.CANCEL:
                        return "Lịch hẹn đã bị hủy";
                    case AppointmentStatus.REJECTED:
                        return "Lịch hẹn bị từ chối";
                    default:
                        return "";
                }
            }
        }
    }
}
