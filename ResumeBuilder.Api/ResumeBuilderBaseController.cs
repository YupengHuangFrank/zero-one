using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ResumeBuilder.Api
{
    public class ResumeBuilderBaseController : ControllerBase
    {
        protected readonly IMapper _mapper;
        public ResumeBuilderBaseController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected TDest Map<TSrc, TDest>(TSrc source)
        {
            try
            {
                return _mapper.Map<TDest>(source);
            }
            catch (AutoMapperMappingException ex)
            {
                throw ex.GetBaseException();
            }
        }
    }
}
