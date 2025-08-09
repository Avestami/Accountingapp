# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY src/Accounting.Domain/Accounting.Domain.csproj src/Accounting.Domain/
COPY src/Accounting.Application/Accounting.Application.csproj src/Accounting.Application/
COPY src/Accounting.Infrastructure/Accounting.Infrastructure.csproj src/Accounting.Infrastructure/
COPY src/Accounting.API/Accounting.API.csproj src/Accounting.API/

RUN dotnet restore src/Accounting.API/Accounting.API.csproj

# Copy the entire source code
    COPY src/ src/
    COPY appsettings.json .

# Build the application
RUN dotnet build src/Accounting.API/Accounting.API.csproj -c Release --no-restore

# Publish the API
RUN dotnet publish src/Accounting.API/Accounting.API.csproj -c Release -o /app/api --no-restore

# Use the official .NET runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/api ./api

# Expose port
EXPOSE 5000

# Start the application
CMD ["dotnet", "api/Accounting.API.dll", "--urls", "http://0.0.0.0:5000"]