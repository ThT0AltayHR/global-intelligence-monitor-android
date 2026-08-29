FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /build

COPY . .

RUN dotnet restore
RUN dotnet build -c Release

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=publish /app .

EXPOSE 5000

ENTRYPOINT ["dotnet", "GlobalIntelligenceMonitor.dll"]
