using System.ComponentModel.DataAnnotations;

namespace IntelligentDocumentAnalysisAPI.DTOs
{
    public class RegisterResponseDTO
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
    }
}
