using System.Data.SQLite;

namespace OrderApi.Orders
{
    internal class ListAllOrders
    {
        public void ListOrders(SQLiteConnection connection)
        {
            try
            {

                string query = @"
                   SELECT Orders.OrderId, Orders.CustomerName, Orders.DateCreated, Orders.Status, OrderItems.OrderItemId, OrderItems.ProductName, OrderItems.Quantity, 
                    OrderItems.PricePerUnit, OrderItems.OrderId FROM Orders JOIN OrderItems ON Orders.OrderId = OrderItems.OrderId";

                using (SQLiteCommand sqlQuery = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    using (SQLiteDataReader reader = sqlQuery.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            Console.WriteLine($"OrderId: {reader["OrderId"]}" +
                                $"\nCustomer name: {reader["CustomerName"]}" +
                                $"\nCreation date: {reader["DateCreated"]}" +
                                $"\nOrder status: {reader["Status"]}" +
                                $"\nOrder item id: {reader["OrderItemId"]}" +
                                $"\nProduct name: {reader["ProductName"]}" +
                                $"\nQuantity: {reader["Quantity"]}" +
                                $"\nPrice per unit: {reader["PricePerUnit"]}" +
                                $"\nFK: {reader["OrderId"]}");
                            Console.WriteLine("\n--------------------------------------------------\n");
                        }
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
