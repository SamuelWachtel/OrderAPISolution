using System.Net.Http.Json;
using OrderApi.Orders;

namespace OrderApi.Network
{
    public class SendAPI
    {
        readonly string APIEndpointURL;

        public SendAPI(string CBSAPIUrl)
        {
            this.APIEndpointURL = CBSAPIUrl;
        }
        public async Task SetOrder(IEnumerable<Order> order, string accessToken)
        {
            var apiOrder = order
                .Select(c => new APICurrency
                {
                    OrderId = c.OrderId.ToString(),
                    CustomerName = c.CustomerName,
                    OrderCreationDate = c.OrderCreationDate,
                    OrderStatus = c.Status.ToString(),
                    OrderItems = new OrderItems
                    {
                        ProductName = c.OrderItems.ProductName,
                        Quantity = c.OrderItems.Quantity,
                        PricePerUnit = c.OrderItems.PricePerUnit
                    }
                }).ToArray();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("accept", "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Put, APIEndpointURL))
                using (var json = JsonContent.Create(new { data = apiOrder }))
                {
                    request.Content = json;

                    request.Headers.Add("Authorization", "Bearer " + accessToken);

                    var result = await client.SendAsync(request);
                    
                    var response = await result.Content.ReadAsStringAsync();
                }
            }
        }
    }

    public class APICurrency
    {

        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderCreationDate { get; set; }
        public string OrderStatus { get; set; }

        public OrderItems OrderItems { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}