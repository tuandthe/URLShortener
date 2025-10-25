# 🔄 Migration từ MySQL sang PostgreSQL

## ✅ Đã hoàn thành

### 1. **Cập nhật NuGet Packages** 

**Xóa:**
- ❌ `Pomelo.EntityFrameworkCore.MySql` (version 9.0.0)

**Thêm:**
- ✅ `Npgsql.EntityFrameworkCore.PostgreSQL` (version 8.0.0)

**Downgrade để tương thích:**
- ✅ `Microsoft.EntityFrameworkCore.Design` từ 9.0.10 → 8.0.0 (match với .NET 8)

**Files thay đổi:**
- `src/Services/UrlShortenerService/URLShortener.UrlShortenerService.csproj`
- `src/Services/RedirectService/URLShortener.RedirectService.csproj`
- `src/Services/AnalyticsService/URLShortener.AnalyticsService.csproj`

---

### 2. **Cập nhật Program.cs**

**Thay đổi từ MySQL:**
```csharp
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
```

**Sang PostgreSQL:**
```csharp
options.UseNpgsql(connectionString)
```

**Files thay đổi:**
- `src/Services/UrlShortenerService/Program.cs`
- `src/Services/RedirectService/Program.cs`
- `src/Services/AnalyticsService/Program.cs`

---

### 3. **Cập nhật docker-compose.yml**

**Thay đổi service từ MySQL:**
```yaml
mysql:
  image: mysql:8.0
  environment:
    MYSQL_ROOT_PASSWORD: Tu@123456789
    MYSQL_DATABASE: urlshortener
  ports:
    - "3308:3306"
```

**Sang PostgreSQL:**
```yaml
postgres:
  image: postgres:16-alpine
  environment:
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: Tu@123456789
    POSTGRES_DB: urlshortener
  ports:
    - "5433:5432"
```

**Connection strings thay đổi:**

**Trước (MySQL):**
```
Server=mysql;Port=3306;Database=urlshortener;User=root;Password=Tu@123456789;
```

**Sau (PostgreSQL):**
```
Host=postgres;Port=5432;Database=urlshortener;Username=postgres;Password=Tu@123456789;
```

**Volume thay đổi:**
- `mysql_data` → `postgres_data`

---

### 4. **Tạo lại Migrations cho PostgreSQL**

Đã tạo migrations mới cho tất cả 3 services:

✅ **UrlShortenerService:**
```bash
dotnet ef migrations add InitialCreatePostgreSQL --context UrlShortenerDbContext
```

✅ **RedirectService:**
```bash
dotnet ef migrations add InitialCreatePostgreSQL --context RedirectDbContext
```

✅ **AnalyticsService:**
```bash
dotnet ef migrations add InitialCreatePostgreSQL --context AnalyticsDbContext
```

---

## 📋 Sự khác biệt MySQL vs PostgreSQL

| Tính năng | MySQL | PostgreSQL |
|-----------|-------|------------|
| **Port mặc định** | 3306 | 5432 |
| **Connection string** | `Server=...;User=...` | `Host=...;Username=...` |
| **Image Docker** | `mysql:8.0` | `postgres:16-alpine` |
| **EF Core Provider** | Pomelo.EntityFrameworkCore.MySql | Npgsql.EntityFrameworkCore.PostgreSQL |
| **Health check** | `mysqladmin ping` | `pg_isready` |
| **Volume path** | `/var/lib/mysql` | `/var/lib/postgresql/data` |

---

## 🚀 Cách chạy với PostgreSQL

### Local Development với Docker Compose:

```bash
cd "d:\Desktop\Mon Hoc\Onluyen\URLShortener"

# Start tất cả services
docker-compose up -d

# Check logs
docker-compose logs -f postgres

# Apply migrations (tự động khi service start)
```

### Deploy lên Render.com:

1. ✅ Tạo **PostgreSQL Database** trên Render (FREE tier)
2. ✅ Copy **Internal Database URL** 
3. ✅ Paste vào Environment Variables của mỗi service:
   ```
   ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/db
   ```
4. ✅ Deploy!

---

## 🔧 Test Connection String

### PostgreSQL Format:

**Standard:**
```
Host=localhost;Port=5432;Database=urlshortener;Username=postgres;Password=Tu@123456789;
```

**URI Format (Render.com):**
```
postgresql://postgres:Tu@123456789@localhost:5432/urlshortener
```

**Convert URI → Standard trong code (optional):**
```csharp
var builder = new Npgsql.NpgsqlConnectionStringBuilder(uriConnectionString);
var standardConnectionString = builder.ToString();
```

---

## ⚠️ Lưu ý quan trọng

1. **PostgreSQL case-sensitive** với table/column names
   - Migrations tự động handle việc này
   
2. **Auto-increment syntax khác:**
   - MySQL: `AUTO_INCREMENT`
   - PostgreSQL: `SERIAL` hoặc `IDENTITY`
   - EF Core tự động convert

3. **Data types khác nhau:**
   - MySQL `LONGTEXT` → PostgreSQL `TEXT`
   - MySQL `DATETIME` → PostgreSQL `TIMESTAMP`
   - EF Core tự động map

4. **Connection pooling:**
   - PostgreSQL có built-in connection pooling tốt hơn MySQL
   - Không cần config gì thêm

---

## 📊 Kết quả

✅ **Tất cả services đã được migrate sang PostgreSQL**
✅ **Docker Compose đã update**
✅ **Migrations mới đã được tạo**
✅ **Sẵn sàng deploy lên Render.com**

---

## 🎯 Next Steps

1. ✅ Test local với `docker-compose up`
2. ✅ Verify migrations apply thành công
3. ✅ Test API endpoints
4. ✅ Deploy lên Render.com
5. ✅ Update RENDER_DEPLOYMENT_GUIDE.md (đã done!)

---

**Happy Migrating! 🐘→🐘**
