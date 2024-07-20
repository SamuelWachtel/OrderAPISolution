namespace OrderApi.Orders
{
    internal class ListAllOrders
    {
        public static void ListOrders(SqlConnection connection)
        {
            try
            {
                string query = "SELECT * FROM Orders JOIN OrderItems ON Orders.OrderId = OrderItems.OrderId";

                connection.Open();
                using (SqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"OrderId: {reader["OrderId"]}, Customer name: {reader["CustomerName"]}, Creation date: {reader["OrderCreationDate"]}, Order status: {reader["Status"]}, Order items: {reader["ProductName"]} - {reader["Quantity"]} - {reader["PricePerUnit"]}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
