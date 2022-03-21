using System;

namespace CoreDocker.Shared.Models.Shared
{
    public abstract record BaseModel : IBaseModel
    {
        public string Id { get; set; } = null!;
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}