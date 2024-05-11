using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Api.Users.Models;
using ResumeBuilder.Application.Users;
using ResumeBuilder.Domain.Users;

namespace ResumeBuilder.Api.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ResumeBuilderBaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMapper mapper,
            IMediator mediator) : base (mapper) 
        { 
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<ActionResult> RegisterAsync([FromBody] UserApi user)
        {
            var domainUser = Map<UserApi, User>(user);
            domainUser.IsVerified = false;
            var confirmEmailUri = Request.Scheme + "://" + Request.Host.Value + "/identity/confirm-email";
            var request = new CreateUserRequest(domainUser, confirmEmailUri);
            var result = await _mediator.Send(request);
            if (result.NumberOfUserCreated <= 0)
                return BadRequest("User already exists.");

            return Created("User created.", result);
        }
    }
}
