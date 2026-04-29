# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy the solution files
COPY SchoolClubs.sln .
COPY SchoolClubs.Web/ SchoolClubs.Web/
COPY SchoolClubs.Tests/ SchoolClubs.Tests/

# Restore and build
RUN dotnet restore
RUN dotnet build -c Release --no-restore

# Test stage (optional)
FROM build AS test
RUN dotnet test SchoolClubs.Tests/SchoolClubs.Tests.csproj -c Release --no-build --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish SchoolClubs.Web/SchoolClubs.Web.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create non-root user
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

EXPOSE 80 443

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "SchoolClubs.Web.dll"]
