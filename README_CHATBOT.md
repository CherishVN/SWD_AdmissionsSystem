# Hệ thống AI Chatbot Tuyển Sinh

## Tổng quan
Hệ thống AI Chatbot đã được tích hợp vào Admission Info System để hỗ trợ người dùng tư vấn về tuyển sinh đại học. Chatbot sử dụng Google Gemini AI để tạo phản hồi thông minh dựa trên dữ liệu thực tế từ hệ thống.

## Tính năng chính

### 1. Chat với AI
- Hỏi đáp về thông tin trường đại học
- Tư vấn về ngành học và điểm chuẩn
- So sánh các trường đại học
- Thông tin về học bổng
- Tin tức tuyển sinh mới nhất

### 2. Quản lý phiên chat
- Tạo phiên chat mới
- Lưu lịch sử chat theo người dùng
- Xóa phiên chat
- Tự động tạo tiêu đề cho phiên chat

## API Endpoints

### Chat Endpoints
```
POST /api/chat/send
GET /api/chat/history
GET /api/chat/session/{sessionId}
DELETE /api/chat/session/{sessionId}
POST /api/chat/new-session
```

### Gửi tin nhắn
```http
POST /api/chat/send
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "message": "Tôi muốn hỏi về điểm chuẩn ngành Công nghệ thông tin",
  "sessionId": null // null để tạo session mới, số để tiếp tục session
}
```

### Phản hồi
```json
{
  "sessionId": 1,
  "botResponse": "Dựa trên dữ liệu từ hệ thống...",
  "timestamp": "2024-12-19T10:30:00Z"
}
```

## Cấu hình

### 1. API Key Gemini
Cập nhật API key trong `appsettings.json`:
```json
{
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY_HERE"
  }
}
```

Hoặc sử dụng biến môi trường:
```bash
export GEMINI_API_KEY="your-api-key-here"
```

### 2. Database
Chatbot sử dụng các bảng đã có sẵn:
- `ChatSession` - Lưu thông tin phiên chat
- `ChatMessage` - Lưu tin nhắn trong phiên chat
- Các bảng dữ liệu tuyển sinh để tạo context cho AI

## Cách sử dụng

### 1. Đăng nhập
Người dùng cần đăng nhập để sử dụng chatbot (yêu cầu JWT token)

### 2. Tạo phiên chat mới
```http
POST /api/chat/new-session
```

### 3. Gửi tin nhắn
```http
POST /api/chat/send
{
  "message": "Xin chào! Tôi muốn biết về ngành CNTT",
  "sessionId": 1
}
```

### 4. Xem lịch sử chat
```http
GET /api/chat/history
```

## Ví dụ câu hỏi

### Về trường đại học
- "Cho tôi biết về Đại học Bách Khoa Hà Nội"
- "So sánh Đại học FPT và Đại học PTIT"
- "Trường nào có ngành CNTT tốt nhất?"

### Về điểm chuẩn
- "Điểm chuẩn ngành CNTT năm 2024 là bao nhiêu?"
- "So sánh điểm chuẩn ngành Y giữa các trường"

### Về học bổng
- "Có học bổng nào cho sinh viên CNTT không?"
- "Điều kiện xin học bổng ở trường nào dễ nhất?"

### Tư vấn chung
- "Tôi 25 điểm khối A, nên chọn trường nào?"
- "Ngành nào dễ xin việc nhất hiện tại?"

## Lưu ý kỹ thuật

### 1. AI Context
Chatbot tự động lấy dữ liệu liên quan từ:
- Thông tin trường đại học
- Thông tin ngành học  
- Điểm chuẩn tuyển sinh
- Tin tức tuyển sinh
- Thông tin học bổng

### 2. Xử lý lỗi
- API key không hợp lệ: Hiển thị thông báo lỗi
- Không có dữ liệu: AI vẫn trả lời dựa trên kiến thức tổng quát
- Lỗi mạng: Thông báo thử lại sau

### 3. Bảo mật
- Yêu cầu JWT token cho tất cả endpoints
- Mỗi người dùng chỉ xem được chat của mình
- API key được bảo mật qua biến môi trường

## Troubleshooting

### Lỗi thường gặp
1. **"Xin lỗi, tôi chưa thể kết nối với dịch vụ AI"**
   - Kiểm tra API key Gemini
   - Đảm bảo API key được cấu hình đúng

2. **401 Unauthorized**
   - Kiểm tra JWT token
   - Đăng nhập lại

3. **500 Internal Server Error**
   - Kiểm tra logs server
   - Kiểm tra kết nối database

### Debug
```bash
# Xem logs chi tiết
dotnet run --verbosity detailed

# Test API với curl
curl -X POST http://localhost:5000/api/chat/send \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello", "sessionId": null}'
```

## Deployment

### Railway/Production
Thêm biến môi trường:
```
GEMINI_API_KEY=your-actual-api-key
```

### Docker
```dockerfile
ENV GEMINI_API_KEY=your-api-key
```

---

*Tạo bởi: Admission Info System AI Chatbot v1.0* 