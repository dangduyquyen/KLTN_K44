using Azure;
using Dapper;
using SV20T1080048.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DataLayers.SQLServer
{
    public class ServiceDAL : _BaseDAL, IServiceDAL
    {
        public ServiceDAL(string connectionString) : base(connectionString)
        {
        }
        public int Add(Service data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Services where ServiceName = @ServiceName)
                        select -1
                    else
                        begin
                            insert into Services(ServiceName, Description, CategoryServiceID, Price,PriceForCombo, Photo)
                            values(@ServiceName,@Description,@CategoryServiceID, @Price, @PriceForCombo, @Photo);
                            select @@identity;
                        end";
                var parameters = new
                {
                    serviceName = data.ServiceName ?? "",
                    description = data.Description ?? "",
                    categoryServiceID = data.CategoryServiceID,
                    price = data.Price,
                    priceForCombo = data.PriceForCombo,
                    photo = data.Photo ?? ""
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryServiceId = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Services 
                    where (@searchValue = N'') or (ServiceName like @searchValue)";
                var parameters = new { searchValue = searchValue };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
    DELETE FROM Services WHERE ServiceID = @serviceId;
";
                var parameters = new { serviceId = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Service? Get(int id)
        {
            Service? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from Services where  ServiceId = @serviceId";
                var parameters = new { serviceId = id };
                data = connection.QueryFirstOrDefault<Service>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from  Services where  ServiceId =  @serviceId)
                        select 1
                    else 
                        select 0";
                var parameters = new { serviceId = id };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Service> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryServiceID = 0)
        {
            List<Service> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                    (
                        select	*, row_number() over (order by ServiceName) as RowNumber
                        from	Services
                        where (@searchValue = N'' or ServiceName like @searchValue)
                                    and (@categoryServiceID = 0 or CategoryServiceID = @categoryServiceID)
                    )
                    select * from cte
                    where  (@pageSize = 0) 
                        or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue,
                    categoryServiceID = categoryServiceID
                };
                data = (connection.Query<Service>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<Service>();
            return data;
        }

        public bool Update(Service data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Services where ServiceID <> @serviceID and ServiceName = @serviceName)
                begin
                    update Services 
                    set ServiceName = @serviceName,
                        Description = @description,
                        Price = @price,
                        PriceForCombo = @priceForCombo,
                        Photo = @photo,
                        CategoryServiceID = @categoryServiceID
                        
                    where ServiceID = @serviceID
                end";

                var parameters = new
                {
                    serviceID = data.ServiceID,
                    serviceName = data.ServiceName ?? "",
                    description = data.Description ?? "",
                    price = data.Price,
                    priceForCombo = data.PriceForCombo,
                    photo = data.Photo ?? "",
                    categoryServiceID = data.CategoryServiceID
                };

                // Thực hiện truy vấn và cập nhật kết quả
                result = connection.Execute(sql, parameters) > 0;
            }

            return result;
        }
    }
}
