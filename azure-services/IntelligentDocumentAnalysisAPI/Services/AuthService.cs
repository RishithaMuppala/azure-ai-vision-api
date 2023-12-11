using IntelligentDocumentAnalysisAPI.DTOs;
using IntelligentDocumentAnalysisAPI.Enums;
using IntelligentDocumentAnalysisAPI.IServices;
using IntelligentDocumentAnalysisAPI.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntelligentDocumentAnalysisAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTAppSettings _jWTAppSettings;

        public AuthService(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWTAppSettings> jWTAppSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jWTAppSettings = jWTAppSettings.Value;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO LoginRequestDTO)
        {
            var user = await _userManager.FindByNameAsync(LoginRequestDTO.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, LoginRequestDTO.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return new LoginResponseDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };
            }
            return new LoginResponseDTO { };
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO RegisterRequestDTO)
        {
            var userExists = await _userManager.FindByNameAsync(RegisterRequestDTO.UserName);
            if (userExists != null)
                return new RegisterResponseDTO { Status = "Error", Message = "User already exists!" };

            IdentityUser user = new()
            {
                Email = RegisterRequestDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = RegisterRequestDTO.UserName
            };
            var result = await _userManager.CreateAsync(user, RegisterRequestDTO.Password);
            if (!result.Succeeded)
            {
                return new RegisterResponseDTO { Status = "Error", Message = result.Errors.ToList()[0].Description };
            }

            var loginParams = new LoginRequestDTO()
            {
                UserName = RegisterRequestDTO.UserName,
                Password = RegisterRequestDTO.Password
            };

            var loginResult = await LoginAsync(loginParams);

            return new RegisterResponseDTO()
            {
                Status = "Success",
                Message = "User Created Successfully",
                Token = loginResult.Token
            };
        }

        public async Task<RegisterResponseDTO> RegisterAdminAsync(RegisterRequestDTO RegisterRequestDTO)
        {
            var userExists = await _userManager.FindByNameAsync(RegisterRequestDTO.UserName);
            if (userExists != null)
                return new RegisterResponseDTO { Status = "Error", Message = "User already exists!" };

            IdentityUser user = new()
            {
                Email = RegisterRequestDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = RegisterRequestDTO.UserName
            };
            var result = await _userManager.CreateAsync(user, RegisterRequestDTO.Password);
            if (!result.Succeeded)
                return new RegisterResponseDTO { Status = "Error", Message = "User creation failed! Please check user details and try again." };

            if (!await _roleManager.RoleExistsAsync(RoleEnum.Admin.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.Admin.ToString()));
            if (!await _roleManager.RoleExistsAsync(RoleEnum.User.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(RoleEnum.User.ToString()));

            if (await _roleManager.RoleExistsAsync(RoleEnum.Admin.ToString()))
            {
                await _userManager.AddToRoleAsync(user, RoleEnum.Admin.ToString());
            }
            if (await _roleManager.RoleExistsAsync(RoleEnum.Admin.ToString()))
            {
                await _userManager.AddToRoleAsync(user, RoleEnum.User.ToString());
            }
            return new RegisterResponseDTO { Status = "Success", Message = "User created successfully!" };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTAppSettings.Secret));

            var token = new JwtSecurityToken(
                issuer: _jWTAppSettings.ValidIssuer,
                audience: _jWTAppSettings.ValidAudience,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }
}
