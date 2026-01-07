using AutoMapper;
using EasyGames.Class;
using EasyGames.Class.DATA;
using EasyGames.Services.ExtensionMethod;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EasyGames.Services.Services
{
    public interface IAuthService : IBaseService<User>
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<string> RegisterAsync(LoginRequest loginRequest);
    }

    public class AuthService : BaseService<User>, IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly ICurrentUserInfo _currentUserInfo;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AuthService> logger,
            ApplicationDbContext dbContext,
            ICurrentUserInfo currentUserInfo,
            IMapper mapper,
            IConfiguration config)
            : base(dbContext, mapper, currentUserInfo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _logger = logger;
            _currentUserInfo = currentUserInfo;
        }

        public async Task<string> RegisterAsync(LoginRequest loginRequest)
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (existingUser != null)
            {
                return "Email already exists";
            }

            var user = new User
            {
                UserName = loginRequest.Email,
                Email = loginRequest.Email,
                UserType = UserType.Customer
            };

            // Identity will automatically hash the password
            var result = await _userManager.CreateAsync(user, loginRequest.Password);

            if (!result.Succeeded)
            {
                return result.Errors.ToString();
            }

            return "User registered successfully";
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // 1. Find user by username
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) throw new Exception("Invalid username or password");

            // 2. Check if disabled
            if (user.Disabled) throw new Exception("User is disabled");

            // 3. Validate password using Identity
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded) throw new Exception("Invalid username or password");

            // 4. Generate JWT token
            string token = GenerateJwtToken(user);

            // 5. Return login response
            return new LoginResponse
            {
                Token = token,
                UserName = null,
                UserId = 0,
                Role = null
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var key = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var keyBytes = Encoding.UTF8.GetBytes(key!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
