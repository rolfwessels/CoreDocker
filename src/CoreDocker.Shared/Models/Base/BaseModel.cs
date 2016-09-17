using System;
using CoreDocker.Shared.Models.Interfaces;

namespace CoreDocker.Shared.Models.Base
{
    public abstract class BaseModel : IBaseModel
    {
        public string Id { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}