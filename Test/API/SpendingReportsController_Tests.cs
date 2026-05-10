using BudgetApp.BLL;
using BudgetApp.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Test
{
    [TestFixture]
    [Category("API")]
    public class SpendingReportControllerIntegrationTests
    {
        private Mock<ISpendingReportProvider> _mockProvider;
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        private readonly List<SpendingReportDTO> _sampleReports = new()
        {
            new SpendingReportDTO { Period = "January 2025", Budgeted = 3000, Income = 5000 },
            new SpendingReportDTO { Period = "February 2025", Budgeted = 3000, Income = 5000 },
        };

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockProvider = new Mock<ISpendingReportProvider>();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(ISpendingReportProvider));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddSingleton(_mockProvider.Object);
                    });
                });

            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _mockProvider.Reset();
        }

        [Test]
        public async Task Get_WhenProviderReturnsData_Returns200()
        {
            _mockProvider
                .Setup(p => p.GetReportsToLastMonth(It.IsAny<int>()))
                .Returns(_sampleReports);

            var response = await _client.GetAsync("/SpendingReport/12");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Get_WhenProviderReturnsData_ResponseDeserializesCorrectly()
        {
            _mockProvider
                .Setup(p => p.GetReportsToLastMonth(It.IsAny<int>()))
                .Returns(_sampleReports);

            var result = await _client
                .GetFromJsonAsync<ResultPacket<List<SpendingReportDTO>>>("/SpendingReport/12");

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data, Has.Count.EqualTo(2));
                Assert.That(result.Data[0].Period, Is.EqualTo("January 2025"));
            });
        }

        [Test]
        public async Task Get_WhenProviderReturnsEmpty_Returns200WithFailurePacket()
        {
            // Note: the controller returns 200 even on failure - it uses
            // IsSuccess in the packet rather than HTTP status codes for this.
            // This test documents that design decision.
            _mockProvider
                .Setup(p => p.GetReportsToLastMonth(It.IsAny<int>()))
                .Returns(new List<SpendingReportDTO>());

            var result = await _client
                .GetFromJsonAsync<ResultPacket<List<SpendingReportDTO>>>("/SpendingReport/12");

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Message, Is.EqualTo("Failed to return spending reports"));
            });
        }

        [Test]
        public async Task Get_RoutePassesMonthsParameterCorrectly()
        {
            _mockProvider
                .Setup(p => p.GetReportsToLastMonth(6))
                .Returns(_sampleReports);

            await _client.GetAsync("/SpendingReport/6");

            _mockProvider.Verify(p => p.GetReportsToLastMonth(6), Times.Once);
        }

        [Test]
        public async Task Get_WithNonNumericMonths_Returns400()
        {
            var response = await _client.GetAsync("/SpendingReport/abc");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Get_WithNoMonthsSegment_ReturnsIndex()
        {
            var response = await _client.GetAsync("/SpendingReport");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content, Does.Contain("<div id=\"app\">Loading...</div>"));
            });
        }
    }
}