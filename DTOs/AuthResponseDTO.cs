namespace AdmissionInfoSystem.DTOs
{
    public class AuthResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public UserDTO? User { get; set; }
    }
} 