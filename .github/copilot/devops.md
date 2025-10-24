# Hướng dẫn cho GitHub Actions

## Quy trình CI (Continuous Integration)
- **Tên file:** `ci.yml`.
- **Trigger:** Kích hoạt khi có `push` hoặc `pull_request` đến nhánh `main` và các nhánh `feature/*`.
- **Các bước chính:**
  1. Checkout code.
  2. Cài đặt môi trường .NET và Node.js.
  3. Build tất cả các service backend.
  4. Chạy unit test cho backend (`dotnet test`).
  5. Build ứng dụng frontend.
  6. (Tùy chọn) Chạy test cho frontend.

## Quy trình CD (Continuous Deployment)
- **Tên file:** `cd.yml`.
- **Trigger:** Kích hoạt chỉ khi có `push` vào nhánh `main` và sau khi quy trình CI đã chạy thành công.
- **Sử dụng Secrets:** Luôn sử dụng GitHub Secrets để lưu các thông tin nhạy cảm (DOCKER_USERNAME, DOCKER_TOKEN, SSH_PRIVATE_KEY, SERVER_HOST).
- **Các bước chính:**
  1. Checkout code.
  2. Đăng nhập vào Docker Hub.
  3. Build và push Docker image cho từng Clean architech và frontend. Tag image bằng commit SHA để đảm bảo tính duy nhất.
  4. Sử dụng SSH để kết nối vào server deploy.
  5. Trên server, thực hiện lệnh `git pull` để lấy code mới nhất (bao gồm file docker-compose.yml đã cập nhật).
  6. Chạy `docker-compose up -d --build` để khởi động lại hệ thống với các image mới nhất.