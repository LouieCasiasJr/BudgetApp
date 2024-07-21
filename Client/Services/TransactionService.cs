using BudgetApp.Shared;
using System.Collections.Specialized;
using System.Net.Http.Json;

namespace BudgetApp.Client
{
    public class TransactionService
    {
        HttpClient _http;

        public TransactionService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ResultPacket<List<TransactionDisplayDTO>>> getAllTransactionDisplays(string from, string to, string bucket)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

            queryString.Add("from", from);
            queryString.Add("to", to);
            if (bucket != null)
                queryString.Add("bucket", bucket);

            string par = queryString.ToString();

            var response = await _http.GetFromJsonAsync<ResultPacket<List<TransactionDisplayDTO>>>($"Transaction/AllDisplay?{par}");
            return response;
        }

    }
}
