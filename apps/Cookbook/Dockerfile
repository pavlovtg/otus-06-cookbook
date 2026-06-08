FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Recipes/Recipes.csproj src/Recipes/
RUN dotnet restore src/Recipes/Recipes.csproj

COPY src/Recipes/ src/Recipes/
RUN dotnet publish src/Recipes/Recipes.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Recipes.dll"]
