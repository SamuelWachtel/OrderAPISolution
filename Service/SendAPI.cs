using Application.Service;
using OrderApi.Contracts;
using OrderApi;
using Serilog;
using System.Globalization;


namespace OrderApi
{
    internal class RecordsFilter
    {
        public async Task FilterCurrenciesWithRecordsAsync(string accessToken, List<string> validCurrencies, List<string> noRecordCurrencies)
        {
            var noRecordsFilter = new NoRecordsFilter();
            int index = 1;
            Log.Logger.LogLine();
            var client = new CBSClient(config.uploadUrl);
            client.NumberOfFailedRequests += noRecordCurrencies.Count;
            client.NumberOfFailedRequests += config.invalidCurrencies.Count();
            foreach (var requestedCurrency in validCurrencies.Except(noRecordCurrencies))
            {
                var iso = Enum.Parse<OrderStatus>(requestedCurrency);
                try
                {
                    
                    using (var streamCodes = File.OpenRead(config.currencyNameAndISOPath))
                    using (var stream = File.OpenRead(FileHelper.GetMostRecentFile()))
                    {
                        var currencyCodes = new CurrencyParser().ParseCurrencyConfiguration(streamCodes);

                        var items = new ExcelParser()
                            .ParseExchangeRates(stream)
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
                            
                        
                        if (items.Length == 0)
                        {
                            Log.Warning($"[Result #{index}] Currency: {iso} - The results don’t fit the right date or value range.");
                            index++;
                            client.NumberOfFailedRequests++;
                            Log.Logger.LogLine();

                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                try
                                {
                                    Log.Information($"[Result #{index}] Currency: {item.CurrencyCode}, Rate: {item.Rate}, Multiplier: {item.Multiplier}, Validity: {item.Validity}");//.ToString("dd/MM/yyyy")}");
                                    //Log.Information($"[Update] Updated by {item.LastUpdate.Username} (user Id: {item.LastUpdate.UserId}) at: {item.LastUpdate.Date}");
                                    index++;
                                    Log.Logger.LogLine();
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, $"Error displaying currency: {item.CurrencyCode} - {ex.Message}");
                                }

                                Log.Information($"[API Upload] Sending record to CBS API");
                                await client.SetCurrencies(items, accessToken);
                            } 
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error executing scheduled task: {ErrorMessage}", ex.Message);
                }
            };
            TextInfo textInfo = new CultureInfo(config.encoding).TextInfo;

            Log.Information($"[API Upload] Task completed.");
            Log.Information($"[Info] Successful updates: {client.NumberOfSuccessfulRequests}");
            Log.Information($"[Info] Failed updates: {client.NumberOfFailedRequests}");
            Log.Logger.LogLineSpace();
        }
        public static OrderStatus ParseCurrencyCode(string currencyCode, IEnumerable<Order> currencyCodes)
        {
            var currency = currencyCodes.FirstOrDefault(x => string.Equals(x.Name, currencyCode, StringComparison.OrdinalIgnoreCase));
            return currency != default ? Enum.Parse<OrderStatus>(currency.Code) : OrderStatus.Unknown;
        }

        public bool HasValidDateRange(Currency currency)
        {
            if (currency == null)
            {
                Log.Error($"Currency is null {currency}");
                Log.Error($"type {currency.GetType()}");
                throw new ArgumentNullException(nameof(currency));
            }
            else
                return DateTime.Parse(currency.Validity) >= DateTime.Now.AddYears(-config.selectCurrenciesfromYear) && DateTime.Parse(currency.Validity) <= DateTime.Now.AddYears(-config.selectCurrenciestoYear);
        }
        public bool HasValidValue(Currency currency)
            => currency.Rate >= config.currencyValueMin && currency.Rate < config.currencyValueMax;

    }
}

