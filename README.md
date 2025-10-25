# URL Shortener - Microservices

Hệ thống rút gọn URL với kiến trúc microservices: .NET 8, React, PostgreSQL, RabbitMQ.

**� Live Demo**: [Frontend](https://tuandthe.github.io/URLShortener) | [API Gateway](https://urlshortener-gateway.onrender.com)

---

## 🏗️ Architecture

```
Frontend (React) → GitHub Pages (FREE)
    ↓
API Gateway (Ocelot) → Render.com (FREE)
    ↓
    ├─→ UrlShortener Service → Render.com (FREE)
    └─→ Redirect Service → Render.com (FREE)
         ↓
         RabbitMQ → CloudAMQP (FREE)
         ↓
         Analytics Worker → Railway.app (~$1/month)
         ↓
    PostgreSQL → Render.com (FREE)
```

---

## 🚀 Quick Start

### Deploy to Production
```bash
# 1. Fork this repo
# 2. Enable GitHub Pages: Settings → Pages → Source: GitHub Actions
# 3. Add GitHub Secrets:
#    - RENDER_DEPLOY_HOOK_GATEWAY
#    - RENDER_DEPLOY_HOOK_URLSHORTENER  
#    - RENDER_DEPLOY_HOOK_REDIRECT
# 4. Push to main → Auto deploy!
```

### Run Locally
```bash
docker-compose up --build
```
- Frontend: http://localhost:3000
- Gateway: http://localhost:5000
- RabbitMQ UI: http://localhost:15672 (guest/guest)

---

## 📡 API

**Base URL**: `https://urlshortener-gateway.onrender.com`

```bash
# Create short URL
POST /api/urls
Body: { "OriginalUrl": "https://google.com" }

# Get all URLs
GET /api/urls

# Redirect
GET /{shortCode}
```

---

## 🛠️ Tech Stack

| Component | Technology | Platform |
|-----------|-----------|----------|
| Frontend | React + TypeScript + Vite | GitHub Pages (FREE) |
| Gateway | .NET 8 + Ocelot | Render.com (FREE) |
| Backend | .NET 8 + EF Core | Render.com (FREE) |
| Worker | .NET 8 + RabbitMQ | Railway.app (~$1/month) |
| Database | PostgreSQL 16 | Render.com (FREE) |
| Message Queue | RabbitMQ | CloudAMQP (FREE) |

**💰 Total Cost**: ~$1/month (Analytics worker only)

---

## 📂 Project Structure

```
├── .github/workflows/deploy-render.yml  # CI/CD
├── src/
│   ├── Frontend/                        # React app
│   ├── Gateway/                         # Ocelot API Gateway
│   ├── Services/
│   │   ├── UrlShortenerService/        # Create URLs
│   │   ├── RedirectService/            # Redirect + RabbitMQ
│   │   └── AnalyticsService/           # Background worker
│   └── Shared/                         # DTOs, Models, Messages
└── docker-compose.yml
```

---

## 🔧 Development

```bash
# Backend
dotnet restore
dotnet run --project src/Services/UrlShortenerService

# Frontend
cd src/Frontend
npm install && npm run dev

# Database migrations
cd src/Services/UrlShortenerService
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 📊 Database Schema

**UrlMappings**
```sql
CREATE TABLE "UrlMappings" (
    "Id" SERIAL PRIMARY KEY,
    "OriginalUrl" TEXT NOT NULL,
    "ShortCode" VARCHAR(8) UNIQUE NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL
);
```

**ClickEvents**
```sql
CREATE TABLE "ClickEvents" (
    "Id" SERIAL PRIMARY KEY,
    "ShortCode" VARCHAR(8) NOT NULL,
    "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UserAgent" VARCHAR(500),
    "IpAddress" VARCHAR(45)
);
```

---

## 📝 Environment Variables

**Render Services**
```bash
DATABASE_URL=postgres://...
RabbitMQ__HostName=shark.rmq.cloudamqp.com
RabbitMQ__Port=5672
RabbitMQ__UserName=mrawftdh
RabbitMQ__Password=***
RabbitMQ__VirtualHost=mrawftdh
```

**Gateway**
```bash
URL_SHORTENER_HOST=urlshortener-service-lwo4.onrender.com
REDIRECT_SERVICE_HOST=urlshortener-redirect.onrender.com
GATEWAY_BASE_URL=https://urlshortener-gateway.onrender.com
```

---

## ✨ Features

- ✅ Auto-generate 8-char short codes
- ✅ Fast redirect with PostgreSQL
- ✅ Click tracking with RabbitMQ
- ✅ Event-driven analytics
- ✅ Auto-deploy with GitHub Actions
- ✅ Swagger API docs
- ✅ Docker support

---

## 🤝 Contributing

1. Fork the project
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

---

## 📄 License

MIT License

## 👨‍💻 Author

**tuandthe** - [GitHub](https://github.com/tuandthe)

---

**Happy Coding! 🚀**


