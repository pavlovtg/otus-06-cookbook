FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ApiGateway/src/ApiGateway/ApiGateway.csproj ApiGateway/src/ApiGateway/
COPY Shared/src/Shared.Database/Shared.Database.csproj Shared/src/Shared.Database/
COPY Shared/src/Shared.Hosting/Shared.Hosting.csproj Shared/src/Shared.Hosting/
RUN dotnet restore ApiGateway/src/ApiGateway/ApiGateway.csproj

COPY ApiGateway/src/ ApiGateway/src/
COPY Shared/src/ Shared/src/
RUN dotnet publish ApiGateway/src/ApiGateway/ApiGateway.csproj -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
