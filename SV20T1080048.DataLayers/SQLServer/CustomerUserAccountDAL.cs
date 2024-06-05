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
    public class CustomerUserAccountDAL : _BaseDAL, IUserAccountDAL
    {
        public CustomerUserAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount Authorize(string userName, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select CustomerID as UserId, Email as UserName, CustomerName as FullName, Email, Photo 
                            from Customers 
                            where Email = @userName and Password = @password";
                var parameters = new
                {
                    userName = userName,
                    password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
