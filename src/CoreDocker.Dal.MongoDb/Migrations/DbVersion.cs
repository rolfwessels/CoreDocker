using System;
using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Dal.MongoDb.Migrations
{
    public record DbVersion : IBaseDalModel
    {
        public DbVersion(int id, string name)
        {
            Id = id;
            Name = name;
            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}