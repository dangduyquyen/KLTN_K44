using SV20T1080048.DataLayers;
using SV20T1080048.DataLayers.SQLServer;
using SV20T1080048.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.BusinessLayers
{
    public class AppointmentDataService
    {
        private static readonly IAppointmentDAL appointmentDB;

        static AppointmentDataService()
        {
            string connectionString = "server=DESKTOP-HVNL2AM\\SQLEXPRESS;user id=sa;password=123123;database=OanhVuSpaDB;TrustServerCertificate=true";
            appointmentDB = new DataLayers.SQLServer.AppointmentDAL(connectionString);
        }

        public static List<Appointment> ListAppointments(string searchValue = "")
        {
            return appointmentDB.List(1, 0, searchValue, 0).ToList();
        }
        public static List<Appointment> ListAppointments(int customerId)
        {
            return appointmentDB.List(1, 0, "", customerId).ToList();
        }
        public static List<Appointment> ListAppointments(int page, int pageSize, string searchValue, int customerId, out int rowCount)
        {
            rowCount = appointmentDB.Count(searchValue, customerId);
            return appointmentDB.List(page, pageSize, searchValue, customerId).ToList();
        }

        public static int AddAppointment(int customerID,int serviceID, DateTime appointmentDate, TimeSpan appointmentTime, string notes)
        {
            Appointment data = new Appointment()
            {
                CustomerID = customerID,
                ServiceID = serviceID,
                AppointmentDate = appointmentDate,
                AppointmentTime = appointmentTime,
                Status = AppointmentStatus.INIT,
                Notes = notes,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            return appointmentDB.Add(data);
        }
        public static Appointment? GetAppointment(int appointmentID)
        {
            // Đảm bảo rằng phương thức Get trong appointmentDB không null
            return appointmentDB.Get(appointmentID);
        }


        public static bool UpdateAppointment(Appointment data)
        {
            return appointmentDB.Update(data);
        }
        public static bool DeleteAppointment(int appointmentId)
        {
            return appointmentDB.Delete(appointmentId);
        }
        public static bool InUsedAppointment(int appointmentId)
        {
            return appointmentDB.InUsed(appointmentId);
        }

    }

}
