using Azure;
using Dapper;
using Microsoft.Data.SqlClient;
using SV20T1080048.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DataLayers.SQLServer
{
    public class ReviewDAL : _BaseDAL, IReviewDAL
    {

        public ReviewDAL(string connectionString) : base(connectionString)
        {
        }



        public int AddReview(Review data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                                    INSERT INTO Reviews (ProductId, Rating, Comment, Date, CustomerName) VALUES (@ProductId, @Rating, @Comment, @Date, @CustomerName);
                                    select @@identity;
                                ";
                var parameters = new
                {
                    productId = data.ProductID,
                    rating = data.Rating,
                    comment = data.Comment,
                    date = data.Date,
                    customerName = data.CustomerName ?? "",
                    
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public IList<Review> ListReview(int productId)
        {
            List<Review> data;
       
            using (var connection = OpenConnection())
            {
                var sql = "Select * from Reviews where ProductId = @productId";
                                
                           
                var parameters = new
                {
                    productId = productId,
                    
                };
                data = (connection.Query<Review>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<Review>();
            return data;
        }
    }
}
