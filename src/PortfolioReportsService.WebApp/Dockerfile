FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM stenn.azurecr.io/stenn-dotnet-sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "PortfolioReportsService.WebApp/PortfolioReportsService.WebApp.csproj"
RUN dotnet build "PortfolioReportsService.WebApp/PortfolioReportsService.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PortfolioReportsService.WebApp/PortfolioReportsService.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PortfolioReportsService.WebApp.dll"]
