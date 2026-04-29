# SchoolClubs - Azure Deployment Guide

## Quick Start (5 minutes)

This guide will help you deploy the SchoolClubs application to **Azure App Service** with **Azure SQL Database**.

## Prerequisites

1. **Azure Account** - [Create free account](https://azure.microsoft.com/free/)
2. **Azure CLI** - [Install](https://learn.microsoft.com/cli/azure/install-azure-cli)
3. **Git** installed on your machine
4. **GitHub account** (for automated deployments)

## Step 1: Create Azure Resources

### Option A: Using Azure Portal (Easiest)

1. **Login to Azure Portal**: https://portal.azure.com

2. **Create Resource Group**:
   - Click "Create a resource"
   - Search for "Resource Group"
   - Create: `schoolclubs-rg`

3. **Create App Service Plan**:
   - Search for "App Service Plan"
   - Click Create
   - Resource Group: `schoolclubs-rg`
   - Name: `schoolclubs-plan`
   - Operating System: **Linux**
   - Runtime Stack: **.NET 8** 
   - SKU: **B1** (Basic - $0.01/hour free tier eligible)

4. **Create App Service**:
   - Search for "App Service"
   - Click Create
   - Resource Group: `schoolclubs-rg`
   - Name: `schoolclubs-app` (will be `schoolclubs-app.azurewebsites.net`)
   - Publish: **Code**
   - Runtime Stack: **.NET 8**
   - App Service Plan: `schoolclubs-plan`

5. **Create Azure SQL Database**:
   - Search for "SQL Database"
   - Click Create
   - Resource Group: `schoolclubs-rg`
   - Database Name: `SchoolClubsDB`
   - New Server: `schoolclubs-sql` (global unique)
   - Admin: `sqladmin`
   - Password: **Create strong password** (save this!)
   - Pricing: **Basic** ($5/month)

### Option B: Using Azure CLI (Faster)

```bash
# Login to Azure
az login

# Set variables
$resourceGroup = "schoolclubs-rg"
$location = "eastus"
$appServicePlan = "schoolclubs-plan"
$appService = "schoolclubs-app"
$sqlServer = "schoolclubs-sql-$([Math]::Floor([decimal](Get-Random -Minimum 100000 -Maximum 999999)))"
$sqlDatabase = "SchoolClubsDB"
$sqlAdmin = "sqladmin"
$sqlPassword = "P@ss$(Get-Random -Minimum 100000 -Maximum 999999)"

# Create Resource Group
az group create --name $resourceGroup --location $location

# Create App Service Plan
az appservice plan create --name $appServicePlan --resource-group $resourceGroup --sku B1 --is-linux --runtime "DOTNETCORE:8.0"

# Create App Service
az webapp create --resource-group $resourceGroup --plan $appServicePlan --name $appService --runtime "DOTNETCORE:8.0"

# Create SQL Server
az sql server create --name $sqlServer --resource-group $resourceGroup --location $location --admin-user $sqlAdmin --admin-password $sqlPassword

# Create SQL Database
az sql db create --resource-group $resourceGroup --server $sqlServer --name $sqlDatabase --sku Basic

# Allow Azure Services access to SQL
az sql server firewall-rule create --resource-group $resourceGroup --server $sqlServer --name "AllowAzureServices" --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

Write-Host "SQL Connection String:"
Write-Host "Server=tcp:$sqlServer.database.windows.net,1433;Initial Catalog=$sqlDatabase;Persist Security Info=False;User ID=$sqlAdmin;Password=$sqlPassword;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;"
```

## Step 2: Get Connection String

1. Go to **SQL Database** > `SchoolClubsDB`
2. Click **Connection strings**
3. Copy the **ADO.NET** connection string
4. Replace `{your_password}` with the SQL admin password you created

Example:
```
Server=tcp:schoolclubs-sql-xxxx.database.windows.net,1433;Initial Catalog=SchoolClubsDB;Persist Security Info=False;User ID=sqladmin;Password=YourPassword123!;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;
```

## Step 3: Configure App Service

### Add Connection String

1. Go to **App Service** > `schoolclubs-app`
2. Click **Configuration** (left menu)
3. Click **Application Settings** tab
4. Click **+ New connection string**
5. Add:
   - **Name**: `DefaultConnection`
   - **Value**: Your SQL connection string (from Step 2)
   - **Type**: `SQLAzure`
6. Click **OK**, then **Save**

### Add App Settings

1. Click **+ New application setting**
2. Add:
   - **Name**: `ASPNETCORE_ENVIRONMENT`
   - **Value**: `Azure`
3. Click **OK**, then **Save**

## Step 4: Deploy Application

### Option A: Deploy from GitHub (Automated)

1. **Create GitHub Secrets**:
   - Go to GitHub repository Settings > Secrets > Actions
   - Add secret `AZURE_APP_NAME`: `schoolclubs-app`
   - Add secret `AZURE_PUBLISH_PROFILE`:
     - In Azure Portal, go to App Service > Overview
     - Click **Download publish profile**
     - Copy entire content and paste as secret value

2. **Push to main branch**:
   ```bash
   git push origin main
   ```
   Deployment will start automatically!

3. **Monitor deployment**:
   - Go to GitHub Actions tab
   - Watch the workflow run

### Option B: Manual Deployment

#### Using Visual Studio

1. Right-click `SchoolClubs.Web` project
2. Select **Publish**
3. Choose **Azure**
4. Select **Azure App Service (Linux)**
5. Sign in to Azure
6. Select `schoolclubs-app`
7. Click **Publish**

#### Using Azure CLI

```bash
# Build the project
dotnet publish SchoolClubs.Web/SchoolClubs.Web.csproj -c Release -o ./publish

# Deploy using Azure App Service extension
az webapp up --resource-group schoolclubs-rg --name schoolclubs-app --runtime DOTNETCORE:8.0
```

#### Using ZIP Deploy

```bash
# Create deployment package
cd .\SchoolClubs.Web\bin\Release\net8.0\publish
Compress-Archive -Path * -DestinationPath ..\..\schoolclubs.zip

# Deploy to Azure
az webapp deployment source config-zip --resource-group schoolclubs-rg --name schoolclubs-app --src "..\..\schoolclubs.zip"
```

## Step 5: Initialize Database

After first deployment, the database needs to be initialized:

### Option A: Using Azure Portal Bash

1. Go to **App Service** > `schoolclubs-app`
2. Click **SSH** (or **Console** if SSH not available)
3. Run:
   ```bash
   cd /home/site/wwwroot
   dotnet SchoolClubs.Web.dll
   ```
   (Let it run for a few seconds to seed the database, then Ctrl+C)

### Option B: Using Entity Framework Migrations

```bash
# Connect to Azure via SSH or Cloud Shell
# From your local machine SSH into the app:
az webapp create-remote-connection --subscription "subscription-name" --resource-group schoolclubs-rg --name schoolclubs-app

# Or use Azure Cloud Shell to run migrations:
dotnet ef database update --project SchoolClubs.Web
```

## Step 6: Verify Deployment

1. Go to **App Service** > `schoolclubs-app`
2. Click **Overview**
3. Click the URL: `https://schoolclubs-app.azurewebsites.net`
4. You should see the SchoolClubs landing page!

### Login with Test Account

Email: `admin@schoolclubs.bg`
Password: `Admin123!`

## Monitoring & Logs

### View Application Logs

1. **App Service** > `schoolclubs-app` > **Log Stream**
   - Real-time logs of your application

2. **App Insights** (optional advanced monitoring)
   - Search for "Application Insights"
   - Connected to your app for detailed analytics

### Check Health

```bash
# View app service status
az webapp list --resource-group schoolclubs-rg

# View diagnostic settings
az webapp log show --resource-group schoolclubs-rg --name schoolclubs-app
```

## Updating After Deployment

### After Making Code Changes

1. Commit and push:
   ```bash
   git add .
   git commit -m "Update feature"
   git push origin main
   ```

2. GitHub Actions automatically deploys! (if GitHub Actions workflow is configured)

### Manual Update

```bash
dotnet publish -c Release -o ./publish
az webapp deployment source config-zip --resource-group schoolclubs-rg --name schoolclubs-app --src ./publish.zip
```

## Troubleshooting

### Application shows "502 Bad Gateway"

1. Check logs: **App Service** > **Log Stream**
2. Common causes:
   - Connection string invalid - verify in **Configuration**
   - Database not initialized - run database seed
   - .NET version mismatch - verify runtime is .NET 8

### Database connection failing

1. Verify connection string in **Configuration**
2. Check SQL Server firewall:
   - **SQL Server** > **Firewalls and virtual networks**
   - Ensure "Allow Azure services and resources" is **ON**

### Deployment fails

1. Check GitHub Actions logs (if using GitHub Deploy)
2. Check **Deployment Center** in App Service
3. Verify publish profile is correctly set in secrets

## Cost Estimation

| Service | SKU | Monthly Cost |
|---------|-----|--------------|
| App Service Plan | B1 | $0.012/hour (~$8.74/month) |
| SQL Database | Basic | ~$5.00/month |
| **Total** | | ~**$13.74/month** |

✅ **Free tier eligible**: First 12 months includes:
- 1 B1 App Service Plan (free)
- 1 SQL Database 5GB (free)
- ~$15 Azure credit/month

## Scale Up Later

When ready to handle more users:
- **App Service**: Upgrade to S1 (Standard) for auto-scaling
- **SQL Database**: Upgrade to S0 (Standard) for better performance

## Custom Domain (Optional)

1. **App Service** > **Custom domains**
2. Add your domain (`schoolclubs.com`)
3. Point DNS CNAME to `schoolclubs-app.azurewebsites.net`
4. Azure will auto-configure HTTPS certificate

## Next Steps

✅ Application deployed and running!

- Monitor logs regularly
- Set up Application Insights for analytics
- Configure auto-scaling for high traffic
- Plan database backups
- Implement CI/CD for team deployments

## Support

For issues:
1. Check [Azure Documentation](https://learn.microsoft.com/azure/)
2. Review application logs in **Log Stream**
3. Check [ASP.NET Core on Azure](https://learn.microsoft.com/dotnet/azure/)
