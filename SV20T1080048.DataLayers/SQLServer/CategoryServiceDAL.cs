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
    public class CategoryServiceDAL : _BaseDAL, ICommonDAL<CategoryService>
    {
        public CategoryServiceDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(CategoryService data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from CategoryService where CategoryServiceName = @CategoryServiceName)
                                select -1
                            else
                                begin
                                    INSERT INTO CategoryService (CategoryServiceName)
                                    VALUES (@CategoryServiceName);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    CategoryServiceName = data.CategoryServiceName ?? "",
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return id;
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
      
                var sql = @"select count(*) from CategoryService
                            where (@searchValue = N'') or (CategoryServiceName like @searchValue)";
                var parameters = new { searchValue = searchValue };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = "delete from CategoryService where CategoryServiceID = @categoryServiceID and not exists(select * from Services where CategoryServiceId = @categoryServiceId)";
                var parameters = new { categoryServiceId = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public CategoryService? Get(int id)
        {
            CategoryService? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from CategoryService where CategoryServiceId = @categoryServiceId";
                var parameters = new { categoryServiceId = id };
                data = connection.QueryFirstOrDefault<CategoryService>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where CategoryServiceId = @categoryServiceId)
                                select 1
                            else 
                                select 0";
                var parameters = new { categoryServiceId = id };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IList<CategoryService> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<CategoryService> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by CategoryServiceName) as RowNumber
	                            from	CategoryService
	                            where	(@searchValue = N'') or (CategoryServiceName like @searchValue)
                            )
                            select * from cte
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = (connection.Query<CategoryService>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<CategoryService>();
            return data;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        

        public bool Update(CategoryService data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from CategoryService where CategoryServiceId <> @categoryServiceId and CategoryServiceName = @categoryServiceName)
                                begin
                                    update CategoryService
                                    set CategoryServiceName = @categoryServiceName
                                    where CategoryServiceID = @CategoryServiceID
                                end";
                var parameters = new
                {
                    categoryServiceId = data.CategoryServiceID,
                    categoryServiceName = data.CategoryServiceName ?? "",
                    
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }

      
    }
}
