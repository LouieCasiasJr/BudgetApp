Referenced commands intended for Powershell in root, denoted with 'ps: '
Install or verify installation of Docker Desktop v29+ (docker --version)
.Net SDK is not required locally as it builds in the container
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


On first run, using references below:
	Build the images, this will take significant time
	Start detached
	Populate the containerized DB from backup
	Access the app at Localhost:8080
	Shut down when finished.


To Build after code changes (or initial download): 
	Build the images without caching - ps: docker compose build --no-cache
		Should see no errors - "Image budgetapp-api Built"

To Start:
	Start detached (keeps terminal free for commands) - ps: docker compose up -d
		OR
	Combined with build if there may be changes - ps: docker compose up --build -d
	Results:
		Should see the build successful (if included)
		2 containers started (DB and API), 1 Network created (For communication between containers);
			DB container shows "Healthy"
		May also create 1 DB Volume if not already existent
		Able to access and run the app at localhost:8080
		Health endpoint available at localhost:8080/health — returns "Healthy" when app and database are connected

Populate containerized DB from Backup on first run or after 'compose down -v':
	App and DB must be running detached - ps: docker compose up -d
	Push DB backup to container:
		ps: docker cp [Full location of '.bak' file on your machine] budgetapp-db-1:/var/backups/BudgetDB.bak
		Message confirms successful copy to destination
	Restore containerized DB from backup: 
		ps: Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
		ps: ./restore-db.ps1
		Message confirms restore successful

To Shut down:
	Shut down only - ps: docker compose down
		2 containers removed, 1 Network removed
		SQL volume intact
	Shut down AND clear persistent "sqldata" volume - ps: docker compose down -v
		2 containers removed, 1 Network removed, 1 SQL volume removed
		You must follow "Populate containerized DB" above to restore the DB	
