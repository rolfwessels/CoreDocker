using CoreDocker.Dal.Persistence;
using MongoDB.Bson;

namespace CoreDocker.Dal.MongoDb
{
    public class ObjectIdGenerator : IIdGenerator
    {
        public string NewId => ObjectId.GenerateNewId().ToString();
    }
}