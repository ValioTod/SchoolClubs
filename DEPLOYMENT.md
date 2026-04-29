# SchoolClubs Deployment Guide

## 🚀 Quick Start: Recommended Deployment

**For most users, we recommend [Azure App Service](./AZURE_DEPLOYMENT.md)** - it's the easiest and most cost-effective option for ASP.NET Core applications.

### 30-Second Overview

1. Create free Azure account (if needed)
2. Run setup script: `.\scripts\azure-setup.ps1`
3. Deploy: `dotnet publish` → `az webapp up`
4. Visit: `https://schoolclubs-app.azurewebsites.net`

👉 **[Full Azure Deployment Guide →](./AZURE_DEPLOYMENT.md)**

---

## Deployment Options Comparison

| Platform | Cost | Setup Time | Difficulty | Best For |
|----------|------|-----------|-----------|----------|
| **Azure** | $13/month | 10 min | ⭐ Easy | ✅ Recommended - Native .NET |
| Docker | Free-$50/mo | 15 min | ⭐⭐ Medium | Containers, scalability |
| IIS | $50-200/yr | 30 min | ⭐⭐ Medium | Windows servers |
| Linux | $5/month | 20 min | ⭐⭐ Medium | VPS, full control |

---

## Overview

This guide provides instructions for deploying the SchoolClubs application to production environments. The application can be deployed using Docker containers or directly to a Windows/Linux server.

## Prerequisites

- .NET 8.0 Runtime
- SQL Server 2019+ (or SQL Server Express)
- Docker & Docker Compose (for containerized deployment)
- IIS (for Windows Server deployment)

## Environment Configuration

### Development Environment
- Connection String: `(localdb)\MSSQLLocalDB`
- Logging Level: `Information`
- ASPNETCORE_ENVIRONMENT: `Development`

### Production Environment
- Connection String: Configure in `appsettings.Production.json`
- Logging Level: `Warning`
- ASPNETCORE_ENVIRONMENT: `Production`
- HTTPS: Enabled with valid SSL certificate

## Deployment Options

### Option 0: Azure App Service (Recommended) ⭐

**See detailed instructions in [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)**

Benefits:
- ✅ Native .NET/SQL Server support from Microsoft
- ✅ Auto-scaling, monitoring, backups included
- ✅ Free tier available ($13/month after)
- ✅ GitHub Actions integration
- ✅ SSL certificates included
- ✅ Application Insights for analytics

Quick setup:
```powershell
# 1. Run automated setup
.\scripts\azure-setup.ps1

# 2. Publish and deploy
dotnet publish SchoolClubs.Web -c Release -o ./publish
az webapp up --resource-group schoolclubs-rg --name schoolclubs-app --runtime DOTNETCORE:8.0
```

### Option 1: Docker Deployment (Scalable)

#### Prerequisites
- Docker installed
- Docker Compose installed

#### Steps

1. **Build and run with Docker Compose:**
   ```bash
   docker-compose up -d
   ```

2. **Verify deployment:**
   ```bash
   docker-compose ps
   curl http://localhost:5000
   ```

3. **View logs:**
   ```bash
   docker-compose logs -f schoolclubs-app
   ```

4. **Stop services:**
   ```bash
   docker-compose down
   ```

#### Production Docker Deployment

For production, push the image to a container registry:

```bash
# Build image
docker build -t schoolclubs:latest .

# Tag for registry
docker tag schoolclubs:latest your-registry.azurecr.io/schoolclubs:latest

# Push to registry
docker push your-registry.azurecr.io/schoolclubs:latest
```

### Option 2: IIS Deployment (Windows Server)

#### Prerequisites
- Windows Server 2016+
- IIS 10+
- .NET 8.0 Hosting Bundle installed

#### Steps

1. **Publish the application:**
   ```bash
   dotnet publish SchoolClubs.Web/SchoolClubs.Web.csproj -c Release -o C:\Publish\SchoolClubs
   ```

2. **Create IIS Application Pool:**
   - Open IIS Manager
   - Create new Application Pool named "SchoolClubsPool"
   - Set .NET CLR version to "No Managed Code"

3. **Create IIS Website:**
   - Create new website "SchoolClubs"
   - Point physical path to `C:\Publish\SchoolClubs`
   - Assign Application Pool "SchoolClubsPool"
   - Configure binding with HTTPS certificate

4. **Configure connection string:**
   - Edit `appsettings.Production.json`
   - Update SQL Server connection string

5. **Set environment variable:**
   - Open Command Prompt as Administrator
   - Run: `setx /M ASPNETCORE_ENVIRONMENT Production`

6. **Restart IIS:**
   ```bash
   iisreset /restart
   ```

### Option 3: Linux Server Deployment

#### Prerequisites
- Ubuntu 20.04 LTS+
- .NET 8.0 Runtime
- Nginx reverse proxy
- SQL Server or Azure SQL Database

#### Steps

1. **Create service directory:**
   ```bash
   sudo mkdir -p /opt/schoolclubs
   cd /opt/schoolclubs
   ```

2. **Publish application:**
   ```bash
   dotnet publish SchoolClubs.Web/SchoolClubs.Web.csproj -c Release -o /opt/schoolclubs/app
   ```

3. **Create systemd service:**
   ```bash
   sudo nano /etc/systemd/system/schoolclubs.service
   ```
   
   Content:
   ```ini
   [Unit]
   Description=SchoolClubs Application
   After=network.target
   
   [Service]
   Type=notify
   User=schoolclubs
   WorkingDirectory=/opt/schoolclubs/app
   ExecStart=/usr/bin/dotnet /opt/schoolclubs/app/SchoolClubs.Web.dll
   Restart=on-failure
   RestartSec=10
   Environment="ASPNETCORE_ENVIRONMENT=Production"
   Environment="ASPNETCORE_URLS=http://0.0.0.0:5000"
   
   [Install]
   WantedBy=multi-user.target
   ```

4. **Enable and start service:**
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl enable schoolclubs
   sudo systemctl start schoolclubs
   ```

5. **Configure Nginx as reverse proxy:**
   ```nginx
   server {
       listen 80;
       server_name schoolclubs.example.com;
       
       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
           proxy_cache_bypass $http_upgrade;
       }
   }
   ```

## Database Deployment

### SQL Server Setup

1. **Create database:**
   ```sql
   CREATE DATABASE SchoolClubsDB;
   ```

2. **Run migrations:**
   ```bash
   dotnet ef database update --project SchoolClubs.Web
   ```

3. **Seed data (optional):**
   The application automatically seeds initial data on first run.

### Azure SQL Database

1. **Create Azure SQL Database** in Azure Portal

2. **Update connection string** in `appsettings.Production.json`:
   ```
   Server=tcp:your-server.database.windows.net,1433;Initial Catalog=SchoolClubsDB;Persist Security Info=False;User ID=admin-username;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;
   ```

3. **Run migrations:**
   ```bash
   dotnet ef database update --project SchoolClubs.Web
   ```

## Security Considerations

- **SSL/TLS**: Always use HTTPS in production
- **Connection Strings**: Store in environment variables, not in code
- **Secrets**: Use Azure Key Vault or similar for sensitive data
- **CORS**: Configure appropriate CORS policies
- **Authentication**: Review and configure authentication providers
- **Database**: Use strong passwords and enable encryption at rest

## Monitoring & Logging

### Application Logging
- Logs are written to console in Docker
- Configure log level in `appsettings.Production.json`
- Monitor with `docker-compose logs`

### Health Checks
- Health endpoint: `http://localhost/health`
- Used by Docker health checks
- Configure in `Program.cs`

### Performance Monitoring
- Azure Application Insights integration can be configured
- Monitor SQL Server query performance
- Track user activities

## Maintenance

### Database Backups
```sql
-- SQL Server backup
BACKUP DATABASE SchoolClubsDB TO DISK = 'C:\Backups\SchoolClubsDB.bak'
```

### Updating the Application
1. Pull latest code
2. Rebuild and publish
3. Update running containers or IIS application
4. Run database migrations if needed
5. Verify deployment

## Troubleshooting

### Application won't start
- Check connection string in `appsettings.Production.json`
- Verify database is accessible
- Check .NET runtime version: `dotnet --version`

### Database connection issues
- Verify SQL Server is running
- Check firewall rules
- Verify credentials in connection string

### Docker issues
- Check Docker daemon: `docker ps`
- View logs: `docker logs container-name`
- Restart services: `docker-compose restart`

## Rollback Procedure

1. Keep previous deployment version available
2. Update application files to previous version
3. Restart application service
4. If database changes were made, restore from backup

## Support

For issues or questions, contact the development team or refer to the main README.md file.
