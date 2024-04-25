using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ResumeBuilder.Infrastructure.Repositories.Users.Models
{
    public class UserInfra
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public IEnumerable<string> ResumeIds { get; set; } = new List<string>();
    }
}
