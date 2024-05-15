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
        [ProducesResponseType(typeof(ErrorResponseApi), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GenerateTokenAsync([FromBody] TokenRequestApi tokenRequestApi)
        {
            var tokenRequest = Map<TokenRequestApi, TokenRequest>(tokenRequestApi);
            var request = new GetJwtTokenRequest(tokenRequest);
            var response = await _mediator.Send(request);
            if (string.IsNullOrEmpty(response.AccessToken))
            {
                var errorResponse = new ErrorResponseApi 
                {
                    Error = "invalid_client",
                    ErrorMessage = "Email or password incorrect."
                };
                return Unauthorized(errorResponse);
            }

            if (response.IsVerified is not null && !response.IsVerified.Value)
            {
                var errorResponse = new ErrorResponseApi
                {
                    Error = "user_not_verified",
                    ErrorMessage = "The email of the user is not verified"
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
                //Disabled for testing purpose
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]!))
            });
            Response.Cookies.Append(_configuration["JwtSettings:RefreshTokenCookieName"]!, response.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                //Disabled for testing purpose
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:RefreshTokenExpiration"]!))
            });
            return Ok(tokenResponse);
        }

        [HttpGet("refresh")]
        [Authorize(AuthenticationSchemes="refresh")]
        [ProducesResponseType(typeof(SuccessTokenResponseApi), 200)]
        [ProducesResponseType(typeof(ErrorResponseApi), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = HttpContext.Request.Cookies[_configuration["JwtSettings:RefreshTokenCookieName"]!]!;
            var request = new RefreshJwtTokenRequest(refreshToken);
            var response = await _mediator.Send(request);
            if (string.IsNullOrEmpty(response.AccessToken))
            {
                var errorResponse = new ErrorResponseApi
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
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenExpiration"]!))
            });
            return Ok(tokenResponse);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(SuccessTokenResponseApi), 200)]
        [ProducesResponseType(typeof(ErrorResponseApi), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshAccessTokenApi refreshAccessTokenApi)
        {
            var refreshToken = refreshAccessTokenApi.RefreshToken!;
            var request = new RefreshJwtTokenRequest(refreshToken);
            var response = await _mediator.Send(request);
            if (string.IsNullOrEmpty(response.AccessToken))
            {
                var errorResponse = new ErrorResponseApi
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
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
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
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(-1)
            });
            Response.Cookies.Append(_configuration["JwtSettings:RefreshTokenCookieName"]!, string.Empty, new CookieOptions
            {
                HttpOnly = true,
                //Secure = true,
                //SameSite = SameSiteMode.Strict,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(-1)
            });
            return Ok();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email)
        {
            var request = new ConfirmEmailRequest(email);
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromQuery] string email)
        {
            try
            {
                var request = new ForgetPasswordRequest(email);
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {                 
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("validate-reset-password-code")]
        public async Task<IActionResult> ValidateResetPasswordCode(ValidateResetPasswordCodeParametersApi parametersApi)
        {
            try
            {
                var email = parametersApi.Email!;
                var code = parametersApi.ResetPasswordCode!;
                var request = new ValidateResetPasswordCodeRequest(email, code);
                var result = await _mediator.Send(request);
                if (result.ResetPasswordPath is null)
                    return BadRequest("Invalid reset password code.");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string resetPasswordCode, ResetPasswordParametersApi parametersApi)
        {
            try 
            {
                var request = new ResetPasswordRequest(email, resetPasswordCode, parametersApi.Password);
                var result = await _mediator.Send(request);
                if (!result.Success)
                    return BadRequest("Wrong email or code");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
