using OrderApi.Orders;

namespace OrderApi
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Options: \n 1)New order\n2)List orders\n3)Payment update\nEnter your choice: ");
            int choice = Convert.ToInt32(Console.ReadLine());


            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = @"(localDB)\MSSQLSERVER02";
            csb.InitialCatalog = "Secretary";
            csb.IntegratedSecurity = true;
            var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\samuel.wachtel\source\repos\Secretar.io\Secretare\SecretaryDB.mdf;Integrated Security=True";
            var continueString = true;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {


                switch (choice)
                {
                    case 1:
                        var newOrder = new CreateNewOrder();
                        newOrder.SaveNewOrder(connection , newOrder.NewOrder());
                        break;
                    case 2:
                        var listOrders = new ListAllOrders();
                        listOrders.ListOrders(connection);
                        break;
                    case 3:
                        var paymentUpdate = new UpdatePaymentStatus();
                        paymentUpdate.PaymentUpdate(connection);
                        break;
                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
            SecurityToken securityToken = new SecurityToken();
            string accessToken = await securityToken.GetAccessToken();
            TaskExecuter taskExecuter = new TaskExecuter();
            await taskExecuter.ExecuteScheduledTask(accessToken);
        }
    }
}
