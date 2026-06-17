FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY apps/ApiGateway/src/ApiGateway/ApiGateway.csproj apps/ApiGateway/src/ApiGateway/
COPY apps/Shared/src/Shared.Database/Shared.Database.csproj apps/Shared/src/Shared.Database/
COPY apps/Shared/src/Shared.Hosting/Shared.Hosting.csproj apps/Shared/src/Shared.Hosting/
RUN dotnet restore apps/ApiGateway/src/ApiGateway/ApiGateway.csproj

COPY apps/ApiGateway/src/ apps/ApiGateway/src/
COPY apps/Shared/src/ apps/Shared/src/
RUN dotnet publish apps/ApiGateway/src/ApiGateway/ApiGateway.csproj -c Release -o /app/publish

COPY docs/contracts /app/publish/contracts

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
