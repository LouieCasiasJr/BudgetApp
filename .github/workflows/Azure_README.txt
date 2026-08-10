CICD_README details pipeline setup. This document is intended to describe the Azure services in use. 

Currently services have been manually configured, but future work will define these services in code.
Due to limitations in Azure, some services are provisioned in West Europe, while others are in North Europe.

Azure service layout:
	Azure Container Registry (ACR) holds the images deployed by our CICD pipeline (tagged "latest" and with specific SHA)
		Service principal is assigned "Container Registry Repository Writer" (ABAC) role on the ACR, this ensures that the 
			pipeline can push to the container registry
		Service principal is assigned the role "Container Apps Contributor" on the resource group, ensuring that
			the pipeline can update the Container Apps to point to a new image
		The Container App managed identity is assigned "Container Registry Repository Reader" (ABAC) role on the ACR, 
			allowing the Container App to pull images from the registry
	Container App (inside of a container app environment) runs the images held in the ACR and exposes them as
		an available application at a url address
	Function App runs the functions held in BudgetApp.Functions
		Service principal is assigned "Website Contributor" role on the resource group to allow Function App deploy
		The function app requires an Azure Storage Account
	Database runs in Azure SQL Server
	Logs and metrics are connected to Azure Application Insights 
	Connection strings for Azure SQL DB and Application Insights are held in Azure Key Vault, consumed by 
		Container (both) and Function (SQL Only) Apps (secrets reference Key Vault Values, Environment Variables referencing those secrets)

Azure Key Vault:
	Any user, including the owner, must have "Key Vault Secrets Officer" in order to edit secrets within the Key Vault
	Container and Function App managed identities must have the role "Key Vault Secrets User" in order to read from the vault.
Managed Identity:
    The Container App uses system-assigned Managed Identity to authenticate to both ACR and Key Vault without stored credentials.
	The Function App uses system-assigned Managed Identity to authenticate to Key Vault without stored credentials.

Application HTTP logs can be accessed through application insights
	Controllers are setup to return 200 with an error message on failure - manual log entries are also reflected 
		in Application Insights
	HTTP logs can be drilled down to show DB level calls
