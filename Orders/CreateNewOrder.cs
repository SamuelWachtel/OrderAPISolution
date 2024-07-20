namespace OrderApi.Orders
{
    internal class CreateNewOrder
    {
        public Order NewOrder()
        {
            Console.WriteLine("Enter customer name: ");
            var customerName = Console.ReadLine();

            OrderItems order = new OrderItems();

            Console.WriteLine("Enter product name: ");
            order.ProductName = Console.ReadLine();

            Console.WriteLine("Enter quantity: ");
            order.Quantity = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter price per unit: ");
            order.PricePerUnit = Convert.ToDecimal(Console.ReadLine());

            return new Order
            {
                OrderId = 1,
                CustomerName = customerName,
                OrderCreationDate = DateTime.Now,
                Status = OrderStatus.New,
                OrderItems = new List<OrderItems> { order }
            };
        }

        public void SaveNewOrder(SqlConnection connection, Order Order)
        {
            var order = Order;
            var x = order.OrderItems;

            try
            {
                using (connection)
                {
                    string query = "INSERT INTO Order (CustomerName, OrderCreationDate, Status) VALUES (@CustomerName, @OrderCreationDate, @Status)";
                    using (SqlCommand sqlQuery = new SqlCommand(query))
                    {
                        sqlQuery.Connection = connection;
                        Console.WriteLine("Enter note title:");
                        string title = Console.ReadLine();
                        Console.WriteLine("Enter note content:");
                        string content = Console.ReadLine();

                        sqlQuery.Parameters.Add("@CustomerName", System.Data.SqlDbType.VarChar, 30).Value = order.CustomerName;
                        sqlQuery.Parameters.Add("@OrderCreationDate", System.Data.SqlDbType.VarChar, 30).Value = order.OrderCreationDate;
                        sqlQuery.Parameters.Add("@Status", System.Data.SqlDbType.VarChar, 30).Value = order.Status;
                        connection.Open();
                        sqlQuery.ExecuteNonQuery();
                        connection.Close();
                    }

                    string query2 = "INSERT INTO OrderItems (ProductName, Quantity, PricePerUnit) VALUES (@ProductName, @Quantity, @PricePerUnit)";
                    using (SqlCommand sqlQuery = new SqlCommand(query2))
                    {

                        sqlQuery.Parameters.Add("@ProductName", System.Data.SqlDbType.VarChar, 30).Value = x[0].ProductName;
                        sqlQuery.Parameters.Add("@Quantity", System.Data.SqlDbType.VarChar, 30).Value = x[0].Quantity;
                        sqlQuery.Parameters.Add("@PricePerUnit", System.Data.SqlDbType.VarChar, 30).Value = x[0].PricePerUnit;
                        connection.Open();
                        sqlQuery.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}




