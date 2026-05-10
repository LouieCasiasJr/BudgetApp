Test project covers unit and integration tests to be picked up for CI/CD
Following testing pyramid method - effort is made to test functions at the lowest level, with the tightest scope possible
A separate "E2E" project will be created later to cover End to End tests written with Playwright

Tests use NUnit for assertions, Moq for layer mocking, and WebApplicationFactory for http testing

Tests can be run with PowerShell command: dotnet test

BLL Unit Tests:
	A separate file containing a single test class for each Provider in BudgetApp.BLL
	Each Class is marked as 'Test Fixture' and 'Category("BLL")'
	Uses [OneTimeSetUp] - performed once before all tests
		Repository (DAL) is mocked using Moq
		Mapper is mocked using Moq - calls .Setup to configure mapping
		Provider is instantiated with mocked dependencies (Repo and Mapper)
	Uses [Setup] to reset mocked repo - each test must setup the repo mock for any underlying 
		method hit within the provider test EG: "GetWhere"
		_mockRepo
            .Setup(r => r.GetWhere(It.IsAny<Expression<Func<SpendingBucket, bool>>>()))
            .Returns((Expression<Func<SpendingBucket, bool>> predicate) =>
                _buckets.Where(predicate.Compile()));
			If the mocked method takes an expression predicate (such as ""GetWhere(i => i.BucketId == id)"), 
				that predicate must be compiled in the result
	By mocking the Repo and Mapper, we test the logic of the provider only - most provider methods are currently 
		thin, and may hold little benefit to testing at this level in this way.


API Integration tests:
	A separate file containing a single test class for each Controller in BudgetApp.Server
	Each Class is marked as 'Test Fixture' and 'Category("API")'
	Uses [OneTimeSetUp] - performed once before all tests
		Provider (BLL) interface is mocked using Moq
		_factory: A WebApplicationFactory with the necessary provider services replaced with the Mocked provider
	Uses [Setup] to reset mocked provider - any test that hits an active endpoint must setup the provider Mock 
		for any provider method called behind that endpoint
	Uses [OneTimeTearDown] to dispose of factory and client
	By mocking the provider, we test the functions of the controller only - receipt at the endpoint and return of ResultPacket
