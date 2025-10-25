
# URL Shortener - Microservices
# URL Shortener - Microservices# URL Shortener - Microservices

Hệ thống rút gọn URL với kiến trúc microservices: .NET 8, React, PostgreSQL, RabbitMQ.

🏗️ Kiến trúc hệ thống
┌─────────────┐
│   Frontend  │ (ReactJS)
│  Port 3000  │
└──────┬──────┘
       │
       ▼
┌─────────────┐
│ API Gateway │ (Ocelot)
│  Port 5000  │
└──────┬──────┘
       │
       ├─────────────────────────┐
       │                         │
       ▼                         ▼
┌──────────────┐        ┌───────────────┐
│UrlShortener  │        │   Redirect    │
│   Service    │        │    Service    │
│  Port 5001   │        │   Port 5002   │
└──────┬───────┘        └───────┬───────┘
       │                        │
       │                        ├────────┐
       ▼                        ▼        │
   ┌────────┐              ┌────────┐   │
   │PostgreSQL             │PostgreSQL  │
   │  DB    │              │   DB   │   │
   └────────┘              └────────┘   │
                                        │
                              ┌─────────▼────────┐
                              │    RabbitMQ      │
                              └─────────┬────────┘
                                        │
                                        ▼
                              ┌──────────────────┐
                              │    Analytics     │
                              │     Service      │
                              │   (Worker)       │
                              └──────────────────┘
## 📦 Các thành phần


### Backend Services (.NET 8)

1. **API Gateway** - https://urlshortener-gateway.onrender.com
   - Ocelot API Gateway
   - Routing và Load Balancing
   - HTTPS + CORS enabled
   - **Platform**: Render.com (FREE)

2. **URL Shortener Service** - https://urlshortener-service-lwo4.onrender.com
   - Tạo mã rút gọn link (8 ký tự)
   - Quản lý URL mappings
   - RESTful API với Swagger
   - **Platform**: Render.com (FREE)

3. **Redirect Service** - https://urlshortener-redirect.onrender.com
   - Xử lý redirect từ short code → original URL
   - Ghi nhận click events
   - Publish events tới RabbitMQ
   - **Platform**: Render.com (FREE)

4. **Analytics Service**
   - Consume click events từ RabbitMQ
   - Lưu trữ analytics data (ClickEvents table)
   - Thống kê số lượt click
   - **Platform**: Railway.app (~$0.50-1/month)

### Frontend

- **React 18** với TypeScript
- **Vite** build tool
- **Axios** cho HTTP requests
- Giao diện tạo và quản lý short URLs
- **Platform**: GitHub Pages (FREE)
- **URL**: https://tuandthe.github.io/URLShortener

### Infrastructure

- **PostgreSQL 16**: Database trên Render.com (FREE)
  - Shared database cho tất cả services
  - Tables: `UrlMappings`, `ClickEvents`
- **RabbitMQ**: CloudAMQP (FREE tier - Little Lemur)
  - Host: shark.rmq.cloudamqp.com
  - VirtualHost: mrawftdh
- **Docker**: Local development với Docker Compose
- **CI/CD**: GitHub Actions tự động deploy

## 🎯 Deployment Flow

```
Push to GitHub main branch
         ↓
GitHub Actions triggered
         ↓
    ┌────┴────┐
    ▼         ▼
Backend    Frontend
(Render)   (Build & Deploy)
         ↓
    GitHub Pages
         ↓
✅ Live!
```

## 🛠️ Tech Stack

### Backend
- **.NET 8**: Web API framework
- **Ocelot**: API Gateway  
- **Entity Framework Core 8**: ORM
- **Npgsql**: PostgreSQL provider
- **RabbitMQ.Client**: Message broker client
- **Swagger/OpenAPI**: API documentation

### Frontend
- **React 18**: UI library
- **TypeScript**: Type-safe JavaScript
- **Vite**: Build tool & dev server
- **Axios**: HTTP client
- **React Router**: SPA routing

### Infrastructure & Deployment
- **PostgreSQL 16**: Relational database (Render.com)
- **RabbitMQ**: Message broker (CloudAMQP)
- **Docker**: Containerization (local dev)
- **GitHub Actions**: CI/CD pipeline
- **Render.com**: Backend hosting (FREE tier)
- **Railway.app**: Analytics worker (~$1/month)
- **GitHub Pages**: Frontend hosting (FREE)


## 📂 Project Structure

```
URLShortener/
├── .github/
│   └── workflows/
│       └── deploy-render.yml       # CI/CD: Backend (Render) + Frontend (GitHub Pages)
├── src/
│   ├── Frontend/                   # React + TypeScript + Vite
│   │   ├── src/
│   │   ├── public/
│   │   ├── Dockerfile
│   │   └── package.json
│   ├── Gateway/                    # Ocelot API Gateway
│   │   ├── ocelot.json            # Route configuration
│   │   ├── Program.cs
│   │   └── Dockerfile
│   ├── Services/
│   │   ├── UrlShortenerService/   # URL creation & management
│   │   │   ├── Controllers/
│   │   │   ├── Data/
│   │   │   ├── Migrations/
│   │   │   └── Dockerfile
│   │   ├── RedirectService/       # URL redirection + RabbitMQ publisher
│   │   │   ├── Controllers/
│   │   │   ├── Data/
│   │   │   ├── Services/
│   │   │   └── Dockerfile
│   │   └── AnalyticsService/      # Background worker
│   │       ├── Worker.cs
│   │       ├── Data/
│   │       └── Dockerfile
│   └── Shared/                    # Shared DTOs, Models, Messages
│       ├── DTOs/
│       ├── Models/
│       └── Messages/
├── docker-compose.yml
├── railway.toml                   # Railway.app config for Analytics
└── README.md
```

## 🔧 Development

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Docker Desktop
- PostgreSQL 16 (or use Docker)
- RabbitMQ (or use CloudAMQP)


