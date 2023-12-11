using System.ComponentModel.DataAnnotations;

namespace IntelligentDocumentAnalysisAPI.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }
}
