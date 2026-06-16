FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Cookbook/src/Recipes/Recipes.csproj Cookbook/src/Recipes/
COPY Shared/src/Shared.Database/Shared.Database.csproj Shared/src/Shared.Database/
COPY Shared/src/Shared.Hosting/Shared.Hosting.csproj Shared/src/Shared.Hosting/
RUN dotnet restore Cookbook/src/Recipes/Recipes.csproj

COPY Cookbook/src/ Cookbook/src/
COPY Shared/src/ Shared/src/
RUN dotnet publish Cookbook/src/Recipes/Recipes.csproj -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
COPY seed/photos /app/seed/photos

ENTRYPOINT ["dotnet", "Recipes.dll"]
