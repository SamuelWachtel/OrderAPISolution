using System.Data.SQLite;

namespace OrderApi.Orders
{
    internal class CreateNewOrder
    {
        public Order NewOrder()
        {
            var orderItemsList = new List<OrderItems>();
           // var completeOrder = new Order();
            var nextItem = true;
            Console.WriteLine("Enter customer name: ");
            var customerName = Console.ReadLine();

            while (nextItem)
            {
                var orderItems = new OrderItems();

                Console.WriteLine("Enter product name: ");
                orderItems.ProductName = Console.ReadLine();

                Console.WriteLine("Enter quantity: ");
                orderItems.Quantity = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter price per unit: ");
                orderItems.PricePerUnit = Convert.ToDecimal(Console.ReadLine());
                orderItemsList.Add(orderItems);

                Console.WriteLine("Do you want to add another item? (Y/N)");
                var response = Console.ReadLine();
                if (response.ToUpper() == "N")
                    nextItem = false;
                else if (response.ToUpper() == "Y")
                    nextItem = true;
            }
            return (new Order
            {
                CustomerName = customerName,
                OrderCreationDate = DateTime.Now,
                Status = OrderStatus.New,
                OrderItems = orderItemsList
            }); ;
        }

        public void SaveNewOrder(SQLiteConnection connection, Order orderList)
        {
            try
            {
                connection.Open();

                string query = "INSERT INTO Orders (CustomerName, DateCreated, Status) VALUES (@CustomerName, @OrderCreationDate, @Status); SELECT last_insert_rowid();";
                int orderId;

                using (SQLiteCommand sqlQuery = new SQLiteCommand(query, connection))
                {
                    sqlQuery.Parameters.AddWithValue("@CustomerName", orderList.CustomerName);
                    sqlQuery.Parameters.AddWithValue("@OrderCreationDate", orderList.OrderCreationDate);
                    sqlQuery.Parameters.AddWithValue("@Status", orderList.Status.ToString());

                    orderId = Convert.ToInt32(sqlQuery.ExecuteScalar());
                }

                foreach (var order in orderList.OrderItems)
                {
                    string query2 = "INSERT INTO OrderItems (OrderId, ProductName, Quantity, PricePerUnit) VALUES (@OrderId, @ProductName, @Quantity, @PricePerUnit)";
                    using (SQLiteCommand sqlQuery = new SQLiteCommand(query2, connection))
                    {
                        sqlQuery.Parameters.AddWithValue("@ProductName", order.ProductName);
                        sqlQuery.Parameters.AddWithValue("@Quantity", order.Quantity);
                        sqlQuery.Parameters.AddWithValue("@PricePerUnit", order.PricePerUnit);
                        sqlQuery.Parameters.AddWithValue("@OrderId", orderId);
                        sqlQuery.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Order and order items created successfully.");
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