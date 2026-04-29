# ⚡ Quick Start: Deploy to Azure in 5 Minutes

This is the fastest way to get your SchoolClubs app live!

## Prerequisites (2 minutes)

1. **Azure Account** - [Create free](https://azure.microsoft.com/free/) (~1 minute)
2. **Azure CLI** - [Install](https://learn.microsoft.com/cli/azure/install-azure-cli) (~1 minute)
3. Already have these:
   - Git
   - .NET 8 SDK

## Deploy (3 minutes)

### Step 1: Run Setup Script
```powershell
cd SchoolClubs
.\scripts\azure-setup.ps1
```
This creates:
- Resource Group
- App Service
- SQL Database
- Firewall rules

**Keep the output safe - you'll need SQL password!**

### Step 2: Publish Your App
```powershell
dotnet publish SchoolClubs.Web -c Release -o ./publish
```

### Step 3: Deploy to Azure
```powershell
az webapp up --resource-group schoolclubs-rg --name schoolclubs-app --runtime DOTNETCORE:8.0
```

### Step 4: Visit Your App
Open browser to: **https://schoolclubs-app.azurewebsites.net**

Login with:
- Email: `admin@schoolclubs.bg`
- Password: `Admin123!`

## That's It! 🎉

Your app is now live on Azure!

## After Deployment

**To update your app:**
```powershell
# Make changes locally
git add .
git commit -m "Update feature"

# Rebuild and deploy
dotnet publish SchoolClubs.Web -c Release -o ./publish
az webapp deployment source config-zip --resource-group schoolclubs-rg --name schoolclubs-app --src ./publish.zip
```

**Optional: Setup auto-deploy from GitHub**

See [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md) for complete setup (connects GitHub pushes to automatic Azure deployment).

## Troubleshooting

### "502 Bad Gateway"
1. Check logs: Open App Service in Azure Portal → Log Stream
2. Likely cause: Connection string not configured
3. Solution: Verify in App Service → Configuration

### "Connection failed"
1. Verify SQL password in App Service → Configuration → Connection Strings
2. Ensure "Allow Azure services" is enabled in SQL Server firewall

### Need more help?
See [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md) for detailed troubleshooting.

## Cost
- **First 12 months**: ~$15 credit/month covers everything free
- **After**: ~$13.74/month (B1 App Service + Basic SQL)

---

**[← Back to AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)** for more options
