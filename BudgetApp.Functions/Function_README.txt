MonthlySummary Function:
	Triggered at 8AM on the first of every month to send an email.
		Uses a TimerTrigger
		Email includes an aggregate summary of the past 6 months of data, showing buckets that are over or under budget.
	Directly hits the BLL SpendingReportProvider, depending also on the DAL and DTO (shared) projects.
	Email send is handled through connection with Azure Communication Services - handled through global config variables
		in local.settings.json, and also stored in the Key Vault, referenced in Azure Function App as Environment Variables. 
	Logs warnings if unable to connect to Communication services, or if no data is found.

Future plan: 
	Automated ingestion per transaction prompting the user for categorization immediately. This would likely use TrueLayer or Plaid	to
		catch and present notifications to the user as new transactions are identified. This would not change the need for the existing 
		monthly report TimerTrigger, but may alter the timing and necessitate an earlier trigger prompting the user to complete 
		any uncategorized transactions.

ADR: Azure Function
	Context: A monthly spending summary notification needs to fire on the first of each month.
	Decision: Azure Function with a timer trigger.
	Reasons:
		The job takes a few seconds, once a month, and we only pay for the seconds it runs. 
			A full app would be a resource waste.
		No HTTP, only background work with no incoming traffic.
	Trade-offs:
		Cold start latency, but acceptable for a scheduled background job.
		Adds a second compute model to maintain alongside Container Apps.
	Alternatives considered:
		Timer-triggered Container App job: more overhead, no benefit for this workload.
		Running the job inside the main Container App on a schedule: couples background 
			work to the web app, harder to scale and maintain independently.
