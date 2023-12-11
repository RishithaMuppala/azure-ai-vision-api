using System.ComponentModel.DataAnnotations;

namespace IntelligentDocumentAnalysisAPI.DTOs
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }


    }
}
