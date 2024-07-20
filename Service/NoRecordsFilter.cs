using Application.Service;
using Serilog;

namespace OrderApi
{
    public class NoRecordsFilter
    {

        public List<string> validCurrencies = new List<string>();
        public List<string> noRecordCurrencies = new List<string>();

        public async Task FilterNoRecordCurrencies(string accessToken)
        {
            var recentFile = FileHelper.GetMostRecentFile();
            try
            {
                using (var streamCodes = File.OpenRead(config.currencyNameAndISOPath))
                using (var stream = File.OpenRead(recentFile))
                {
                      var allCurrencyCodes = new CurrencyParser().ParseCurrencyConfiguration(streamCodes);
            
                    var allRecordItems = new ExcelParser().ParseExchangeRates(File.OpenRead(recentFile))
                        .Where(item => item != null)
                        .Select(item => new Currency()
                        {
                            CurrencyName = item.CurrencyName,
                            CurrencyCode = RecordsFilter.ParseCurrencyCode(item.CurrencyName, allCurrencyCodes),
                        })
                        .ToArray();
             

                    if (config.requestedCurrencyISOCode == "")
                    {
                        validCurrencies = allRecordItems.Select(item => item.CurrencyCode.ToString()).ToList();
                    
                    }
                    else
                    {
                        validCurrencies = config.validCurrencies;
                        if (config.invalidCurrencies.Count() > 0)

                        noRecordCurrencies.AddRange(config.validCurrencies.ToList());
                        foreach (var item in allRecordItems)
                        {
                            noRecordCurrencies.Remove(item.CurrencyCode.ToString());
                        }
                    }

                    if (config.validCurrencies.ToList().Count() > 0)
                    {
                        if (noRecordCurrencies.Count() > 0)
                            Log.Warning($"[Validation] Valid currencies with no records ({noRecordCurrencies.Count()}): {string.Join(", ", noRecordCurrencies)}");
                        Log.Information($"[Validation] Showing records for currencies ({validCurrencies.Except(config.invalidCurrencies).Except(noRecordCurrencies).Count()}): " +
                            $"{string.Join(", ", validCurrencies.Except(config.invalidCurrencies).Except(noRecordCurrencies))}");
                        
                    }
                    else
                        Log.Information($"[Validation] All currencies have records.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[ERROR] Filtering no record currencies unsuccessfull: {ex.Message}");
            }
            RecordsFilter recordsFilter = new RecordsFilter();
            await recordsFilter.FilterCurrenciesWithRecordsAsync(accessToken, validCurrencies, noRecordCurrencies);
            return;
        }
    }
}
