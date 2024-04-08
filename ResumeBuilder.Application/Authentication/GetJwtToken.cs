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

        public Task<GetJwtTokenResponse> Handle(GetJwtTokenRequest request, CancellationToken cancellationToken)
        {
            var hashedPassword = _userRepository.GetHashedPassword(request.TokenRequest.Email!);
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return Task.FromResult(new GetJwtTokenResponse(string.Empty));

            var verificationResult = _passwordHasher.VerifyHashedPassword(string.Empty, hashedPassword, request.TokenRequest.Password!);
            if (verificationResult.Equals(PasswordVerificationResult.Failed))
                return Task.FromResult(new GetJwtTokenResponse(string.Empty));
            
            if (verificationResult.Equals(PasswordVerificationResult.SuccessRehashNeeded))
                Console.WriteLine("Password needs to be rehashed");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, request.TokenRequest.Email!),
              };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);
            return Task.FromResult(new GetJwtTokenResponse(jwt));
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
