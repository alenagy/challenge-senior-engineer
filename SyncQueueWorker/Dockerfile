# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY SyncQueueWorker.csproj ./SyncQueueWorker.csproj
RUN dotnet restore "SyncQueueWorker.csproj"

# Copy the entire source code
COPY . .

# Build the project
RUN dotnet build "SyncQueueWorker.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "SyncQueueWorker.csproj" -c Release -o /app/publish

# Runtime stage - Use .NET 8 Runtime for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point to run the worker
ENTRYPOINT ["dotnet", "SyncQueueWorker.dll"]