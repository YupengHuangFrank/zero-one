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
using System.Security.Cryptography;


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
            var user = await _userRepository.GetUserFromEmail(request.TokenRequest.Email!);
            if (user == null)
                return new GetJwtTokenResponse(string.Empty, string.Empty);

            if (string.IsNullOrWhiteSpace(user.Password))
                return new GetJwtTokenResponse(string.Empty, string.Empty);

            var verificationResult = _passwordHasher.VerifyHashedPassword(string.Empty, user.Password, request.TokenRequest.Password!);
            if (verificationResult.Equals(PasswordVerificationResult.Failed))
                return new GetJwtTokenResponse(string.Empty, string.Empty);
            
            if (verificationResult.Equals(PasswordVerificationResult.SuccessRehashNeeded))
                Console.WriteLine("Password needs to be rehashed.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var rsaKey = RSA.Create();
            rsaKey.ImportRSAPrivateKey(Convert.FromBase64String(_configuration["JwtSettings:PrivateKey"]!), out _);

            var accessTokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JwtSettings:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JwtSettings:Audience"]!),
                new Claim(JwtRegisteredClaimNames.Typ, _configuration["JwtSettings:AccessTokenTypeName"]!),
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

            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);

            var refreshTokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JwtSettings:Issuer"]!),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JwtSettings:Audience"]!),
                new Claim(JwtRegisteredClaimNames.Typ, _configuration["JwtSettings:RefreshTokenTypeName"]!),
            };


            success = int.TryParse(_configuration["JwtSettings:RefreshTokenExpiration"]!, out var refreshTokenValidMinutes);
            if (!success)
                throw new Exception("Invalid configuration for refresh token expiration.");

            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(refreshTokenClaims),
                Expires = DateTime.UtcNow.AddMinutes(refreshTokenValidMinutes),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsaKey), SecurityAlgorithms.RsaSha256)
            };

            var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);

            var accessTokenString = tokenHandler.WriteToken(accessToken);
            var refreshTokenString = tokenHandler.WriteToken(refreshToken);
            return new GetJwtTokenResponse(accessTokenString, refreshTokenString);
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
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public GetJwtTokenResponse(string jwtToken, string? refreshToken)
        {
            AccessToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
