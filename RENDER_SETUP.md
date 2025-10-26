# 🚀 Render.com Setup - Step by Step

## 📦 Thông tin Database từ ảnh của bạn

```
Hostname: dpg-d3u9ja1e5dus739gj9q8-a
Port: 5432
Database: urlshortener_xclo
Username: urlshortener_user
Password: ••••••••••••••••••••
Internal Database URL: postgresql://urlshortener_user:password@dpg-...
External Database URL: postgresql://urlshortener_user:password@dpg-...
```

---

## 🎯 BƯỚC 1: Tạo Web Services trên Render

### 1.1. Gateway Service

1. **New +** → **Web Service**
2. **Connect GitHub repo:** `tuandthe/URLShortener`
3. **Cấu hình:**
   ```
   Name: urlshortener-gateway
   Region: Singapore
   Branch: main
   Root Directory: (để trống)
   Runtime: Docker
   Dockerfile Path: src/Gateway/Dockerfile
   Plan: Free
   ```
4. **Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:$PORT
   ```
5. Click **Create Web Service**
6. **Đợi deploy xong** → Copy URL Gateway (vd: `https://urlshortener-gateway.onrender.com`)

---

### 1.2. UrlShortener Service

1. **New +** → **Web Service**
2. **Connect GitHub repo:** `tuandthe/URLShortener`
3. **Cấu hình:**
   ```
   Name: urlshortener-service
   Region: Singapore
   Branch: main
   Root Directory: (để trống)
   Runtime: Docker
   Dockerfile Path: src/Services/UrlShortenerService/Dockerfile
   Plan: Free
   ```
4. **Environment Variables:**
   
   **Mở Render Database Dashboard** → Click vào `urlshortener-db` → Copy **Internal Database URL**
   
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:$PORT
   ConnectionStrings__DefaultConnection=<PASTE INTERNAL DATABASE URL TỪ RENDER>
   BaseUrl=https://urlshortener-gateway.onrender.com
   ```
   
   **Ví dụ Internal Database URL:**
   ```
   postgresql://urlshortener_user:YOUR_REAL_PASSWORD@dpg-d3u9ja1e5dus739gj9q8-a/urlshortener_xclo
   ```

5. Click **Create Web Service**

---

### 1.3. Redirect Service

1. **New +** → **Web Service**
2. **Connect GitHub repo:** `tuandthe/URLShortener`
3. **Cấu hình:**
   ```
   Name: urlshortener-redirect
   Region: Singapore
   Branch: main
   Root Directory: (để trống)
   Runtime: Docker
   Dockerfile Path: src/Services/RedirectService/Dockerfile
   Plan: Free
   ```
4. **Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:$PORT
   ConnectionStrings__DefaultConnection=<PASTE INTERNAL DATABASE URL>
   ```
   
   **⚠️ Lưu ý về RabbitMQ:**
   - Render FREE tier không hỗ trợ RabbitMQ
   - **Tạm thời bỏ qua** RabbitMQ variables
   - Service vẫn chạy được nhưng không ghi analytics

5. Click **Create Web Service**

---

### 1.4. Analytics Service (Background Worker)

**⚠️ CHÚ Ý:** Analytics Service cần RabbitMQ. Có 2 options:

**Option 1: Tạm thời SKIP Analytics Service** (recommended)
- Analytics Service chỉ ghi logs, không ảnh hưởng core functions
- Redirect và URL shortening vẫn hoạt động bình thường

**Option 2: Dùng CloudAMQP (RabbitMQ free tier)**
1. Tạo tài khoản: https://www.cloudamqp.com
2. Tạo instance miễn phí
3. Copy AMQP URL
4. Quay lại setup Analytics Service với RabbitMQ config

---

## 🔗 BƯỚC 2: Lấy Deploy Hooks

### 2.1. Gateway Deploy Hook

1. Vào **urlshortener-gateway** service
2. Tab **Settings**
3. Scroll xuống **Deploy Hook**
4. Click **Copy**
5. Lưu lại URL (dạng: `https://api.render.com/deploy/srv-xxxxx?key=yyyyy`)

### 2.2. UrlShortener Deploy Hook

Lặp lại với service **urlshortener-service**

### 2.3. Redirect Deploy Hook

Lặp lại với service **urlshortener-redirect**

---

## 🔐 BƯỚC 3: Setup GitHub Secrets

1. Vào GitHub: https://github.com/tuandthe/URLShortener
2. **Settings** → **Secrets and variables** → **Actions**
3. **New repository secret** - Thêm 3 secrets:

```
Name: RENDER_DEPLOY_HOOK_GATEWAY
Value: https://api.render.com/deploy/srv-xxxxx?key=yyyyy

Name: RENDER_DEPLOY_HOOK_URLSHORTENER  
Value: https://api.render.com/deploy/srv-xxxxx?key=yyyyy

Name: RENDER_DEPLOY_HOOK_REDIRECT
Value: https://api.render.com/deploy/srv-xxxxx?key=yyyyy
```

---

## 🚀 BƯỚC 4: Deploy!

```powershell
cd "d:\Desktop\Mon Hoc\Onluyen\URLShortener"

git add .
git commit -m "Update appsettings.json for PostgreSQL"
git push origin main
```

**GitHub Actions sẽ tự động:**
1. Trigger workflow
2. Gọi Render deploy hooks
3. Render rebuild và deploy services

---

## ✅ BƯỚC 5: Verify Deployment

### 5.1. Check Logs trên Render

Mỗi service → Tab **Logs** → Xem có lỗi không

**Lỗi thường gặp:**
```
Failed to connect to database
```
**Fix:** Kiểm tra ConnectionStrings__DefaultConnection có đúng Internal Database URL

---

### 5.2. Test API Endpoints

```bash
# Test Gateway (thay URL thật)
curl https://urlshortener-gateway.onrender.com/

# Test Create Short URL
curl -X POST https://urlshortener-gateway.onrender.com/api/urls \
  -H "Content-Type: application/json" \
  -d "{\"longUrl\": \"https://google.com\"}"

# Response sẽ trả về shortCode, ví dụ: "abc123"

# Test Redirect (thay SHORT_CODE)
curl -L https://urlshortener-redirect.onrender.com/abc123
```

---

## 📊 Summary

✅ **Services đã tạo:**
- [ ] Gateway - `urlshortener-gateway.onrender.com`
- [ ] UrlShortener - `urlshortener-service.onrender.com`
- [ ] Redirect - `urlshortener-redirect.onrender.com`
- [ ] Database - `urlshortener-db` (PostgreSQL)

✅ **GitHub Secrets:**
- [ ] RENDER_DEPLOY_HOOK_GATEWAY
- [ ] RENDER_DEPLOY_HOOK_URLSHORTENER
- [ ] RENDER_DEPLOY_HOOK_REDIRECT

✅ **Environment Variables (mỗi service):**
- [ ] ASPNETCORE_ENVIRONMENT=Production
- [ ] ASPNETCORE_URLS=http://+:$PORT
- [ ] ConnectionStrings__DefaultConnection=<Internal DB URL>
- [ ] BaseUrl=<Gateway URL> (chỉ UrlShortener service)

---

## 🐛 Troubleshooting

### Lỗi: "Failed to connect to database"

**Check:**
1. Environment variable `ConnectionStrings__DefaultConnection` có đúng không
2. Phải dùng **Internal Database URL**, không phải External
3. Format phải là: `postgresql://user:pass@host/database`

### Lỗi: "Build failed" 

**Check:**
1. Dockerfile path đúng chưa: `src/Services/.../Dockerfile`
2. GitHub repo đã push code mới chưa
3. Xem Render Logs để biết chi tiết

### Service "Sleeping"

- Render free tier sleep sau 15 phút không dùng
- Lần đầu truy cập sẽ mất ~1-2 phút wake up
- **Không phải lỗi!**

---

## 💰 Chi phí

```
PostgreSQL Database: FREE (1GB)
3x Web Services: FREE (750 giờ/tháng/service)
Total: $0/month
```

---

**Happy Deploying! 🎉**
