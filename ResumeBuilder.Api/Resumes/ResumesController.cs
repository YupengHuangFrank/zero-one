using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ResumeBuilder.Api.Resumes.Models;
using ResumeBuilder.Api.Resumes.Models.Exceptions;
using ResumeBuilder.Application.Resumes;
using ResumeBuilder.Domain.Resumes;
using ResumeBuilder.Domain.Users;
using ResumeBuilder.Infrastructure.Repositories.Resumes;
using ResumeBuilder.Infrastructure.Repositories.Users;
using System.IdentityModel.Tokens.Jwt;

namespace ResumeBuilder.Api.Resumes
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ResumesController : ResumeBuilderBaseController
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public ResumesController(IResumeRepository resumeRepository, IUserRepository userRepository, IMediator mediator, IMapper mapper) : base(mapper)
        {
            _resumeRepository = resumeRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetResumes()
        {
            try
            {
                var user = await GetUserFromToken(HttpContext);
                var resumes = await _resumeRepository.GetResumes(user.ResumeIds);

                return Ok(resumes.ToJson());
            }
            catch (BearerAuthorizationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetResume(string id)
        {
            var resume = await _resumeRepository.GetResume(id);
            if (resume == null)
                return NotFound();

            return Ok(resume.ToJson());
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> SaveResume([FromBody] ResumeApi resume)
        {
            try
            {
                var user = await GetUserFromToken(HttpContext);
                var domainResume = Map<ResumeApi, Resume>(resume);
                domainResume.UserId = user.Id;
                var request = new SaveResumeRequest(user, domainResume);
                var result = await _mediator.Send(request);

                if (!result.Success)
                    return BadRequest();

                return Ok(result);
            }
            catch (BearerAuthorizationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        private async Task<User> GetUserFromToken(HttpContext context)
        {
            var token = GetBearerToken(context);
            var email = token.Payload[JwtRegisteredClaimNames.Email];
            if (email == null)
                throw new BearerAuthorizationException("Invalid token.");

            var user = await _userRepository.GetUser(email.ToString()!);
            if (user == null)
                throw new BearerAuthorizationException("User not found.");

            return user;
        }

        private JwtSecurityToken GetBearerToken(HttpContext context)
        {
            var bearerToken = context.Request.Headers["Authorization"];
            var tokenString = bearerToken.ToString().Replace("Bearer ", string.Empty);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);
            return token;
        }
    }
}
