using SV20T1080048.BusinessLayers;
using SV20T1080048.DataLayers;
using SV20T1080048.DataLayers.SQLServer;
using SV20T1080048.DomainModels;

namespace SV20T1080048.BusinessLayers
{
    public class ServiceDataService
    {
        private static readonly ServiceDAL serviceDB;

        static ServiceDataService()
        {
            string connectionString = "server=DESKTOP-HVNL2AM\\SQLEXPRESS;user id=sa;password=123123;database=OanhVuSpaDB;TrustServerCertificate=true";

            serviceDB = new DataLayers.SQLServer.ServiceDAL(connectionString);
            
        }

        public static List<Service> ListServices(int page, int pageSize, string searchValue, int categoryServiceId, out int rowCount)
        {
            rowCount = serviceDB.Count(searchValue, categoryServiceId);
            return serviceDB.List(page, pageSize, searchValue, categoryServiceId).ToList();
        }
        
        public static Service? GetService(int id)
        {
            return serviceDB.Get(id);
        }
        public static int AddService(Service data)
        {
            return serviceDB.Add(data);
        }
        public static bool UpdateService(Service data)
        {
            return serviceDB.Update(data);
        }
        public static bool DeleteService(int id)
        {
            return serviceDB.Delete(id);
        }
        public static bool InUsedService(int id)
        {
            return serviceDB.InUsed(id);
        }
    }
}
