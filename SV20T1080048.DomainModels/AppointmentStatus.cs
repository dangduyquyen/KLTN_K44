using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DomainModels
{
    public class AppointmentStatus
    {
        /// <summary>
        /// Lịch hẹn vừa được khởi tạo (đang chờ duyệt)
        /// </summary>
        public const int INIT = 1;
        /// <summary>
        /// Lịch hẹn đã được duyệt
        /// </summary>
        public const int ACCEPTED = 2;

        public const int FINISHED = 3;
        /// <summary>
        /// Lịch hẹn bị hủy
        /// </summary>
        public const int CANCEL = -1;
        /// <summary>
        /// Lịch hẹn bị từ chối
        /// </summary>
        public const int REJECTED = -2;
    }
}
