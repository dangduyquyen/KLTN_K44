﻿using Dapper;
using SV20T1080048.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080048.DataLayers.SQLServer
{
    public class OrderDAL : _BaseDAL, IOrderDAL
    {
        public OrderDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Order data, IEnumerable<OrderDetail> details)
        {
            int orderId = 0;
            using (var connection = OpenConnection())
            {
                var sqlAddOrder = @"
                                    insert into Orders(CustomerID, OrderTime, EmployeeID, AcceptTime, ShipperID, ShippedTime, FinishedTime, Status)
                                   values(@customerID, @orderTime, @employeeID, @acceptTime, @shipperID, @shippedTime,      @finishedTime, @status);
                                   select @@identity";

                var parameters = new
                {
                    customerID = data.CustomerID,
                    orderTime = data.OrderTime,
                    employeeID = data.EmployeeID,
                    acceptTime = data.AcceptTime,
                    shipperID = data.ShipperID,
                    shippedTime = data.ShippedTime,
                    finishedTime = data.FinishedTime,
                    status = data.Status,
                };
                orderId = connection.ExecuteScalar<int>(sql: sqlAddOrder, param: parameters, commandType: CommandType.Text);

                var sqlAddDetail = @"insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice) 
                                    values(@orderID, @productID, @quantity, @salePrice);";
                foreach (var item in details)
                {
                    var detailParameters = new
                    {
                        orderID = orderId,
                        productID = item.ProductID,
                        quantity = item.Quantity,
                        salePrice = item.SalePrice,
                    };
                    connection.Execute(sql: sqlAddDetail, param: detailParameters, commandType: CommandType.Text);
                }

                connection.Close();
            }
            return orderId;
        }

        public long AddOrderDetail(OrderDetail data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice) 
                                    values(@orderID, @productID, @quantity, @salePrice);";
                var parameters = new
                {
                    orderID = data.OrderID,
                    productID = data.ProductID,
                    quantity = data.Quantity,
                    salePrice = data.SalePrice,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(int status, string searchValue, int customerID)
        {
            int count = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT COUNT(*)
                            FROM Orders AS o
                                LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                            WHERE (@status = 0 OR o.Status = @status)
                                AND (@searchValue = N'' OR c.CustomerName LIKE @searchValue OR s.ShipperName LIKE @searchValue)
                                AND (@customerID = 0 OR o.CustomerID = @customerID)";

                var parameters = new
                {
                    status = status,
                    searchValue = string.IsNullOrEmpty(searchValue) ? "" : "%" + searchValue + "%",
                    customerID = customerID
                };

                count = connection.ExecuteScalar<int>(sql, parameters);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int orderID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM OrderDetails WHERE OrderID = @orderID;                                    
                            DELETE FROM Orders WHERE OrderID = @orderID;";
                var parameters = new { orderID = orderID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteOrderDetail(int orderID, int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = "DELETE FROM OrderDetails WHERE OrderID = @orderID AND ProductID = @productID";
                var parameters = new
                {
                    orderID = orderID,
                    productID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }


        public bool DeleteOrderDetailByProductID(int productID)
        {
            throw new NotImplementedException();
        }

        public Order? GetById(int id)
        {
            Order data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  o.*,  
                                            c.CustomerName,
                                            c.ContactName as CustomerContactName,
                                            c.Address as CustomerAddress,
                                            c.Email as CustomerEmail,
                                             e.FullName as EmployeeFullName,
                                            s.ShipperName,
                                            s.Phone as ShipperPhone
                                    FROM    Orders as o
                                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                    WHERE   o.OrderID = @OrderID";
                var parameters = new { OrderID = id };
                data = connection.QueryFirstOrDefault<Order>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public OrderDetail GetOrderDetail(int orderID, int productID)
        {
            OrderDetail? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	od.*, p.ProductName, p.Unit, p.Photo		
                                    FROM	OrderDetails AS od
		                                    JOIN Products AS p ON od.ProductID = p.ProductID
                                    WHERE	od.OrderID = @orderID AND od.ProductID = @productID";
                var parameters = new
                {
                    orderID = orderID,
                    productID = productID
                };
                data = connection.QueryFirstOrDefault<OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public IList<Order> List(int page = 1, int pageSize = 0, string searchValue = "", int status = 0, int customerID = 0)
        {
            List<Order> data = new List<Order>();

            // Adjust the search value for SQL LIKE query
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"
                    WITH cte AS (
                        SELECT 
                            o.*, 
                            c.CustomerName, 
                            c.ContactName AS CustomerContactName,
                            c.Address AS CustomerAddress, 
                            c.Email AS CustomerEmail,
                            e.FullName AS EmployeeFullName, 
                            s.ShipperName, 
                            s.Phone AS ShipperPhone,
                            ROW_NUMBER() OVER(ORDER BY o.OrderID DESC) AS RowNumber
                        FROM Orders AS o
                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                        WHERE 
                            (@status = 0 OR o.Status = @status) AND
                            (@searchValue = N'' OR c.CustomerName LIKE @searchValue OR s.ShipperName LIKE @searchValue) AND
                            (@customerID = 0 OR o.CustomerID = @customerID)
                    )
                    SELECT * 
                    FROM cte
                    WHERE (@pageSize = 0) OR (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                    ORDER BY RowNumber";

                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    status = status,
                    searchValue = searchValue,
                    customerID = customerID
                };

                data = connection.Query<Order>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }

            if (data == null)
                data = new List<Order>();

            return data;
        }

        public IList<OrderDetail> ListOrderDetails(int orderID)
        {
            List<OrderDetail> data = new List<OrderDetail>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	od.*, p.ProductName, p.Unit, p.Photo		
                                    FROM	OrderDetails AS od
		                                    JOIN Products AS p ON od.ProductID = p.ProductID
                                    WHERE	od.OrderID = @orderID";
                var parameters = new
                {
                    orderID = orderID
                };
                data = (connection.Query<OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();

                connection.Close();
            }
            return data;
        }


        public bool Update(Order data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Orders
                                    SET     CustomerID = @customerID,
                                            OrderTime = @orderTime,
                                            EmployeeID = @employeeID,
                                            AcceptTime = @acceptTime,
                                            ShipperID = @shipperID,
                                            ShippedTime = @shippedTime,
                                            FinishedTime = @finishedTime,
                                            Status = @status
                                    WHERE   OrderID = @orderID";

                var parameters = new
                {
                    orderID = data.OrderID,
                    customerID = data.CustomerID,
                    orderTime = data.OrderTime,
                    employeeID = data.EmployeeID,
                    acceptTime = data.AcceptTime ?? null,
                    shipperID = data.ShipperID ?? null,
                    shippedTime = data.ShippedTime,
                    finishedTime = data.FinishedTime,
                    status = data.Status
                };

                result = connection.Execute(sql, parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }

        public int SaveOrderDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            int result = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if exists (select 1 from OrderDetails where OrderID = @orderID and ProductID = @productID)
                                    begin
                                        UPDATE OrderDetails
                                        SET Quantity = @quantity,
                                            SalePrice = @salePrice
                                        WHERE OrderID = @orderID AND ProductID = @productID;
                                    end
                                    else
                                    begin
                                        INSERT INTO OrderDetails (OrderID, ProductID, Quantity, SalePrice)
                                        VALUES (@orderID, @productID, @quantity, @salePrice);
                                    end;

                                    SELECT @@ROWCOUNT AS AffectedRows;";

                var parameters = new
                {
                    orderID = orderID,
                    productID = productID,
                    quantity = quantity,
                    salePrice = salePrice
                };

                result = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            }
            return result;
        }

        
    }
}
