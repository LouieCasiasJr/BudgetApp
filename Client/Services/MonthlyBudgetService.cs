using BudgetApp.Shared;
using System.Net.Http.Json;

namespace BudgetApp.Client
{
    public class MonthlyBudgetService
    {
        HttpClient _http;

        public MonthlyBudgetService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ResultPacket<List<MonthlyBudgetDTO>>> getActiveBudgets()
        {
            var response = await _http.GetFromJsonAsync<ResultPacket<List<MonthlyBudgetDTO>>>($"MonthlyBudgets");
            return response;
        }
    }
}
