using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ResumeBuilder.Domain.Authentication;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ResumeBuilder.Application.Authentication
{
    public class GetJwtToken : IRequestHandler<GetJwtTokenRequest, GetJwtTokenResponse>
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<string> _passwordHasher;

        public GetJwtToken(IConfiguration configuration, IUserRepository userRepository, IPasswordHasher<string> passwordHasher)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<GetJwtTokenResponse> Handle(GetJwtTokenRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUser(request.TokenRequest.Email!);
            if (user == null)
                return new GetJwtTokenResponse(string.Empty);

            if (string.IsNullOrWhiteSpace(user.Password))
                return new GetJwtTokenResponse(string.Empty);

            var verificationResult = _passwordHasher.VerifyHashedPassword(string.Empty, user.Password, request.TokenRequest.Password!);
            if (verificationResult.Equals(PasswordVerificationResult.Failed))
                return new GetJwtTokenResponse(string.Empty);
            
            if (verificationResult.Equals(PasswordVerificationResult.SuccessRehashNeeded))
                Console.WriteLine("Password needs to be rehashed.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, request.TokenRequest.Email!),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JwtSettings:Issuer"]!),
              };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = $"Bearer {tokenHandler.WriteToken(token)}";
            return new GetJwtTokenResponse(jwt);
        }
    }

    public class GetJwtTokenRequest : IRequest<GetJwtTokenResponse>
    {
        public TokenRequest TokenRequest { get; set; }

        public GetJwtTokenRequest(TokenRequest tokenRequest)
        {
            TokenRequest = tokenRequest;
        }
    }

    public class GetJwtTokenResponse
    {
        public string? JwtToken { get; set; }

        public GetJwtTokenResponse(string jwtToken)
        {
            JwtToken = jwtToken;
        }
    }
}
