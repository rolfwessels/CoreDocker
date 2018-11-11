using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Core.Framework.MessageUtil.Models
{
    public class DalUpdateMessage<T>
    {
        public DalUpdateMessage(T value, UpdateTypes updateType)
        {
            Value = value;
            UpdateType = updateType;
        }

        public T Value { get; }

        public UpdateTypes UpdateType { get; }

        public override string ToString()
        {
            return string.Format("UpdateType: {0}, Value: {1}", UpdateType, Value);
        }
    }
}