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
    public class AppointmentDAL : _BaseDAL, IAppointmentDAL
    {
        public AppointmentDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Appointment data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                                    insert into Appointment(CustomerID, ServiceID, AppointmentDate, AppointmentTime, Status, Notes, CreatedAt, UpdatedAt)
                                   values(@customerID, @serviceID, @appointmentDate, @appointmentTime, @status, @notes, @createdAt, @updatedAt);
                                   select @@identity";

                var parameters = new
                {
                    customerID = data.CustomerID,
                    serviceID = data.ServiceID,
                    appointmentDate = data.AppointmentDate,
                    appointmentTime = data.AppointmentTime,
                    status = data.Status,
                    notes = data.Notes,
                    createdAt = data.CreatedAt,
                    updatedAt = data.UpdatedAt
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue = "", int customerID = 0)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"
                    select count(*)
                    from Appointment
                    where (@customerID = 0 or CustomerID = @customerID)
                    and (@searchValue = '' or Notes like '%' + @searchValue + '%')";

                var parameters = new
                {
                    customerID = customerID,
                    searchValue = searchValue
                };
                return connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            }
        }

        public bool Delete(int appointmentID)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"
                    delete from Appointment
                    where AppointmentID = @appointmentID";

                var parameters = new
                {
                    appointmentID = appointmentID
                };
                return connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
        }

        public Appointment? Get(int id)
        {
            Appointment? data = null;

            try
            {
                using (var connection = OpenConnection())
                {
                    var sql = @"
                SELECT 
                    a.*,
                    c.CustomerName,
                    s.ServiceName
                FROM 
                    Appointment a
                JOIN 
                    Customers c ON a.CustomerID = c.CustomerID
                JOIN 
                    Services s ON a.ServiceID = s.ServiceID
                WHERE 
                    a.AppointmentID = @appointmentId";
                    var parameters = new { appointmentId = id };

                    data = connection.QueryFirstOrDefault<Appointment>(
                        sql: sql,
                        param: parameters,
                        commandType: CommandType.Text
                    );
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu có
                Console.WriteLine(ex.Message);
            }

            return data;
        }

        public bool InUsed(int appointmentID)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"
                select count(*)
                from SomeRelatedTable
                where AppointmentID = @appointmentID";

                var parameters = new
                {
                    appointmentID = appointmentID
                };
                return connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
        }

        public IList<Appointment> List(int page = 1, int pageSize = 0, string searchValue = "", int customerID = 0)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"
                    select a.*, c.CustomerName, s.ServiceName
                    from Appointment a
                    join Customers c on a.CustomerID = c.CustomerID
                    join Services s on a.ServiceID = s.ServiceID
                    where (@customerID = 0 or a.CustomerID = @customerID)
                    and (@searchValue = '' or a.Notes like '%' + @searchValue + '%')
                    order by a.AppointmentDate
                    offset @offset rows fetch next @pageSize rows only";

                var parameters = new
                {
                    customerID = customerID,
                    searchValue = searchValue,
                    offset = (page - 1) * pageSize,
                    pageSize = pageSize
                };
                return connection.Query<Appointment>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
            }
        }

        public bool Update(Appointment data)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"
                    update Appointment
                    set CustomerID = @customerID,
                        ServiceID = @serviceID,
                        AppointmentDate = @appointmentDate,
                        AppointmentTime = @appointmentTime,
                        Status = @status,
                        Notes = @notes,
                        UpdatedAt = @updatedAt
                    where AppointmentID = @appointmentID";

                var parameters = new
                {
                    customerID = data.CustomerID,
                    serviceID = data.ServiceID,
                    appointmentDate = data.AppointmentDate,
                    appointmentTime = data.AppointmentTime,
                    status = data.Status,
                    notes = data.Notes,
                    updatedAt = data.UpdatedAt,
                    appointmentID = data.AppointmentID
                };
                return connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
        }
    }
}
