using System.Configuration;
using System.Data.SQLite;
using System.Net.Http.Json;

namespace OrderApi.Orders
{
    internal class UpdatePaymentStatus
    {
        public async Task<bool> GetPaymentStatus(int orderId)
        {
            string url = ConfigurationManager.AppSettings["APIEndpoint"];
            string mockServerUrl = $"{url}/{orderId}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("accept", "application/json");

                HttpResponseMessage response = await client.GetAsync(mockServerUrl);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response content: {responseContent}");

                return responseContent.Trim('"') == "Paid";
            }
        }


        public async Task<Order> PaymentUpdate(SQLiteConnection connection)
        {
            try
            {
                connection.Open();
                Console.WriteLine("Enter the OrderId to update payment status: ");
                var orderId = int.Parse(Console.ReadLine());
                var orderIdString = orderId.ToString();
                OrderStatus newOrderStatus;
                
                var paymentStatus = new UpdatePaymentStatus();
                bool isPaid = await GetPaymentStatus(orderId);
                
                if (isPaid)
                    newOrderStatus = OrderStatus.Paid;
                else
                    newOrderStatus = OrderStatus.Cancelled;


                string query = "UPDATE Orders SET Status = @NewStatus WHERE OrderId = @OrderId";

                using (SQLiteCommand sqlQuery = new SQLiteCommand(query, connection))
                {
                    sqlQuery.Parameters.AddWithValue("@NewStatus", newOrderStatus.ToString());
                    sqlQuery.Parameters.AddWithValue("@OrderId", orderId);
                    orderId = Convert.ToInt32(sqlQuery.ExecuteScalar());
                }

                Console.WriteLine("Order status was changed successfully.");

                connection.Close();
                return UpdatedOrder(connection, orderIdString);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public Order UpdatedOrder(SQLiteConnection connection, string orderId)
        {
            try
            {

                connection.Open();
                var query = $"SELECT Orders.OrderId, Orders.CustomerName, Orders.DateCreated, Orders.Status, OrderItems.OrderItemId, OrderItems.ProductName, " +
                    $"OrderItems.Quantity, OrderItems.PricePerUnit, OrderItems.OrderId FROM Orders JOIN OrderItems ON Orders.OrderId = OrderItems.OrderId " +
                    $"WHERE Orders.OrderId = {orderId}";

                var updatedOrder = new Order();

                using (SQLiteCommand sqlQuery = new SQLiteCommand(query, connection))
                {

                    using (SQLiteDataReader reader = sqlQuery.ExecuteReader())
                    {
                        var updatedOrderItems = new List<OrderItems>();
                        while (reader.Read())
                        {
                            updatedOrder.OrderId = Convert.ToInt32(reader["OrderId"]);
                            updatedOrder.OrderCreationDate = Convert.ToDateTime(reader["DateCreated"]);
                            updatedOrder.CustomerName = reader["CustomerName"].ToString();
                            updatedOrder.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), reader["Status"].ToString());
                            updatedOrderItems.Add(new OrderItems()
                            {
                                ProductName = reader["ProductName"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                PricePerUnit = Convert.ToDecimal(reader["PricePerUnit"])
                            });
                        }

                        updatedOrder.OrderItems = updatedOrderItems;

                    }

                    connection.Close();

                    return updatedOrder;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}