using OrderApi.Network;
using OrderApi.Orders;
using System.Configuration;
using System.Data.SQLite;

namespace OrderApi
{
    class Program
    {
        public static async Task Main()
        {
            var connectionString = ConfigurationManager.AppSettings["databaseConnectionString"];
            var continueString = true;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                while (continueString)
                {
                    Console.WriteLine("Options: \n1) New order\n2) List orders\n3) Payment update\nEnter your choice: ");
                    

                    string choice = Console.ReadLine();
                    if (!int.TryParse(choice, out int parsedChoice))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        continue;
                    }

                    if (parsedChoice < 1 || parsedChoice > 3)
                    {
                        Console.WriteLine("Invalid choice. Please enter a valid number between 1 and 3.");
                        continue;
                    }
                   
                    SendAPI sendAPI = new SendAPI();
                    switch (parsedChoice)
                    {
                        case 1:
                            var newOrder = new CreateNewOrder();
                            var orderList = newOrder.NewOrder();
                            newOrder.SaveNewOrder(connection, orderList);
                            await sendAPI.SetOrder(orderList);
                            break;
                        case 2:
                            var listOrders = new ListAllOrders();
                            listOrders.ListOrders(connection);
                            break;
                        case 3:
                            var paymentUpdate = new UpdatePaymentStatus();
                            Order updatedOrder = await paymentUpdate.PaymentUpdate(connection);
                            await sendAPI.SetOrder(updatedOrder);
                            break;
                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }
                    Console.WriteLine("Do you want to continue? (Y/N)");
                    var response = Console.ReadLine();
                    while (response.ToUpper() != "N" && response.ToUpper() != "Y")
                    {
                        Console.WriteLine("Invalid input. Please enter Y or N.");
                        response = Console.ReadLine();
                    }
                    if (response.ToUpper() == "N")
                        continueString = false;
                    else if (response.ToUpper() == "Y")
                        continueString = true;
                    else { }
                }
            }
        }
    }
}
