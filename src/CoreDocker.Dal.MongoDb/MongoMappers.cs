using System;
using CoreDocker.Dal.Models.Base;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace CoreDocker.Dal.MongoDb
{
    public class MongoMappers
    {

        public void InitializeMappers(ILogger logger)
        {
            SetupDataTimeSerializer(logger);
            SetupMapping();
        }

        private static void SetupMapping()
        {
            BsonClassMap.RegisterClassMap<BaseDalModel>(cm =>
            {
                cm.MapProperty(c => c.CreateDate).SetElementName("Cd");
                cm.MapProperty(c => c.UpdateDate).SetElementName("Ud");
            });
            BsonClassMap.RegisterClassMap<BaseDalModelWithId>(cm =>
            {
                cm.MapIdProperty(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        private static void SetupDataTimeSerializer(ILogger logger)
        {
            try
            {
                var serializer = new DateTimeSerializer(DateTimeKind.Local);
                BsonSerializer.RegisterSerializer(typeof(DateTime), serializer);
            }
            catch (Exception e)
            {
                logger.LogError("MongoMappers:InitializeMappers " + e.Message,e);
            }
        }
    }
}