using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ResumeBuilder.Infrastructure.Repositories.Resumes.Models
{
    public class ResumeInfra
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public TemplateInfra? Template { get; set; }
        public HeaderInfra? Header { get; set; }
        public IEnumerable<WorkExperienceInfra>? WorkExperience { get; set; }
        public IEnumerable<EducationInfra>? Education { get; set; }
        public string? Summary { get; set; }
    }
}
