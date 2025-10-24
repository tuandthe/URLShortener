#!/bin/bash
# Railway Deployment Script for Bash

# 1. Install Railway CLI (chỉ chạy 1 lần)
echo "Installing Railway CLI..."
npm install -g @railway/cli

# 2. Login to Railway
echo -e "\nLogin to Railway..."
railway login

# 3. Link to your project
echo -e "\nLink to Railway project..."
echo "Paste your Railway Project ID when prompted"
railway link

# 4. Deploy Gateway
echo -e "\nDeploying Gateway Service..."
railway up --service gateway --dockerfile src/Gateway/Dockerfile

# 5. Deploy UrlShortener Service
echo -e "\nDeploying UrlShortener Service..."
railway up --service urlshortener-service --dockerfile src/Services/UrlShortenerService/Dockerfile

# 6. Deploy Redirect Service
echo -e "\nDeploying Redirect Service..."
railway up --service redirect-service --dockerfile src/Services/RedirectService/Dockerfile

# 7. Deploy Analytics Service
echo -e "\nDeploying Analytics Service..."
railway up --service analytics-service --dockerfile src/Services/AnalyticsService/Dockerfile

echo -e "\n✅ All services deployed successfully!"
echo "Check Railway Dashboard to verify: https://railway.app"
