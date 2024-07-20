using BenchmarkDotNet.Configs;
using CBSExchangeRatesUpdater.Service;
using Serilog;
using System.Globalization;
using OrderApi.Network;
using OrderApi.Contracts;
using System.IO;
namespace OrderApi
{
    public class TaskExecuter
    {

        public async Task ExecuteScheduledTask(string accessToken)
        {
            var uploadUrl = @"https://alza.orders.cz";
            var client = new SendAPI(uploadUrl);


            var items = new Order();
            items.OrderId = 1;
            items.OrderItems = new List<OrderItems>();


                            .Where(item => item != null)
                            .Select(item => new Currency()
                            {
                                //CurrencyName = item.CurrencyName,
                                CurrencyCode = ParseCurrencyCode(item.CurrencyName, currencyCodes),
                                Validity = item.Validity,//.ToString(),
                                Multiplier = item.Multiplier,
                                Rate = item.Rate
                            })
                            .Where(HasValidDateRange)
                            .Where(HasValidValue)
                            .Where(item => item.CurrencyCode != OrderStatus.Unknown)
                            .Where(item => iso == item.CurrencyCode)
                            .OrderByDescending(item => item.Validity)
                            .Take(config.numberOfRecordsForEachCurrency)
                            .ToArray();



            client.SetOrder(items, accessToken);

            NoRecordsFilter noRecordsFilter = new NoRecordsFilter();
            await noRecordsFilter.FilterNoRecordCurrencies(accessToken);
        }
    }
}

