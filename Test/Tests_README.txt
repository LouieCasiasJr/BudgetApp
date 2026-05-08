Test project covers unit tests to be picked up for CI/CD
A separate "E2E" project will be created later to cover End to End tests written with Playwright

Tests here use NUnit for assertions, Moq for provider mocking, and WebApplicationFactory for http testing

Tests can be run with PowerShell command: dotnet test

BLL Unit Tests:
A separate file containing a single test class for each provider in BudgetApp.BLL
Each Class is marked as 'Test Fixture' and 'Category("BLL")'


API Integration tests:
A separate file containing a single test class for each Controller in the BLL
Each Class is marked as 'Test Fixture' and 'Category("API")'
