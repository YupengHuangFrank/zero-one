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

        [HttpPost(Name = "CreateUser")]
        public async Task<ActionResult> CreateUserAsync([FromBody] UserApi user)
        {
            var domainUser = Map<UserApi, User>(user);
            var request = new CreateUserRequest(domainUser);
            var result = await _mediator.Send(request);
            return Created("Created", result);
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
