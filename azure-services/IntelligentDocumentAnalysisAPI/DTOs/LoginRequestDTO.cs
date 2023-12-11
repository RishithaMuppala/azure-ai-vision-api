using System.ComponentModel.DataAnnotations;

namespace IntelligentDocumentAnalysisAPI.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

    }
}
