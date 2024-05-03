using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Api.Authentication.Models;
using ResumeBuilder.Application.Authentication;
using ResumeBuilder.Domain.Authentication;

namespace ResumeBuilder.Api.Authentication
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ResumeBuilderBaseController
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public IdentityController(IMapper mapper, IMediator mediator, IConfiguration configuration) : base(mapper)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost("token")]
        [ProducesResponseType(typeof(SuccessTokenResponseApi), 200)]
        [ProducesResponseType(typeof(ErrorTokenResponseApi), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GenerateTokenAsync([FromBody] TokenRequestApi tokenRequestApi)
        {
            var tokenRequest = Map<TokenRequestApi, TokenRequest>(tokenRequestApi);
            var request = new GetJwtTokenRequest(tokenRequest);
            var response = await _mediator.Send(request);
            if (string.IsNullOrEmpty(response.AccessToken))
            {
                var errorResponse = new ErrorTokenResponseApi 
                {
                    Error = "invalid_client",
                    ErrorMessage = "Email or password incorrect."
                };
                return Unauthorized(errorResponse);
            }

            var tokenResponse = new SuccessTokenResponseApi 
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                TokenType = "Bearer",
                ExpiresIn = "3600",
            };

            Response.Cookies.Append(_configuration["JwtSettings:AccessTokenCookieName"]!, response.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]!))
            });
            Response.Cookies.Append(_configuration["JwtSettings:RefreshTokenCookieName"]!, response.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:RefreshTokenExpiration"]!))
            });
            return Ok(tokenResponse);
        }

        [HttpGet("refresh")]
        [Authorize(AuthenticationSchemes="refresh")]
        [ProducesResponseType(typeof(SuccessTokenResponseApi), 200)]
        [ProducesResponseType(typeof(ErrorTokenResponseApi), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = HttpContext.Request.Cookies[_configuration["JwtSettings:RefreshTokenCookieName"]!]!;
            var request = new RefreshJwtTokenRequest(refreshToken);
            var response = await _mediator.Send(request);
            if (string.IsNullOrEmpty(response.AccessToken))
            {
                var errorResponse = new ErrorTokenResponseApi
                {
                    Error = "invalid_client",
                    ErrorMessage = "Invalid refresh token."
                };
                return Unauthorized(errorResponse);
            }

            var tokenResponse = new SuccessTokenResponseApi
            {
                AccessToken = response.AccessToken,
                TokenType = "Bearer",
                ExpiresIn = "3600",
            };

            Response.Cookies.Append(_configuration["JwtSettings:AccessTokenCookieName"]!, response.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]!))
            });
            return Ok(tokenResponse);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(SuccessTokenResponseApi), 200)]
        [ProducesResponseType(typeof(ErrorTokenResponseApi), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshAccessTokenApi refreshAccessTokenApi)
        {
            var refreshToken = refreshAccessTokenApi.RefreshToken!;
            var request = new RefreshJwtTokenRequest(refreshToken);
            var response = await _mediator.Send(request);
            if (string.IsNullOrEmpty(response.AccessToken))
            {
                var errorResponse = new ErrorTokenResponseApi
                {
                    Error = "invalid_client",
                    ErrorMessage = "Invalid refresh token."
                };
                return Unauthorized(errorResponse);
            }

            var tokenResponse = new SuccessTokenResponseApi
            {
                AccessToken = response.AccessToken,
                TokenType = "Bearer",
                ExpiresIn = "3600",
            };

            Response.Cookies.Append(_configuration["JwtSettings:AccessTokenCookieName"]!, response.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]!))
            });
            return Ok(tokenResponse);
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append(_configuration["JwtSettings:AccessTokenCookieName"]!, string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(-1)
            });
            Response.Cookies.Append(_configuration["JwtSettings:RefreshTokenCookieName"]!, string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(-1)
            });
            return Ok();
        }
    }
}
