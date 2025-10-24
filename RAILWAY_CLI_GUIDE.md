# 🚂 Railway CLI - Manual Deployment Guide

Hướng dẫn deploy thủ công bằng Railway CLI (không dùng GitHub Actions).

## 📋 Prerequisites

- Node.js đã cài đặt
- Tài khoản Railway
- Project đã setup trên Railway (MySQL, RabbitMQ, 4 services)

---

## 🔧 Setup Railway CLI

### Bước 1: Cài đặt Railway CLI

```powershell
# PowerShell (Windows)
npm install -g @railway/cli
```

```bash
# Bash (Linux/Mac)
npm install -g @railway/cli
```

### Bước 2: Login vào Railway

```bash
railway login
```

Lệnh này sẽ:
- Mở browser để bạn login
- Tạo authentication token
- Lưu token vào máy local

### Bước 3: Link project

**Cách 1: Link bằng Project ID**
```bash
railway link [PROJECT_ID]
```

**Cách 2: Link interactively**
```bash
railway link
```
Chọn project từ danh sách

---

## 🚀 Deploy Commands

### Deploy từng service riêng:

#### 1. Deploy Gateway
```bash
railway up --service gateway --dockerfile src/Gateway/Dockerfile
```

#### 2. Deploy UrlShortener Service
```bash
railway up --service urlshortener-service --dockerfile src/Services/UrlShortenerService/Dockerfile
```

#### 3. Deploy Redirect Service
```bash
railway up --service redirect-service --dockerfile src/Services/RedirectService/Dockerfile
```

#### 4. Deploy Analytics Service
```bash
railway up --service analytics-service --dockerfile src/Services/AnalyticsService/Dockerfile
```

---

## 📜 Deploy tất cả bằng script

### Windows (PowerShell):

```powershell
# Chạy script
.\deploy-railway.ps1
```

**Nếu gặp lỗi execution policy:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
.\deploy-railway.ps1
```

### Linux/Mac (Bash):

```bash
# Cho phép execute
chmod +x deploy-railway.sh

# Chạy script
./deploy-railway.sh
```

---

## 🔍 Các lệnh Railway CLI hữu ích

### Xem logs
```bash
# Logs của service cụ thể
railway logs --service gateway

# Follow logs (real-time)
railway logs --service gateway --follow
```

### Xem danh sách services
```bash
railway status
```

### Xem environment variables
```bash
railway variables
```

### Set environment variable
```bash
railway variables set KEY=VALUE --service gateway
```

### Unlink project
```bash
railway unlink
```

### Xem thông tin project
```bash
railway environment
```

---

## 🎯 Deploy Flow thủ công

```
1. Setup Railway Project trên Web UI
   ├── Tạo MySQL
   ├── Tạo RabbitMQ
   └── Tạo 4 Empty Services

2. Install Railway CLI
   └── npm install -g @railway/cli

3. Login & Link
   ├── railway login
   └── railway link [PROJECT_ID]

4. Deploy từng service
   ├── railway up --service gateway --dockerfile src/Gateway/Dockerfile
   ├── railway up --service urlshortener-service --dockerfile src/Services/UrlShortenerService/Dockerfile
   ├── railway up --service redirect-service --dockerfile src/Services/RedirectService/Dockerfile
   └── railway up --service analytics-service --dockerfile src/Services/AnalyticsService/Dockerfile

5. Verify
   └── Check Railway Dashboard
```

---

## 🐛 Troubleshooting

### ❌ Error: "railway: command not found"
**Solution:**
```bash
# Kiểm tra npm global path
npm config get prefix

# Thêm vào PATH (Windows)
$env:Path += ";C:\Users\[YOUR_USERNAME]\AppData\Roaming\npm"

# Thêm vào PATH (Linux/Mac)
export PATH="$PATH:$(npm config get prefix)/bin"
```

### ❌ Error: "Not logged in"
**Solution:**
```bash
railway login
```

### ❌ Error: "No project linked"
**Solution:**
```bash
railway link [PROJECT_ID]
```

### ❌ Error: "Service not found"
**Solution:**
- Kiểm tra tên service trên Railway Dashboard
- Tên phải khớp chính xác (case-sensitive)

### ❌ Error: "Dockerfile not found"
**Solution:**
- Kiểm tra path đến Dockerfile
- Phải chạy lệnh từ root của repository

---

## 💡 Tips

### 1. Deploy nhanh hơn
Thêm flag `--detach` để không đợi build hoàn tất:
```bash
railway up --service gateway --dockerfile src/Gateway/Dockerfile --detach
```

### 2. Deploy từ branch khác
```bash
railway up --service gateway --dockerfile src/Gateway/Dockerfile --branch develop
```

### 3. Xem build logs
```bash
railway logs --deployment [DEPLOYMENT_ID]
```

### 4. Set nhiều variables một lúc
```bash
railway variables set \
  ASPNETCORE_ENVIRONMENT=Production \
  ASPNETCORE_URLS=http://+:8080 \
  --service gateway
```

---

## 📊 So sánh: Manual vs GitHub Actions

| Feature | Manual CLI | GitHub Actions |
|---------|-----------|----------------|
| **Setup** | Dễ, nhanh | Cần config workflow |
| **Deploy** | Phải chạy thủ công | Tự động khi push |
| **Speed** | Deploy ngay | Đợi GitHub queue |
| **Control** | Full control | Automated |
| **Best for** | Testing, Debug | Production, CI/CD |

---

## 🎯 Recommended Workflow

### Development:
```bash
# Deploy thủ công để test
railway up --service gateway --dockerfile src/Gateway/Dockerfile
```

### Production:
```bash
# Setup GitHub Actions để auto-deploy
git push origin master
```

---

## 📚 Railway CLI Reference

Xem full documentation:
```bash
railway help
```

Hoặc truy cập: https://docs.railway.app/develop/cli

---

**Happy Deploying! 🚂✨**
