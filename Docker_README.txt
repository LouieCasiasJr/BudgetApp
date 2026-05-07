Referenced commands intended for Powershell in root
Install or verify installation of Docker Desktop (docker --version)
create a .env file in solution root:
	Must include a variable "SA_PASSWORD" - value must have Capital and lowercase letters, numeric digit, and a special character.
dockerfile:
	Running .Net 8 - mcr.microsoft.com/dotnet/sdk:8.0
	Exposed on port 8080 (localhost:8080)
	Runs from "Server" directory
docker-compose.yml:
	Running MSSQL 2022 - mcr.microsoft.com/mssql/server:2022-latest
	DB Exposed on port 1434 from local, 1433 within the container.
	Persistent "sqldata" volume at /var/opt/mssql/data (can restore here)
	DB HealthCheck - starts after 20 seconds, retries every 10, up to 5 times.
	API start is dependent on DB passing the health check.
dockerignore 
	Keeps docker from copying unnecessary files with the build

To build after code changes (or initial download): 
	Build the images without caching - docker compose build --no-cache
		Should see no errors - "Image budgetapp-api Built"
To Start:
	Start detached (keeps terminal free for commands) - docker compose up -d
		OR
	Combined with build if there may be changes - docker-compose up --build -d
	Results:
		Should see the build successful
		2 containers built (DB and API), 1 Network created (For communication between containers); DB container shows "Healthy"

To Shut down:
	Shut down only - docker compose down
		2 containers removed, 1 Network removed
	Shut down AND clear persistent "sqldata" volume - docker compose down -v
		2 containers removed, 1 Network removed, 1 SQL volume removed
	
Restoration of containerized DB from Backup:
	App and DB must be running detached - docker compose up -d
	Push DB backup to container:
		docker cp [Full location of '.bak' file on your machine] budgetapp-db-1:/var/backups/BudgetDB.bak
		Message confirms successful copy to destination
	Restore containerized DB from backup: 
		Run the script in ./restore-db.ps1 - if you copy/paste, enter the appropriate password
		Message confirms restore successful
		
