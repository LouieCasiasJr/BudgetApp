using Azure;
using Azure.Communication.Email;
using BudgetApp.BLL;
using BudgetApp.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace BudgetApp.Functions
{
    public class MonthlySummary
    {
        private readonly ILogger _logger;
        private readonly ISpendingReportProvider _spendingReportProvider;

        public MonthlySummary(ILoggerFactory loggerFactory, ISpendingReportProvider spendingReportProvider)
        {
            _logger = loggerFactory.CreateLogger<MonthlySummary>();
            _spendingReportProvider = spendingReportProvider;
        }

        [Function("MonthlySummary")]
        public void Run([TimerTrigger("0 0 8 1 * *")] TimerInfo myTimer)
        {
            var connectionString = Environment.GetEnvironmentVariable("CommunicationServices__ConnectionString");
            var domain = Environment.GetEnvironmentVariable("CommunicationServices__Domain");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Communication Services connection string not found, skipping email send.");
                return;
            }

            _logger.LogInformation($"Monthly summary function started at: {DateTime.Now}");

            var reports = _spendingReportProvider.GetReportsToLastMonth(6);
            var aggregate = reports.FirstOrDefault();

            if (aggregate == null)
            {
                _logger.LogWarning("No spending report data available.");
                return;
            }

            var emailBody = BuildEmailBody(aggregate);
            var emailClient = new EmailClient(connectionString);

            var emailMessage = new EmailMessage(
                senderAddress: $"DoNotReply@{domain}",
                recipientAddress: "louiecasiasjr@gmail.com",
                content: new EmailContent($"Budget Summary — {aggregate.Period}")
                {
                    PlainText = emailBody
                }
            );

            emailClient.Send(WaitUntil.Completed, emailMessage);
            _logger.LogInformation("Monthly summary email sent successfully.");
        }

        private string BuildEmailBody(SpendingReportDTO report)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Budget Summary — {report.Period}");
            sb.AppendLine($"{"Category",-30} {"Budget",10} {"Actual",10} {"Delta",10}");
            sb.AppendLine(new string('-', 62));

            foreach (var delta in report.Deltas)
            {
                var actual = delta.Budget + delta.Delta;
                var status = delta.Delta > 0 ? "OVER" : "under";
                sb.AppendLine($"{delta.BucketLabel,-30} {delta.Budget,10:C} {actual,10:C} {delta.Delta,10:C} {status}");
            }

            sb.AppendLine(new string('-', 62));
            sb.AppendLine($"{"Total Budgeted:",-30} {report.Budgeted,10:C}");
            sb.AppendLine($"{"Income:",-30} {report.Income,10:C}");

            return sb.ToString();
        }
    }
}
