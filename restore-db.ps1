$env:SA_PASSWORD = ((Get-Content .env | Where-Object { $_ -match "SA_PASSWORD" }) -split "=")[1]

docker exec -i budgetapp-db-1 /opt/mssql-tools18/bin/sqlcmd `
  -S localhost `
  -U sa `
  -P "$env:SA_PASSWORD" `
  -No `
  -Q "RESTORE DATABASE Budget FROM DISK='/var/backups/BudgetDB.bak' WITH MOVE 'Budget' TO '/var/opt/mssql/data/Budget.mdf', MOVE 'Budget_log' TO '/var/opt/mssql/data/Budget_log.ldf', REPLACE"