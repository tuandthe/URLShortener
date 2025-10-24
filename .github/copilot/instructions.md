# Instructions cho dự án URL Shortener

## Về dự án này
Đây là một hệ thống rút gọn link được xây dựng trên kiến trúc microservice. Mục tiêu là tạo ra một hệ thống có hiệu năng cao, dễ bảo trì và mở rộng. Luôn phản hồi bằng tiếng Việt.

##  Kiến trúc tổng quan
- **Frontend:** ReactJS
- **Backend:** C# với .NET, chia thành nhiều microservice.
- **API Gateway:** Ocelot đóng vai trò là cổng vào duy nhất, xử lý routing và các tác vụ chung.
- **Các Microservice:**
  - `UrlShortenerService`: Xử lý logic tạo link ngắn.
  - `RedirectService`: Xử lý điều hướng và ghi nhận sự kiện click.
  - `AnalyticsService`: Xử lý phân tích dữ liệu từ các sự kiện.
- **Database:** MySQL.
- **Giao tiếp bất đồng bộ:** RabbitMQ được sử dụng để `RedirectService` gửi message cho `AnalyticsService`.
- **Triển khai:** Toàn bộ hệ thống được đóng gói bằng Docker và điều phối bằng Docker Compose.
- **CI/CD:** Sử dụng GitHub Actions để tự động hóa quy trình build, test và deploy.

## Nguyên tắc cốt lõi
1.  **Clean Code:** Mã nguồn phải dễ đọc, dễ hiểu. Ưu tiên tên biến, hàm rõ ràng.
2.  **SOLID:** Tuân thủ các nguyên tắc SOLID trong thiết kế.
3.  **DRY (Don't Repeat Yourself):** Tránh lặp lại code. Tái sử dụng logic thông qua các hàm, class hoặc service chung.
4.  **Bất đồng bộ:** Sử dụng `async/await` cho tất cả các hoạt động I/O (gọi API, truy vấn database).
5.  **Ghi log (Logging):** Sử dụng thư viện logging có cấu trúc (structured logging) như Serilog cho backend để dễ dàng theo dõi và gỡ lỗi.