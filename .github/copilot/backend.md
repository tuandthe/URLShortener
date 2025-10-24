# Hướng dẫn cho Backend C# (.NET)

## Thiết kế và Kiến trúc
1.  **Clean Architecture:** Áp dụng các nguyên tắc của Clean Architecture hoặc Vertical Slice Architecture. Tách biệt rõ ràng các lớp: Domain, Application, Infrastructure và Presentation (API).
2.  **Dependency Injection (DI):** Sử dụng DI một cách triệt để. Mọi service, repository, và các dependency khác phải được đăng ký trong DI container và được inject vào constructor.
3.  **Result Pattern:** Sử dụng Result Pattern thay vì ném exception cho các lỗi nghiệp vụ (business logic errors) để làm cho luồng điều khiển rõ ràng hơn.

## ASP.NET Core Web API
1.  **Controller tinh gọn:** Giữ cho Controller tinh gọn. Logic nghiệp vụ phải nằm trong Application layer (ví dụ: trong các Command/Query Handler của MediatR).
2.  **DTOs (Data Transfer Objects):** Luôn sử dụng DTOs để nhận request và trả về response. Không bao giờ để lộ các đối tượng Domain (Entities) ra ngoài API.
3.  **Sử dụng MediatR:** Khuyến khích sử dụng thư viện MediatR để triển khai CQRS (Command Query Responsibility Segregation), giúp tách biệt logic đọc và ghi.

## Tương tác Database
- Sử dụng **Entity Framework Core** làm ORM chính.
- Áp dụng **Repository Pattern** để trừu tượng hóa việc truy cập dữ liệu.
- Tất cả các truy vấn database phải là bất đồng bộ (sử dụng các phương thức `...Async`).

## Giao tiếp với RabbitMQ
- Sử dụng thư viện `MassTransit` để đơn giản hóa việc publish và consume message từ RabbitMQ. Nó cung cấp các abstraction mạnh mẽ và tích hợp tốt với DI.