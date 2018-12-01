using System;

namespace CoreDocker.Dal.Models.Base
{
    public interface IBaseDalModel
    {
        DateTime CreateDate { get; set; }
        DateTime UpdateDate { get; set; }
    }
}