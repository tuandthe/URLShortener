# 🚀 Railway Deployment - Quick Start Guide

Hướng dẫn deploy nhanh URLShortener lên Railway bằng GitHub Actions.

## ⚡ TL;DR - Quick Steps

1. ✅ Tạo project trên Railway
2. ✅ Thêm MySQL + RabbitMQ + 4 Empty Services
3. ✅ Lấy Railway Token và Project ID
4. ✅ Add vào GitHub Secrets
5. ✅ Push code → GitHub Actions tự động deploy!

---

## 📋 Chi tiết từng bước

### 1️⃣ Setup Railway Project (5 phút)

**A. Tạo Project:**
```
1. Login vào railway.app
2. Click "New Project" → "Empty Project"
3. Đặt tên: urlshortener-microservices
```

**B. Thêm MySQL:**
```
Click "+ New" → "Database" → "Add MySQL"
Tên: mysql
```

**C. Thêm RabbitMQ:**
```
Click "+ New" → "Empty Service"
Tên: rabbitmq
Settings → Deploy from Docker Image
Image: rabbitmq:3-management
Variables:
  RABBITMQ_DEFAULT_USER=guest
  RABBITMQ_DEFAULT_PASS=guest
```

**D. Thêm 4 Empty Services:**
```
Service 1: gateway
Service 2: urlshortener-service
Service 3: redirect-service
Service 4: analytics-service
```

**E. Set Environment Variables cho từng service:**

**Gateway:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:$PORT
```

**UrlShortener Service:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:$PORT
ConnectionStrings__DefaultConnection=${{MySQL.DATABASE_URL}}
BaseUrl=https://${{gateway.RAILWAY_PUBLIC_DOMAIN}}
```

**Redirect Service:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:$PORT
ConnectionStrings__DefaultConnection=${{MySQL.DATABASE_URL}}
RabbitMQ__HostName=${{rabbitmq.RAILWAY_PRIVATE_DOMAIN}}
RabbitMQ__Port=5672
RabbitMQ__UserName=guest
RabbitMQ__Password=guest
```

**Analytics Service:**
```
DOTNET_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=${{MySQL.DATABASE_URL}}
RabbitMQ__HostName=${{rabbitmq.RAILWAY_PRIVATE_DOMAIN}}
RabbitMQ__Port=5672
RabbitMQ__UserName=guest
RabbitMQ__Password=guest
```

### 2️⃣ Lấy Railway Credentials (2 phút)

**A. Railway Token:**
```
1. Vào railway.app/account/tokens
2. Click "Create Token"
3. Copy token (chỉ hiện 1 lần!)
```

**B. Project ID:**
```
1. Vào project của bạn
2. Settings → Copy "Project ID"
```

### 3️⃣ Setup GitHub Secrets (2 phút)

```
1. Vào GitHub repo → Settings → Secrets and variables → Actions
2. Click "New repository secret"
3. Thêm 2 secrets:
   
   Name: RAILWAY_TOKEN
   Value: [paste token từ bước 2A]
   
   Name: RAILWAY_PROJECT_ID
   Value: [paste project ID từ bước 2B]
```

### 4️⃣ Deploy! (1 phút)

```bash
git add .
git commit -m "Setup Railway deployment"
git push origin master
```

✅ Xem GitHub Actions → tab "Actions" để theo dõi deployment!

---

## 🔍 Verify Deployment

### Check GitHub Actions:
```
1. Vào GitHub repo → tab Actions
2. Xem workflow "Deploy to Railway"
3. Đảm bảo tất cả steps đều ✅ green
```

### Check Railway Dashboard:
```
1. Vào railway.app → project của bạn
2. Xem tất cả 4 services đang running (green)
3. Check logs nếu có service failed (red)
```

### Test Endpoints:
```bash
# Lấy Gateway URL từ Railway
GATEWAY_URL="https://gateway-production-xxxx.up.railway.app"

# Test health
curl $GATEWAY_URL/health

# Create short URL
curl -X POST $GATEWAY_URL/api/urls \
  -H "Content-Type: application/json" \
  -d '{"longUrl": "https://google.com"}'

# Test redirect
curl -L $GATEWAY_URL/{shortCode}
```

---

## 🐛 Troubleshooting

### ❌ Problem: GitHub Actions fails với "railway: command not found"
**Solution:** 
- Check workflow file có `npm install -g @railway/cli`
- Thử chạy lại workflow

### ❌ Problem: "Invalid token" error
**Solution:**
- Verify `RAILWAY_TOKEN` trong GitHub Secrets
- Token có thể expired → tạo token mới

### ❌ Problem: "Project not found"
**Solution:**
- Verify `RAILWAY_PROJECT_ID` đúng
- Check project ID trong Railway Settings

### ❌ Problem: Service build fails
**Solution:**
- Check Railway logs để xem error chi tiết
- Verify Dockerfile paths trong workflow
- Ensure all dependencies trong Dockerfile

### ❌ Problem: Service không connect được MySQL
**Solution:**
- Check MySQL service đang running
- Verify environment variable: `${{MySQL.DATABASE_URL}}`
- Check connection string format

---

## 📊 Architecture Overview

```
┌─────────────────────────────────────────────┐
│           GitHub (Source Code)              │
└──────────────┬──────────────────────────────┘
               │ Push to master
               ▼
┌─────────────────────────────────────────────┐
│         GitHub Actions (CI/CD)              │
│  1. Install Railway CLI                     │
│  2. Link to Railway Project                 │
│  3. Deploy each service with Dockerfile     │
└──────────────┬──────────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────────┐
│         Railway Platform (Cloud)            │
│                                             │
│  ┌─────────┐  ┌──────────────┐            │
│  │  MySQL  │  │   RabbitMQ   │            │
│  └────┬────┘  └──────┬───────┘            │
│       │              │                     │
│  ┌────┴──────────────┴────────────┐       │
│  │                                 │       │
│  │  ┌─────────────────────────┐   │       │
│  │  │   Gateway Service       │◄──┼───────┼─── Public Domain
│  │  └───────┬─────────────────┘   │       │
│  │          │                      │       │
│  │  ┌───────▼─────────────────┐   │       │
│  │  │ UrlShortener Service    │   │       │
│  │  └─────────────────────────┘   │       │
│  │                                 │       │
│  │  ┌─────────────────────────┐   │       │
│  │  │  Redirect Service       │   │       │
│  │  └─────────────────────────┘   │       │
│  │                                 │       │
│  │  ┌─────────────────────────┐   │       │
│  │  │  Analytics Service      │   │       │
│  │  └─────────────────────────┘   │       │
│  │                                 │       │
│  └─────────────────────────────────┘       │
└─────────────────────────────────────────────┘
```

---

## 🎯 Next Steps

1. ✅ **Generate Public Domain** cho Gateway service
2. ✅ **Update `ocelot.json`** với Railway internal URLs
3. ✅ **Setup monitoring** alerts trên Railway
4. ✅ **Configure custom domain** (optional)
5. ✅ **Enable auto-deployment** on push (đã có sẵn!)

---

## 📚 References

- [Railway Docs](https://docs.railway.app/)
- [Railway CLI Commands](https://docs.railway.app/develop/cli)
- [GitHub Actions](https://docs.github.com/en/actions)
- [Full Deployment Guide](./RAILWAY_DEPLOYMENT.md)

---

**Happy Deploying! 🚂✨**
