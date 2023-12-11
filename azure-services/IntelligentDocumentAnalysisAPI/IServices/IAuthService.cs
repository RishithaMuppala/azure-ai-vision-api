using IntelligentDocumentAnalysisAPI.DTOs;

namespace IntelligentDocumentAnalysisAPI.IServices
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginViewModel);
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO registerViewModel);
        Task<RegisterResponseDTO> RegisterAdminAsync(RegisterRequestDTO registerViewModel);
    }
}
