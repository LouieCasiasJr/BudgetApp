using BudgetApp.Shared;
using System.Net.Http.Json;

namespace BudgetApp.Client
{
    public class BudgetReportService
    {
        HttpClient _http;

        public BudgetReportService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ResultPacket<List<SpendingReportDTO>>> getReports(int months = 12)
        {
            var response = await _http.GetFromJsonAsync<ResultPacket<List<SpendingReportDTO>>>($"SpendingReport/{months}");
            return response;
        }

    }
}
