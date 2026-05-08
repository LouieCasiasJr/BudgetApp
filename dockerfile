FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BudgetApp.sln ./
COPY BudgetApp.BLL/BudgetApp.BLL.csproj BudgetApp.BLL/
COPY BudgetApp.DAL/BudgetApp.DAL.csproj BudgetApp.DAL/
COPY Client/BudgetApp.Client.csproj Client/
COPY Server/BudgetApp.Server.csproj Server/
COPY Shared/BudgetApp.Shared.csproj Shared/
COPY Test/BudgetApp.Test.csproj Test/

RUN dotnet restore

COPY . .
RUN dotnet publish Server/BudgetApp.Server.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "BudgetApp.Server.dll"]
