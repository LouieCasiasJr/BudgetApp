Build pipeline
	Runs on every branch push. 
	Restores, builds, tests, and validates the Docker image. A failing build blocks merge to main.
	build.yml:
		Checks out the repo
		Checks GH cache for nuget packages
		Sets up .Net v8.0.x
		Installs nuget packages (from cache if found)
		Runs the build with Release config - must build successfully or merge is blocked
		Runs tests (without building again) from BudgetApp.Test - all must pass or merge will be blocked
		Builds Docker image - "budget-app"

Deploy pipeline: 
	Runs on merge to main (with environment approval). 
	Pushes two tags to Azure Container Registry: `latest` and the commit SHA.
	deploy.yml:
		Checks out the repo
		Logs into Azure CLI (for subscription-level operations) with GH secret AZURE CREDENTIALS
		Authenticates Docker to Azure Container Registry (Docker protocol) with separated values
		Build and push the image to container registry, tagged with "Latest" and the SHA

Github secrets:
	AZURE_CREDENTIALS - Full JSON Azure login block
	ACR_LOGIN_SERVER - Registry URL (budgetappacr.azurecr.io), used by docker/login
	ACR_PASSWORD - Service principal client secret, used by docker/login
	ACR_USERNAME  - Service principal client ID, used by docker/login

Container Registry at `budgetappacr.azurecr.io/budget-app:<sha>`
