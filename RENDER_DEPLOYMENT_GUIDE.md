# 🚀 Render.com Deployment Guide

Hướng dẫn chi tiết deploy URLShortener Microservices lên Render.com với GitHub Actions.

---

## 📋 Tổng quan

**Render.com** là platform cloud miễn phí (có plan trả phí) hỗ trợ:
- ✅ Docker deployments
- ✅ PostgreSQL/MySQL databases  
- ✅ Auto-deploy from GitHub
- ✅ Free SSL certificates
- ✅ Custom domains

**Free tier limits:**
- 750 hours/month per service
- Services sleep sau 15 phút không hoạt động
- Startup time ~1-2 phút khi wake up

---

## 🎯 Kiến trúc deploy

```
GitHub Push → GitHub Actions → Trigger Render Deploy Hooks
                                        ↓
                        ┌───────────────┴───────────────┐
                        ▼                               ▼
                   Render Platform              PostgreSQL Database
                        │
        ┌───────────────┼───────────────┬───────────────┐
        ▼               ▼               ▼               ▼
    Gateway      UrlShortener      Redirect        Analytics
   (Web Service)  (Web Service)  (Web Service)  (Background Worker)
```

---

## 📝 BƯỚC 1: Tạo tài khoản Render

1. Truy cập: https://render.com
2. Click **"Get Started"**
3. Sign up bằng GitHub account (recommended)
4. Verify email

✅ **Hoàn thành**: Bạn đã có tài khoản Render!

---

## 📦 BƯỚC 2: Tạo PostgreSQL Database

**Lưu ý**: Render **KHÔNG** support MySQL free tier, ta sẽ dùng PostgreSQL.

### 2.1. Tạo Database

1. Vào Render Dashboard: https://dashboard.render.com
2. Click **"New +"** → **"PostgreSQL"**
3. Điền thông tin:
   ```
   Name: urlshortener-db
   Database: urlshortener
   User: urlshortener_user
   Region: Singapore (gần VN nhất)
   PostgreSQL Version: 16
   Plan: Free
   ```
4. Click **"Create Database"**

### 2.2. Lấy Connection String

1. Vào database vừa tạo
2. Scroll xuống phần **"Connections"**
3. Copy **"Internal Database URL"** (dạng: `postgresql://...`)
4. Lưu lại để dùng sau

✅ **Hoàn thành**: Database đã sẵn sàng!

---

## 🔧 BƯỚC 3: Cập nhật code để dùng PostgreSQL

### 3.1. Sửa Connection String format

PostgreSQL dùng connection string khác MySQL. Cần update code:

**File: `src/Services/UrlShortenerService/Program.cs`**

Tìm dòng:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

Đổi thành:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
```

**File: `src/Services/RedirectService/Program.cs`** - Tương tự

**File: `src/Services/AnalyticsService/Program.cs`** - Tương tự

### 3.2. Update NuGet packages

Thêm vào các file `.csproj`:

```xml
<!-- Xóa MySQL package -->
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.0" Remove="true" />

<!-- Thêm PostgreSQL package -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

### 3.3. Tạo migrations mới cho PostgreSQL

```bash
cd src/Services/UrlShortenerService
dotnet ef migrations add InitialCreatePostgreSQL --context ApplicationDbContext
```

✅ **Hoàn thành**: Code đã support PostgreSQL!

---

## 🐳 BƯỚC 4: Tạo Web Services trên Render

### 4.1. Gateway Service

1. Click **"New +"** → **"Web Service"**
2. Connect GitHub repository: `tuandthe/URLShortener`
3. Điền thông tin:
   ```
   Name: urlshortener-gateway
   Region: Singapore
   Branch: main
   Root Directory: (để trống)
   Runtime: Docker
   Dockerfile Path: src/Gateway/Dockerfile
   Plan: Free
   ```
4. Environment Variables:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:$PORT
   ```
5. Click **"Create Web Service"**

### 4.2. UrlShortener Service

Lặp lại như Gateway với thông tin:
```
Name: urlshortener-service
Dockerfile Path: src/Services/UrlShortenerService/Dockerfile
Environment Variables:
  ASPNETCORE_ENVIRONMENT=Production
  ASPNETCORE_URLS=http://+:$PORT
  ConnectionStrings__DefaultConnection=[PASTE DATABASE URL TỪ BƯỚC 2.2]
  BaseUrl=https://urlshortener-gateway.onrender.com
```

### 4.3. Redirect Service

```
Name: urlshortener-redirect
Dockerfile Path: src/Services/RedirectService/Dockerfile
Environment Variables:
  ASPNETCORE_ENVIRONMENT=Production
  ASPNETCORE_URLS=http://+:$PORT
  ConnectionStrings__DefaultConnection=[DATABASE URL]
  RabbitMQ__HostName=localhost
  RabbitMQ__Port=5672
  RabbitMQ__UserName=guest
  RabbitMQ__Password=guest
```

**⚠️ Lưu ý**: Render free tier không support RabbitMQ. Có 2 cách:
- **Option 1**: Dùng CloudAMQP (RabbitMQ as a service - free tier)
- **Option 2**: Tắt RabbitMQ trong development (comment code)

### 4.4. Analytics Service

```
Name: urlshortener-analytics
Dockerfile Path: src/Services/AnalyticsService/Dockerfile
Instance Type: Background Worker (không phải Web Service!)
Environment Variables:
  DOTNET_ENVIRONMENT=Production
  ConnectionStrings__DefaultConnection=[DATABASE URL]
```

✅ **Hoàn thành**: Tất cả services đã được tạo!

---

## 🔗 BƯỚC 5: Lấy Deploy Hooks

Deploy hooks cho phép GitHub Actions trigger deployment.

### 5.1. Lấy hook từ mỗi service

Với mỗi service (Gateway, UrlShortener, Redirect, Analytics):

1. Vào service trên Render Dashboard
2. Click tab **"Settings"**
3. Scroll xuống **"Deploy Hook"**
4. Click **"Copy"** để copy URL
5. Lưu lại

Bạn sẽ có 4 URLs giống như:
```
https://api.render.com/deploy/srv-xxxxx?key=yyyyy
```

✅ **Hoàn thành**: Đã có deploy hooks!

---

## 🔐 BƯỚC 6: Setup GitHub Secrets

1. Vào GitHub repository: https://github.com/tuandthe/URLShortener
2. **Settings** → **Secrets and variables** → **Actions**
3. Click **"New repository secret"**
4. Thêm 4 secrets:

```
Name: RENDER_DEPLOY_HOOK_GATEWAY
Value: [Paste deploy hook URL của Gateway]

Name: RENDER_DEPLOY_HOOK_URLSHORTENER
Value: [Paste deploy hook URL của UrlShortener]

Name: RENDER_DEPLOY_HOOK_REDIRECT
Value: [Paste deploy hook URL của Redirect]

Name: RENDER_DEPLOY_HOOK_ANALYTICS
Value: [Paste deploy hook URL của Analytics]
```

✅ **Hoàn thành**: GitHub Actions đã được config!

---

## 🚀 BƯỚC 7: Deploy!

### 7.1. Commit và push code

```bash
cd d:\Desktop\Mon Hoc\Onluyen\URLShortener

git add .
git commit -m "Setup Render deployment with GitHub Actions"
git push origin main
```

### 7.2. Theo dõi deployment

1. **GitHub Actions**: Vào tab **Actions** trên GitHub repo
   - Xem workflow "Deploy to Render" đang chạy
   - Đảm bảo tất cả steps ✅ green

2. **Render Dashboard**: https://dashboard.render.com
   - Xem từng service đang deploy
   - Status: "Deploying..." → "Live"
   - Xem logs nếu có lỗi

### 7.3. Verify deployment

Sau khi tất cả services "Live":

```bash
# Test Gateway
curl https://urlshortener-gateway.onrender.com/health

# Test UrlShortener API
curl -X POST https://urlshortener-gateway.onrender.com/api/urls \
  -H "Content-Type: application/json" \
  -d '{"longUrl": "https://google.com"}'

# Lấy URL từ response và test redirect
curl -L https://urlshortener-redirect.onrender.com/[SHORT_CODE]
```

✅ **Hoàn thành**: Services đã live trên Render!

---

## 🌐 BƯỚC 8: Deploy Frontend lên Render

### 8.1. Tạo Static Site

1. Click **"New +"** → **"Static Site"**
2. Connect repository `tuandthe/URLShortener`
3. Điền thông tin:
   ```
   Name: urlshortener-frontend
   Branch: main
   Root Directory: src/Frontend
   Build Command: npm install && npm run build
   Publish Directory: dist
   ```
4. Environment Variables:
   ```
   VITE_API_URL=https://urlshortener-gateway.onrender.com
   ```
5. Click **"Create Static Site"**

### 8.2. Update Frontend API URL

**File: `src/Frontend/src/services/urlShortenerService.ts`**

```typescript
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';
```

✅ **Hoàn thành**: Frontend đã deploy!

---

## 📊 BƯỚC 9: Cấu hình CORS

Backend cần allow requests từ frontend domain.

**File: `src/Gateway/Program.cs`**

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://urlshortener-frontend.onrender.com",
            "http://localhost:3000"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// ...

app.UseCors("AllowFrontend");
```

Commit và push lại để trigger redeploy.

✅ **Hoàn thành**: CORS đã được config!

---

## 🔄 BƯỚC 10: Setup Auto-Deploy

Render đã tự động watch GitHub repo!

Mỗi khi push lên `main`:
1. GitHub Actions trigger deploy hooks
2. Render services tự động rebuild và deploy
3. Downtime ~1-2 phút

✅ **Hoàn thành**: Auto-deploy đã sẵn sàng!

---

## 📝 URLs của bạn

Sau khi hoàn tất, bạn sẽ có:

```
Frontend:     https://urlshortener-frontend.onrender.com
Gateway:      https://urlshortener-gateway.onrender.com
UrlShortener: https://urlshortener-service.onrender.com
Redirect:     https://urlshortener-redirect.onrender.com
Analytics:    (Background worker - không có public URL)
Database:     (Internal only)
```

---

## 🐛 Troubleshooting

### ❌ Service build fails

**Check:**
- Dockerfile paths đúng chưa
- Dependencies trong Dockerfile
- Logs trong Render Dashboard

### ❌ Database connection fails

**Check:**
- Connection string đúng format PostgreSQL
- Database URL có dùng Internal URL
- Migrations đã chạy chưa

### ❌ RabbitMQ not available

**Solutions:**
- Dùng CloudAMQP: https://www.cloudamqp.com (free tier)
- Tạm thời comment RabbitMQ code trong development

### ❌ CORS errors

**Check:**
- Frontend domain trong CORS policy
- Redeploy backend sau khi update CORS

### ❌ Service sleeps after 15 minutes

**Free tier limitation**: Services tự động sleep sau 15 phút không dùng.

**Solutions:**
- Upgrade to paid plan ($7/month)
- Hoặc dùng uptime monitoring (ping service mỗi 10 phút)

---

## 💰 Chi phí

### Free Tier (hiện tại)
```
PostgreSQL Database:  Free (1GB storage)
4x Web Services:      Free (750 hours/month mỗi service)
1x Static Site:       Free (100GB bandwidth/month)
---
Total: $0/month
```

### Paid Plan (recommended cho production)
```
PostgreSQL Database:  $7/month (10GB storage, không sleep)
4x Web Services:      $28/month ($7 x 4, không sleep, custom domains)
1x Static Site:       Free
---
Total: $35/month
```

---

## 🎯 Next Steps

1. ✅ Monitor logs trên Render Dashboard
2. ✅ Setup custom domain (optional)
3. ✅ Enable auto-scaling (paid plan)
4. ✅ Setup monitoring alerts
5. ✅ Implement database backups

---

## 📚 Resources

- [Render Documentation](https://render.com/docs)
- [PostgreSQL on Render](https://render.com/docs/databases)
- [Deploy Hooks](https://render.com/docs/deploy-hooks)
- [GitHub Actions](https://docs.github.com/en/actions)

---

**Happy Deploying! 🚀**
