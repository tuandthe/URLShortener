# HƯỚNG DẪN TRIỂN KHAI HỆ THỐNG URL SHORTENER

## 📋 Mục lục
1. [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
2. [Cài đặt môi trường Development](#cài-đặt-môi-trường-development)
3. [Triển khai với Docker](#triển-khai-với-docker)
4. [Cấu hình CI/CD](#cấu-hình-cicd)
5. [Kiểm tra và Test](#kiểm-tra-và-test)
6. [Troubleshooting](#troubleshooting)

## 🎯 Yêu cầu hệ thống

### Phần mềm cần thiết
- Docker Desktop 20.10+ hoặc Docker Engine
- Docker Compose 2.0+
- .NET 9 SDK (cho development)
- Node.js 18+ (cho development frontend)
- Git

### Cấu hình tối thiểu
- RAM: 4GB+
- Disk: 10GB+ free space
- CPU: 2 cores+

## 🛠️ Cài đặt môi trường Development

### 1. Clone Repository

```bash
git clone https://github.com/yourusername/URLShortener.git
cd URLShortener
```

### 2. Build Backend (.NET)

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Verify build
dotnet test
```

### 3. Setup Frontend

```bash
cd src/Frontend

# Install dependencies
npm install

# Start development server
npm start
```

Frontend sẽ chạy tại: http://localhost:3000

### 4. Run Backend Services Locally

#### UrlShortenerService
```bash
cd src/Services/UrlShortenerService
dotnet run
```
Service chạy tại: http://localhost:5001

#### RedirectService
```bash
cd src/Services/RedirectService
dotnet run
```
Service chạy tại: http://localhost:5002

#### AnalyticsService
```bash
cd src/Services/AnalyticsService
dotnet run
```
Background worker (không có port)

#### API Gateway
```bash
cd src/Gateway
dotnet run
```
Gateway chạy tại: http://localhost:5000

### 5. Setup Database và RabbitMQ

```bash
# Chạy chỉ MySQL và RabbitMQ
docker-compose up -d mysql rabbitmq
```

## 🐳 Triển khai với Docker

### 1. Build tất cả Docker Images

```bash
# Ở thư mục root
docker-compose build
```

### 2. Chạy toàn bộ hệ thống

```bash
# Start all services
docker-compose up -d

# Xem logs
docker-compose logs -f

# Xem logs của một service cụ thể
docker-compose logs -f urlshortener-service
```

### 3. Kiểm tra trạng thái

```bash
# Kiểm tra containers
docker-compose ps

# Health check
curl http://localhost:5000/api/health
```

### 4. Stop và Clean up

```bash
# Stop all services
docker-compose down

# Stop và xóa volumes
docker-compose down -v

# Clean up unused resources
docker system prune -f
```

## 🚀 Cấu hình CI/CD

### 1. Setup GitHub Secrets

Vào Settings → Secrets and variables → Actions, thêm các secrets:

```
DOCKER_USERNAME=your_dockerhub_username
DOCKER_PASSWORD=your_dockerhub_password
SERVER_HOST=your_server_ip
SERVER_USERNAME=your_ssh_username
SSH_PRIVATE_KEY=your_ssh_private_key
```

### 2. Tạo SSH Key (nếu chưa có)

```bash
# Generate SSH key
ssh-keygen -t rsa -b 4096 -C "your_email@example.com"

# Copy public key to server
ssh-copy-id user@your_server_ip

# Copy private key để paste vào GitHub Secret
cat ~/.ssh/id_rsa
```

### 3. Setup Server Production

```bash
# SSH vào server
ssh user@your_server_ip

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/download/v2.20.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Clone repository
git clone https://github.com/yourusername/URLShortener.git
cd URLShortener

# Create production environment file
cp .env.example .env.production
```

### 4. Workflow Triggers

**CI Workflow** (`ci.yml`) - Chạy khi:
- Push to `main`
- Push to `feature/**` branches
- Pull request to `main`

**CD Workflow** (`cd.yml`) - Chạy khi:
- Push to `main` branch only

## ✅ Kiểm tra và Test

### 1. API Testing

```bash
# Health check
curl http://localhost:5000/api/health

# Tạo URL rút gọn
curl -X POST http://localhost:5000/api/shorten \
  -H "Content-Type: application/json" \
  -d '{"originalUrl":"https://www.google.com"}'

# Test redirect (thay abc123 bằng shortCode nhận được)
curl -I http://localhost:5000/abc123
```

### 2. Frontend Testing

1. Mở browser: http://localhost:3000
2. Nhập URL: `https://www.google.com`
3. Click "Rút gọn"
4. Kiểm tra URL rút gọn
5. Click vào URL để test redirect

### 3. Database Verification

```bash
# Access MySQL
docker exec -it urlshortener-mysql mysql -uroot -prootpassword

# Queries
USE urlshortener;
SELECT * FROM UrlMappings;
SELECT * FROM ClickEvents;
```

### 4. RabbitMQ Monitoring

- Truy cập: http://localhost:15672
- Username: `guest`
- Password: `guest`
- Kiểm tra queue `click_events`

## 🔧 Troubleshooting

### Lỗi: Port đã được sử dụng

```bash
# Kiểm tra port đang sử dụng
netstat -ano | findstr :5000

# Kill process (Windows)
taskkill /PID <PID> /F

# Kill process (Linux/Mac)
kill -9 <PID>
```

### Lỗi: Cannot connect to MySQL

```bash
# Restart MySQL container
docker-compose restart mysql

# Check logs
docker-compose logs mysql

# Verify connection
docker exec -it urlshortener-mysql mysqladmin ping -h localhost
```

### Lỗi: RabbitMQ connection failed

```bash
# Restart RabbitMQ
docker-compose restart rabbitmq

# Check status
docker-compose logs rabbitmq

# Wait for RabbitMQ to be ready (30-60 seconds)
```

### Lỗi: Docker build failed

```bash
# Clear Docker cache
docker builder prune -a

# Rebuild without cache
docker-compose build --no-cache
```

### Lỗi: Frontend không kết nối được API

1. Kiểm tra `.env` file trong `src/Frontend`
2. Đảm bảo `REACT_APP_API_URL=http://localhost:5000`
3. Restart frontend: `npm start`
4. Clear browser cache

### Lỗi: Migration failed

```bash
# Manual migration (UrlShortenerService)
cd src/Services/UrlShortenerService
dotnet ef database update

# Manual migration (RedirectService)
cd src/Services/RedirectService
dotnet ef database update

# Manual migration (AnalyticsService)
cd src/Services/AnalyticsService
dotnet ef database update
```

## 📊 Monitoring và Logs

### View Service Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f urlshortener-service
docker-compose logs -f redirect-service
docker-compose logs -f analytics-service
docker-compose logs -f api-gateway
docker-compose logs -f frontend

# Last 100 lines
docker-compose logs --tail=100 urlshortener-service
```

### Monitor Resource Usage

```bash
# Docker stats
docker stats

# Specific container
docker stats urlshortener-service
```

## 🔐 Security Checklist

- [ ] Đổi password MySQL mặc định
- [ ] Đổi credentials RabbitMQ
- [ ] Cấu hình HTTPS cho production
- [ ] Enable firewall rules
- [ ] Cấu hình rate limiting
- [ ] Setup monitoring và alerting
- [ ] Backup database định kỳ
- [ ] Review logs thường xuyên

## 📞 Support

Nếu gặp vấn đề:
1. Kiểm tra [Troubleshooting](#troubleshooting)
2. Xem logs: `docker-compose logs`
3. Tạo issue trên GitHub
4. Liên hệ team support

## 🎉 Success Criteria

Hệ thống triển khai thành công khi:
- ✅ All containers running: `docker-compose ps`
- ✅ Frontend accessible: http://localhost:3000
- ✅ API Gateway accessible: http://localhost:5000
- ✅ Can create short URL
- ✅ Can redirect from short URL
- ✅ Click events saved to database
- ✅ RabbitMQ processing messages
- ✅ All health checks passing

---

**Chúc bạn triển khai thành công! 🚀**
