# Docker Setup for Travel Accounting System

This guide explains how to build and run the Travel Accounting System using Docker.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop) installed on your machine
- [Docker Compose](https://docs.docker.com/compose/install/) (usually included with Docker Desktop)

## Architecture

The application consists of three main components:

1. **SQL Server Database**: Stores all application data
2. **.NET Core API**: Backend API built with .NET 5.0
3. **Vue.js Frontend**: User interface built with Vue.js

## Configuration

The Docker setup uses the following configuration:

- **Database**: SQL Server 2019 Express
  - Port: 1433 (default SQL Server port)
  - Username: sa
  - Password: YourStrongPassword123! (defined in docker-compose.yml)

- **API**: .NET Core 5.0
  - Port: 5000 (mapped to container port 80)
  - Environment: Development

- **UI**: Vue.js with Nginx
  - Port: 5678 (mapped to container port 80)

## Building and Running

To build and run the entire application stack:

```bash
# Navigate to the project root directory
cd /path/to/Accountingapp

# Build and start all services
docker-compose up -d

# To see logs
docker-compose logs -f

# To stop all services
docker-compose down
```

## Accessing the Application

- **Frontend**: http://localhost:5678
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

## Database Migrations

The first time you run the application, you need to apply database migrations:

```bash
# Access the API container
docker exec -it accounting-api /bin/bash

# Run migrations (inside the container)
dotnet ef database update
```

## Troubleshooting

### Database Connection Issues

If the API cannot connect to the database, ensure:

1. The SQL Server container is running: `docker ps | grep accounting-db`
2. The connection string in the API environment variables is correct
3. Wait a few moments for SQL Server to initialize fully

### Frontend Cannot Connect to API

If the frontend cannot connect to the API:

1. Ensure the API container is running: `docker ps | grep accounting-api`
2. Check the Nginx configuration in the UI container
3. Verify that the API is accessible at http://localhost:5000

## Data Persistence

The SQL Server data is persisted in a Docker volume named `sqlserver_data`. This ensures your data remains even if containers are removed.

To remove all data and start fresh:

```bash
docker-compose down -v
```

## Security Notes

- The default SQL Server password in the docker-compose.yml file is for development only
- For production, use environment variables or Docker secrets to manage sensitive information
- Update the JWT key in the API configuration for production deployments