FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Accounting.API/Accounting.API.csproj", "src/Accounting.API/"]
COPY ["src/Accounting.Application/Accounting.Application.csproj", "src/Accounting.Application/"]
COPY ["src/Accounting.Domain/Accounting.Domain.csproj", "src/Accounting.Domain/"]
COPY ["src/Accounting.Infrastructure/Accounting.Infrastructure.csproj", "src/Accounting.Infrastructure/"]
RUN dotnet restore "src/Accounting.API/Accounting.API.csproj"
COPY . .
WORKDIR "/src/src/Accounting.API"
RUN dotnet build "Accounting.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Accounting.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Accounting.API.dll"]