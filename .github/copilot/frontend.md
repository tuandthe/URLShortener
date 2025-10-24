# Hướng dẫn cho Frontend ReactJS

## Cấu trúc và Component
1.  **Functional Components & Hooks:** Chỉ sử dụng functional components với React Hooks (`useState`, `useEffect`, `useContext`, `useMemo`, `useCallback`). Không sử dụng Class Components.
2.  **Cấu trúc thư mục:** Tuân thủ cấu trúc thư mục sau:
    - `src/components/`: Cho các component tái sử dụng.
    - `src/pages/`: Cho các component đại diện cho một trang.
    - `src/hooks/`: Cho các custom hook.
    - `src/services/`: Cho logic gọi API.
    - `src/store/` hoặc `src/context/`: Cho quản lý state toàn cục.
3.  **Styling:** Sử dụng CSS Modules hoặc Styled-components để đóng gói style cho từng component, tránh xung đột CSS.

## Quản lý State
- Với state đơn giản trong component, dùng `useState`.
- Với state phức tạp hoặc cần chia sẻ giữa nhiều component, ưu tiên sử dụng `Context API` kết hợp với `useReducer`. Đối với ứng dụng lớn hơn, cân nhắc `Zustand` hoặc `Redux Toolkit`.

## Tương tác API
- Sử dụng `axios` để thực hiện các yêu cầu HTTP.
- Tạo một instance axios chung để cấu hình base URL và interceptor (xử lý lỗi, token).
- Tách biệt logic gọi API ra khỏi component, đặt trong thư mục `src/services/`.
- Sử dụng custom hooks (ví dụ `useFetch`) để đóng gói logic gọi API, quản lý trạng thái loading và error.