# Quy tắc thiết kế RESTful API

1.  **Sử dụng danh từ số nhiều cho tài nguyên:**
    - `GET /api/v1/urls` (lấy danh sách)
    - `GET /api/v1/urls/{shortCode}` (lấy một tài nguyên)
2.  **Sử dụng đúng các phương thức HTTP:**
    - `GET`: Lấy dữ liệu.
    - `POST`: Tạo mới tài nguyên.
    - `PUT`: Cập nhật toàn bộ tài nguyên.
    - `PATCH`: Cập nhật một phần tài nguyên.
    - `DELETE`: Xóa tài nguyên.
3.  **Sử dụng mã trạng thái HTTP (Status Code) chuẩn:**
    - `200 OK`: Thành công.
    - `201 Created`: Tạo mới thành công.
    - `204 No Content`: Thành công nhưng không có nội dung trả về (dùng cho DELETE).
    - `400 Bad Request`: Lỗi từ phía client (dữ liệu không hợp lệ).
    - `401 Unauthorized`: Chưa xác thực.
    - `403 Forbidden`: Đã xác thực nhưng không có quyền.
    - `404 Not Found`: Không tìm thấy tài nguyên.
    - `500 Internal Server Error`: Lỗi từ phía server.
4.  **Versioning:** Luôn có phiên bản API trong URL, ví dụ: `/api/v1/...`.
5.  **Định dạng Response:**
    - Luôn trả về JSON.
    - Tên thuộc tính trong JSON dùng `camelCase`.
    - Cấu trúc response lỗi phải nhất quán, ví dụ:
      ```json
      {
        "error": {
          "code": "InvalidParameter",
          "message": "The provided URL is not valid."
        }
      }
      ```