namespace AdmissionInfoSystem.DTOs
{
    public class AuthResponseDTO
    {
        public UserDTO User { get; set; } = new UserDTO();
        public string Token { get; set; } = string.Empty;
    }
} 