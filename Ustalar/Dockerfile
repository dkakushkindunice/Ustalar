# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Install Node.js for Tailwind CSS build
RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y --no-install-recommends nodejs \
    && rm -rf /var/lib/apt/lists/*

# Install npm dependencies first (better layer caching)
COPY ["package.json", "./"]
RUN npm install

COPY ["Ustalar.csproj", "."]
RUN dotnet restore

COPY . .

# Compile Tailwind CSS (scans all .cshtml files)
RUN npm run css:build

RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Ustalar.dll"]
