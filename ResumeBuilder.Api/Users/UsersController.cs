using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Api.Authentication.Models;
using ResumeBuilder.Api.Users.Models;
using ResumeBuilder.Application.Users;
using ResumeBuilder.Domain.Users;
using Swashbuckle.AspNetCore.Annotations;

namespace ResumeBuilder.Api.Users
{
    [ApiController]
    [Route("[controller]")]
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
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(string), 401)]
        public async Task<ActionResult> RegisterAsync([FromBody] UserApi user)
        {
            var domainUser = Map<UserApi, User>(user);
            var request = new CreateUserRequest(domainUser);
            var result = await _mediator.Send(request);
            if (result.NumberOfUserCreated <= 0)
                return BadRequest("User already exists");

            return Created("User created", result);
        }

        private TDest Map<TSrc, TDest>(TSrc source)
        {
            try
            {
                return _autoMapper.Map<TDest>(source);
            }
            catch (AutoMapperMappingException ex)
            {
                throw ex.GetBaseException();
            }
        }
    }
}
