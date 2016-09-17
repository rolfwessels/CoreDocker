using System.Threading.Tasks;
using MongoDB.Driver;

namespace CoreDocker.Dal.Mongo.Migrations
{
    public interface IMigration
    {
        Task Update(IMongoDatabase db);
    }
}