using System.Configuration;
using System.Net.Http.Json;
using OrderApi.Orders;

namespace OrderApi.Network
{
    public class SendAPI
    {
        readonly string APIEndpointURL = ConfigurationManager.AppSettings["APIEndpoint"];

        public async Task SetOrder(Order order)
        {
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("accept", "application/json");

                using (var request = new HttpRequestMessage(HttpMethod.Put, APIEndpointURL))
                using (var json = JsonContent.Create(new { data = order }))
                {
                    request.Content = json;

                    var result = await client.SendAsync(request);
                    
                    var response = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"Status code: {result.StatusCode}");
                    Console.WriteLine(response);
                }
            }
        }
    }
}
