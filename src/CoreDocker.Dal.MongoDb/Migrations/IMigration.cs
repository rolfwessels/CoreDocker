using System.Threading.Tasks;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb.Migrations
{
    public interface IMigration
    {
        Task Update(IMongoDatabase db);
    }
}