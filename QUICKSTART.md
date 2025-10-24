# 🚀 Quick Start Guide

## Chạy hệ thống trong 3 bước

### Bước 1: Clone và setup
```bash
git clone <repository-url>
cd URLShortener
```

### Bước 2: Build và chạy với Docker
```bash
docker-compose up -d --build
```

### Bước 3: Truy cập
- **Frontend**: http://localhost:3000
- **API Gateway**: http://localhost:5000
- **RabbitMQ UI**: http://localhost:15672 (guest/guest)

## Kiểm tra nhanh

```bash
# Xem trạng thái
docker-compose ps

# Xem logs
docker-compose logs -f

# Test API
curl -X POST http://localhost:5000/api/shorten \
  -H "Content-Type: application/json" \
  -d '{"originalUrl":"https://www.google.com"}'
```

## Dừng hệ thống

```bash
docker-compose down
```

---

**Chi tiết đầy đủ:** Xem [DEPLOYMENT.md](DEPLOYMENT.md)
