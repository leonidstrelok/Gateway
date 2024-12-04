#!/usr/bin/env pwsh

docker-compose -f docker-compose.db.yml -f docker-compose.db.override.yml up -d

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssw0rd1!" -p 1433:1433 --name mssql-container -d mcr.microsoft.com/mssql/server:2022-latest