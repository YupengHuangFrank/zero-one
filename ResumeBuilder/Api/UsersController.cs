using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Api.Models;
using ResumeBuilder.Application.Users;
using ResumeBuilder.Domain.Models;

namespace ResumeBuilder.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _autoMapper;
        private readonly IMediator _mediator;

        public UsersController(IMapper autoMapper,
            IMediator mediator) 
        { 
            _autoMapper = autoMapper;
            _mediator = mediator;
        }

        [HttpPost]
        [Route($"Users")]
        public async Task<ActionResult> CreateUserAsync([FromBody] UserApi user)
        {
            var domainUser = _autoMapper.Map<User>(user);
            var request = new CreateUserRequest(domainUser);
            var result = await _mediator.Send(request);
            return Created("Created", result);
        }

        [HttpPost]
        [Route($"Users")]
        public ActionResult GetTokenAsync()
        {
            return Ok("User");
        }
    }
}
