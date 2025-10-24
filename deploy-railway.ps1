# Railway Deployment Script for PowerShell

# 1. Install Railway CLI (chỉ chạy 1 lần)
Write-Host "Installing Railway CLI..." -ForegroundColor Green
npm install -g @railway/cli

# 2. Login to Railway
Write-Host "`nLogin to Railway..." -ForegroundColor Green
railway login

# 3. Link to your project
Write-Host "`nLink to Railway project..." -ForegroundColor Green
Write-Host "Paste your Railway Project ID when prompted" -ForegroundColor Yellow
railway link

# 4. Deploy Gateway
Write-Host "`nDeploying Gateway Service..." -ForegroundColor Green
railway up --service gateway --dockerfile src/Gateway/Dockerfile

# 5. Deploy UrlShortener Service
Write-Host "`nDeploying UrlShortener Service..." -ForegroundColor Green
railway up --service urlshortener-service --dockerfile src/Services/UrlShortenerService/Dockerfile

# 6. Deploy Redirect Service
Write-Host "`nDeploying Redirect Service..." -ForegroundColor Green
railway up --service redirect-service --dockerfile src/Services/RedirectService/Dockerfile

# 7. Deploy Analytics Service
Write-Host "`nDeploying Analytics Service..." -ForegroundColor Green
railway up --service analytics-service --dockerfile src/Services/AnalyticsService/Dockerfile

Write-Host "`n✅ All services deployed successfully!" -ForegroundColor Green
Write-Host "Check Railway Dashboard to verify: https://railway.app" -ForegroundColor Cyan
