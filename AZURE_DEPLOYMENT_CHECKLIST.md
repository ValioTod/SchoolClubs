# Azure Deployment Checklist

## Pre-Deployment

- [ ] Have Azure account (free: https://azure.microsoft.com/free/)
- [ ] Azure CLI installed
- [ ] Git configured
- [ ] .NET 8 SDK installed

## Azure Setup

- [ ] Run setup script: `.\scripts\azure-setup.ps1`
- [ ] Save SQL password securely
- [ ] Note App Service URL (schoolclubs-app.azurewebsites.net)
- [ ] Verify resources created in Azure Portal

## Application Configuration

- [ ] Connection string added to App Service settings
- [ ] ASPNETCORE_ENVIRONMENT set to "Azure"
- [ ] Health check configured (if using Application Insights)

## Build & Publish

- [ ] Build application: `dotnet build`
- [ ] Run tests: `dotnet test`
- [ ] Publish: `dotnet publish SchoolClubs.Web -c Release -o ./publish`

## Deployment

- [ ] Deploy via `az webapp up` command
- [ ] Or use Visual Studio Publish to Azure
- [ ] Or use GitHub Actions with publish profile

## Post-Deployment

- [ ] Visit app URL in browser
- [ ] Login with test account (admin@schoolclubs.bg / Admin123!)
- [ ] Test main features (create club, join event, etc.)
- [ ] Check Application Insights for any errors
- [ ] Monitor Log Stream for warnings

## Monitoring

- [ ] Set up Azure Alerts for failures
- [ ] Enable Application Insights (optional but recommended)
- [ ] Configure backup policy for SQL Database
- [ ] Document custom domain (if using)

## Team Access (Optional)

- [ ] Share publish profile with team for deployments
- [ ] Set up GitHub Actions for CI/CD
- [ ] Grant Azure RBAC permissions to team members

## Performance (Optional - Scale Later)

- [ ] Monitor CPU/Memory in App Service metrics
- [ ] Upgrade to S1 (Standard) if needed for auto-scaling
- [ ] Upgrade SQL Database if needed for better performance
- [ ] Configure CDN for static files (optional)

## Security

- [ ] Enable HTTPS only (default in Azure)
- [ ] Review SQL Database firewall rules
- [ ] Enable Azure Defender for Azure SQL
- [ ] Review Application settings for secrets
- [ ] Consider Key Vault for sensitive data

## Documentation

- [ ] Document SQL password location (secure!)
- [ ] Document admin credentials location
- [ ] Keep backup of connection strings
- [ ] Document any custom configurations

---

**Status: [ ] All items complete - Ready for production! 🚀**
