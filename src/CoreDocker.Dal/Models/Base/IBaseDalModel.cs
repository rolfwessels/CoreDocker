using System;

namespace CoreDocker.Dal.Models.Base
{
    public interface IBaseDalModel
    {
        DateTime CreateDate { get; }
        DateTime UpdateDate { get; }
    }
}