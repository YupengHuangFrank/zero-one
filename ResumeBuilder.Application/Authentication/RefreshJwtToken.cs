using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ResumeBuilder.Application.Authentication
{
    public class RefreshJwtToken : IRequestHandler<RefreshJwtTokenRequest, RefreshJwtTokenResponse>
    {
        private readonly IConfiguration _configuration;

        public RefreshJwtToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<RefreshJwtTokenResponse> Handle(RefreshJwtTokenRequest request, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();            
            var rsaKey = RSA.Create();
            rsaKey.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["JwtSettings:PrivateKey"]!), out _);

            var refreshToken = tokenHandler.ReadJwtToken(request.RefreshToken);
            var audience = refreshToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud)?.Value;
            if (audience != _configuration["JwtSettings:RefreshTokenAudience"])
                return Task.FromResult(new RefreshJwtTokenResponse(null));

            var userId = refreshToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Task.FromResult(new RefreshJwtTokenResponse(null));

            var accessTokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JwtSettings:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JwtSettings:AccessTokenAudience"]!)
             };

            var success = int.TryParse(_configuration["JwtSettings:AccessTokenExpiration"]!, out var accessTokenValidMinutes);
            if (!success)
                throw new Exception("Invalid configuration for access token expiration.");

            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(accessTokenClaims),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenValidMinutes),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsaKey), SecurityAlgorithms.RsaSha256)
            };

            var newAccessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            var newAccessTokenString = tokenHandler.WriteToken(newAccessToken);
            return Task.FromResult(new RefreshJwtTokenResponse(newAccessTokenString));
        }
    }


    public class RefreshJwtTokenRequest : IRequest<RefreshJwtTokenResponse>
    {
        public string? RefreshToken { get; set; }

        public RefreshJwtTokenRequest(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }

    public class RefreshJwtTokenResponse
    {
        public string? AccessToken { get; }

        public RefreshJwtTokenResponse(string? accessToken)
        {
            AccessToken = accessToken;
        }
    }
}
