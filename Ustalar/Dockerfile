# CSS build stage
FROM node:22-alpine AS css-build
WORKDIR /src
COPY package.json .
RUN npm install
COPY . .
RUN npm run css:build

# .NET build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Ustalar.csproj", "."]
RUN dotnet restore
COPY . .
COPY --from=css-build /src/wwwroot/css/app.min.css ./wwwroot/css/app.min.css
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ustalar.dll"]
