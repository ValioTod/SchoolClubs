# Auto-deploy to Azure setup script
# This script automates the Azure resource creation and deployment

param(
    [string]$ResourceGroup = "schoolclubs-rg",
    [string]$Location = "eastus",
    [string]$AppServicePlan = "schoolclubs-plan",
    [string]$AppService = "schoolclubs-app",
    [string]$SqlServerName = "schoolclubs-sql-$(Get-Random -Minimum 100000 -Maximum 999999)",
    [string]$SqlDatabase = "SchoolClubsDB",
    [string]$SqlAdmin = "sqladmin"
)

# Generate secure SQL password
$SqlPassword = "P@ss$(Get-Random -Minimum 100000 -Maximum 999999)!Aa"

Write-Host "========================================" -ForegroundColor Green
Write-Host "SchoolClubs Azure Deployment Setup" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Check if logged in to Azure
Write-Host "Checking Azure CLI login..." -ForegroundColor Yellow
$currentUser = az account show 2>$null
if (-not $currentUser) {
    Write-Host "Not logged in to Azure. Please login..." -ForegroundColor Yellow
    az login
}

Write-Host "Logged in successfully!" -ForegroundColor Green
Write-Host ""

# Create Resource Group
Write-Host "Creating Resource Group: $ResourceGroup" -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location
Write-Host "✓ Resource Group created" -ForegroundColor Green
Write-Host ""

# Create App Service Plan
Write-Host "Creating App Service Plan: $AppServicePlan" -ForegroundColor Yellow
az appservice plan create `
    --name $AppServicePlan `
    --resource-group $ResourceGroup `
    --sku B1 `
    --is-linux `
    --runtime "DOTNETCORE:8.0"
Write-Host "✓ App Service Plan created" -ForegroundColor Green
Write-Host ""

# Create App Service
Write-Host "Creating App Service: $AppService" -ForegroundColor Yellow
az webapp create `
    --resource-group $ResourceGroup `
    --plan $AppServicePlan `
    --name $AppService `
    --runtime "DOTNETCORE:8.0"
Write-Host "✓ App Service created" -ForegroundColor Green
Write-Host ""

# Create SQL Server
Write-Host "Creating SQL Server: $SqlServerName" -ForegroundColor Yellow
az sql server create `
    --name $SqlServerName `
    --resource-group $ResourceGroup `
    --location $Location `
    --admin-user $SqlAdmin `
    --admin-password $SqlPassword
Write-Host "✓ SQL Server created" -ForegroundColor Green
Write-Host ""

# Create SQL Database
Write-Host "Creating SQL Database: $SqlDatabase" -ForegroundColor Yellow
az sql db create `
    --resource-group $ResourceGroup `
    --server $SqlServerName `
    --name $SqlDatabase `
    --sku Basic
Write-Host "✓ SQL Database created" -ForegroundColor Green
Write-Host ""

# Allow Azure Services
Write-Host "Configuring firewall rules..." -ForegroundColor Yellow
az sql server firewall-rule create `
    --resource-group $ResourceGroup `
    --server $SqlServerName `
    --name "AllowAzureServices" `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0
Write-Host "✓ Firewall rules configured" -ForegroundColor Green
Write-Host ""

# Build connection string
$SqlFqdn = "$SqlServerName.database.windows.net"
$ConnectionString = "Server=tcp:$SqlFqdn,1433;Initial Catalog=$SqlDatabase;Persist Security Info=False;User ID=$SqlAdmin;Password=$SqlPassword;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;"

# Configure App Service
Write-Host "Configuring App Service settings..." -ForegroundColor Yellow

# Add connection string
az webapp config connection-string set `
    --resource-group $ResourceGroup `
    --name $AppService `
    --settings DefaultConnection=$ConnectionString `
    --connection-string-type SQLAzure

# Add app settings
az webapp config appsettings set `
    --resource-group $ResourceGroup `
    --name $AppService `
    --settings ASPNETCORE_ENVIRONMENT=Azure

Write-Host "✓ App Service configured" -ForegroundColor Green
Write-Host ""

# Display results
Write-Host "========================================" -ForegroundColor Green
Write-Host "Setup Complete! 🎉" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Configuration Details:" -ForegroundColor Cyan
Write-Host "  Resource Group: $ResourceGroup"
Write-Host "  App Service URL: https://$AppService.azurewebsites.net"
Write-Host "  SQL Server: $SqlServerName.database.windows.net"
Write-Host "  SQL Database: $SqlDatabase"
Write-Host "  SQL Admin: $SqlAdmin"
Write-Host ""
Write-Host "🔐 Save these credentials safely:" -ForegroundColor Yellow
Write-Host "  SQL Password: $SqlPassword"
Write-Host ""
Write-Host "📝 Next steps:" -ForegroundColor Green
Write-Host "  1. Build the project:"
Write-Host "     dotnet publish SchoolClubs.Web -c Release -o ./publish"
Write-Host ""
Write-Host "  2. Deploy to Azure:"
Write-Host "     az webapp up --resource-group $ResourceGroup --name $AppService --runtime DOTNETCORE:8.0"
Write-Host ""
Write-Host "  3. Or download publish profile for automated CI/CD:"
Write-Host "     Go to Azure Portal > App Service > Overview > Download publish profile"
Write-Host ""
Write-Host "  4. Visit your app:"
Write-Host "     https://$AppService.azurewebsites.net"
Write-Host ""
Write-Host "For more details, see AZURE_DEPLOYMENT.md" -ForegroundColor Green
