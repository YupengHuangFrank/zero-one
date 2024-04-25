using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using ResumeBuilder.Domain.Resumes;
using ResumeBuilder.Infrastructure.Repositories.Resumes.Models;

namespace ResumeBuilder.Infrastructure.Repositories.Resumes
{
    public interface IResumeRepository
    {
        Task<string?> AddOrUpdateResume(Resume resume);
        Task<Resume?> GetResume(string id);
        Task<IEnumerable<Resume>> GetResumes(IEnumerable<string> ids);
    }

    public class ResumeRepository : IResumeRepository
    {
        private readonly IMongoCollection<ResumeInfra> _resumeCollection;
        private readonly IMapper _mapper;

        public ResumeRepository(IMongoCollection<ResumeInfra> resumeCollection, IMapper mapper)
        {
            _resumeCollection = resumeCollection;
            _mapper = mapper;
        }

        public async Task<string?> AddOrUpdateResume(Resume resume)
        {
            var resumeInfra = _mapper.Map<ResumeInfra>(resume);
            if (string.IsNullOrEmpty(resumeInfra.Id))
                resumeInfra.Id = ObjectId.GenerateNewId().ToString();

            var result = await _resumeCollection.ReplaceOneAsync(x => x.Id == resumeInfra.Id, resumeInfra, new ReplaceOptions { IsUpsert = true });
            return result.IsAcknowledged ? resumeInfra.Id : null;
        }

        public async Task<Resume?> GetResume(string id)
        {
            var resumeInfras = await _resumeCollection.FindAsync(x => x.Id == id);
            var resumeInfra = resumeInfras.FirstOrDefault();
            if (resumeInfra == null)
                return null;

            var resume = _mapper.Map<Resume>(resumeInfra);
            return resume;
        }

        public async Task<IEnumerable<Resume>> GetResumes(IEnumerable<string> ids)
        {
            var resumeInfras = await _resumeCollection.FindAsync(x => ids.Contains(x.Id));
            var results = new List<Resume>();
            var resumeInfrasList = resumeInfras.ToList();
            if (resumeInfrasList.Count > 0)
            {
                foreach(var resumeInfra in resumeInfrasList)
                {
                    var resume = _mapper.Map<Resume>(resumeInfra);
                    results.Add(resume);
                }
            }

            return results;
        }
    }
}
