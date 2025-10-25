# URL Shortener - Microservice Architecture

Hệ thống rút gọn link hoàn chỉnh được xây dựng theo kiến trúc microservice với .NET 8, ReactJS, PostgreSQL và RabbitMQ.

## 🏗️ Kiến trúc hệ thống

```
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
```

## 📦 Các thành phần

### Backend Services (.NET 8)

1. **API Gateway** (Port 5000)
   - Ocelot API Gateway
   - Routing và Load Balancing
   - Authentication & Authorization

2. **URL Shortener Service** (Port 5001)
   - Tạo mã rút gọn link
   - Quản lý URL mappings
   - RESTful API

3. **Redirect Service** (Port 5002)
   - Xử lý redirect từ short code → original URL
   - Ghi nhận click events
   - Publish events tới RabbitMQ

4. **Analytics Service** (Background Worker)
   - Consume click events từ RabbitMQ
   - Lưu trữ analytics data
   - Thống kê số lượt click

### Frontend

- **React** với TypeScript
- **Vite** build tool
- **Axios** cho HTTP requests
- Giao diện tạo và quản lý short URLs

### Infrastructure

- **PostgreSQL**: Database cho URL mappings và analytics
- **RabbitMQ**: Message broker cho event-driven architecture (optional với CloudAMQP)
- **Docker & Docker Compose**: Containerization
- **Render.com**: Cloud deployment platform (FREE!)

## 🚀 Deploy lên Render.com với GitHub Actions

### Quick Start

1. ✅ Fork/Clone repository này
2. ✅ Tạo tài khoản miễn phí tại [Render.com](https://render.com)
3. ✅ Tạo PostgreSQL database trên Render
4. ✅ Tạo 4 Web Services + 1 Static Site
5. ✅ Setup GitHub Secrets với Deploy Hooks
6. ✅ Push code → Auto deploy!

### 📖 Hướng dẫn chi tiết

Xem file **[RENDER_DEPLOYMENT_GUIDE.md](./RENDER_DEPLOYMENT_GUIDE.md)** để có hướng dẫn từng bước chi tiết!

### 🎯 Deployment Flow

```
Push to GitHub main branch
         ↓
GitHub Actions triggered
         ↓
Trigger Render Deploy Hooks
         ↓
Render rebuilds & deploys services
         ↓
✅ Live on Render!
```

## 🛠️ Tech Stack

### Backend
- **.NET 8**: Web API framework
- **Ocelot**: API Gateway
- **Entity Framework Core**: ORM
- **PostgreSQL**: Relational database
- **RabbitMQ/CloudAMQP**: Message broker

### Frontend
- **React 18**: UI library
- **TypeScript**: Type-safe JavaScript
- **Vite**: Build tool
- **Axios**: HTTP client

### DevOps
- **Docker**: Containerization
- **GitHub Actions**: CI/CD
- **Render.com**: Cloud platform (FREE tier available)

## 📝 API Endpoints

### URL Shortener Service (`/api/urls`)

```bash
# Tạo short URL
POST /api/urls
Body: { "longUrl": "https://example.com" }

# Lấy danh sách URLs
GET /api/urls

# Lấy URL theo ID
GET /api/urls/{id}

# Xóa URL
DELETE /api/urls/{id}
```

### Redirect Service

```bash
# Redirect từ short code
GET /{shortCode}
```

## 🏃 Run Locally với Docker Compose

```bash
# Build và start tất cả services
docker-compose up --build

# Stop tất cả services
docker-compose down

# Xem logs
docker-compose logs -f [service-name]
```

Services sẽ chạy tại:
- Frontend: http://localhost:3000
- API Gateway: http://localhost:5000
- URL Shortener: http://localhost:5001
- Redirect Service: http://localhost:5002
- MySQL: localhost:3308
- RabbitMQ Management: http://localhost:15672

## 📂 Project Structure

```
URLShortener/
├── src/
│   ├── Frontend/              # React app
│   ├── Gateway/               # Ocelot API Gateway
│   ├── Services/
│   │   ├── UrlShortenerService/
│   │   ├── RedirectService/
│   │   └── AnalyticsService/
│   └── Shared/                # Shared DTOs, Models
├── .github/
│   └── workflows/
│       └── deploy-railway.yml # GitHub Actions workflow
├── docker-compose.yml
└── README.md
```

## 🔧 Development

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Docker Desktop
- MySQL
- RabbitMQ

### Backend Development

```bash
# Restore dependencies
dotnet restore

# Run migrations
dotnet ef database update --project src/Services/UrlShortenerService

# Run service
dotnet run --project src/Services/UrlShortenerService
```

### Frontend Development

```bash
cd src/Frontend
npm install
npm run dev
```

## 📊 Features

- ✅ Tạo mã rút gọn link tự động
- ✅ Redirect nhanh từ short code
- ✅ Tracking số lượt click
- ✅ Analytics dashboard
- ✅ RESTful API
- ✅ Event-driven architecture
- ✅ Microservices architecture
- ✅ Docker containerization
- ✅ CI/CD với GitHub Actions
- ✅ Cloud deployment trên Railway

## 🤝 Contributing

Contributions, issues và feature requests đều được welcome!

## 📄 License

This project is [MIT](LICENSE) licensed.

## 👨‍💻 Author

**tuandthe**

- GitHub: [@tuandthe](https://github.com/tuandthe)
- Repository: [URLShortener](https://github.com/tuandthe/URLShortener)

---

**Happy Coding! 🚀**

1. **API Gateway** - Điểm vào duy nhất cho hệ thống
   - Framework: ASP.NET Core với Ocelot
   - Port: 5000
   - Chức năng: Routing, Load balancing

2. **UrlShortenerService** - Tạo link rút gọn
   - Port: 5001
   - Database: MySQL
   - Endpoint: `POST /api/urls`

3. **RedirectService** - Điều hướng từ link ngắn
   - Port: 5002
   - Database: MySQL
   - Message Broker: RabbitMQ
   - Endpoint: `GET /{shortCode}`

4. **AnalyticsService** - Xử lý dữ liệu phân tích
   - Type: Worker Service (Background)
   - Database: MySQL
   - Message Broker: RabbitMQ

### Frontend
- Framework: ReactJS + TypeScript
- Port: 3000
- Features: 
  - Giao diện đơn giản, thân thiện
  - Responsive design
  - Real-time validation

### Infrastructure
- **Database**: MySQL 8.0
- **Message Broker**: RabbitMQ
- **Container**: Docker & Docker Compose
- **CI/CD**: GitHub Actions

## 🚀 Cài đặt và chạy

### Yêu cầu
- Docker & Docker Compose
- .NET 9 SDK (cho development)
- Node.js 18+ (cho development)

### Chạy toàn bộ hệ thống với Docker Compose

```bash
# Clone repository
git clone <repository-url>
cd URLShortener

# Build và chạy tất cả services
docker-compose up -d --build

# Xem logs
docker-compose logs -f

# Dừng hệ thống
docker-compose down

# Dừng và xóa volumes
docker-compose down -v
```

### Các URL truy cập

- Frontend: http://localhost:3000
- API Gateway: http://localhost:5000
- RabbitMQ Management: http://localhost:15672 (guest/guest)

## 🛠️ Development

### Backend

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run specific service
cd src/Services/UrlShortenerService
dotnet run

# Run tests
dotnet test
```

### Frontend

```bash
cd src/Frontend

# Install dependencies
npm install

# Run development server
npm start

# Build for production
npm run build
```

## 📊 Database Schema

### UrlMappings
```sql
CREATE TABLE UrlMappings (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    OriginalUrl TEXT NOT NULL,
    ShortCode VARCHAR(8) UNIQUE NOT NULL,
    CreatedAt DATETIME NOT NULL,
    INDEX IX_UrlMappings_ShortCode (ShortCode)
);
```

### ClickEvents
```sql
CREATE TABLE ClickEvents (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    ShortCode VARCHAR(8) NOT NULL,
    Timestamp DATETIME NOT NULL,
    UserAgent VARCHAR(500),
    IpAddress VARCHAR(45),
    INDEX IX_ClickEvents_ShortCode (ShortCode),
    INDEX IX_ClickEvents_Timestamp (Timestamp)
);
```

## 🔄 CI/CD Pipeline

### Continuous Integration (CI)
- Trigger: Push to `main` hoặc `feature/**` branches
- Steps:
  1. Build all .NET projects
  2. Run unit tests
  3. Upload test results

### Continuous Deployment (CD)
- Trigger: Push to `main` branch
- Steps:
  1. Build Docker images
  2. Push to Docker Hub
  3. Deploy to production server via SSH

### GitHub Secrets cần thiết
```
DOCKER_USERNAME
DOCKER_PASSWORD
SERVER_HOST
SERVER_USERNAME
SSH_PRIVATE_KEY
```

## 🎯 API Endpoints

### API Gateway

#### Tạo URL rút gọn
```http
POST /api/shorten
Content-Type: application/json

{
  "originalUrl": "https://example.com/very/long/url"
}

Response: 201 Created
{
  "shortUrl": "http://localhost:5000/abc123",
  "shortCode": "abc123",
  "originalUrl": "https://example.com/very/long/url",
  "createdAt": "2025-10-22T12:00:00Z"
}
```

#### Redirect
```http
GET /{shortCode}

Response: 302 Found
Location: https://example.com/very/long/url
```

## 📝 Nguyên tắc thiết kế

- **Clean Architecture**: Tách biệt concerns, dễ maintain
- **SOLID Principles**: Code dễ đọc, dễ mở rộng
- **Async/Await**: Tất cả I/O operations đều async
- **RESTful API**: Tuân thủ chuẩn REST
- **Microservices**: Loosely coupled, independently deployable
- **Event-Driven**: Communication qua RabbitMQ
- **Docker**: Containerization cho tất cả services

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## 📈 Monitoring

- RabbitMQ Management UI: http://localhost:15672
- Database: Kết nối qua MySQL Workbench (localhost:3306)

## 🔒 Security

- CORS enabled cho development
- Input validation
- SQL Injection protection (EF Core)
- XSS protection
- Security headers trong Nginx

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👨‍💻 Author

URL Shortener Microservice System - 2025
