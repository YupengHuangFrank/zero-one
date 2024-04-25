using AutoMapper;
using MediatR;
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

        public IdentityController(IMapper mapper, IMediator mediator) : base(mapper)
        {
            _mediator = mediator;
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
            if (string.IsNullOrEmpty(response.JwtToken))
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
                AccessToken = response.JwtToken,
                TokenType = "Bearer",
                ExpiresIn = "3600",
            };
            return Ok(tokenResponse);
        }
    }
}
