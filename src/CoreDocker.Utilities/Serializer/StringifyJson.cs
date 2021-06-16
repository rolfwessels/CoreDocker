using System;
using System.Text;
using Newtonsoft.Json;

namespace CoreDocker.Utilities.Serializer
{
    public interface IStringify
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string value);
        ReadOnlyMemory<byte> SerializeToUtf8Bytes<T>(T value);
        object Deserialize(Type type, ReadOnlyMemory<byte> value);
    }

    public class StringifyJson : IStringify
    {
        private readonly JsonSerializerSettings _setting;

        public StringifyJson()
        {
            _setting = new JsonSerializerSettings() {Formatting = Formatting.None};
        }

        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value,_setting);
        }

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _setting);
        }

        public object Deserialize(Type type, ReadOnlyMemory<byte> value)
        {
            return Deserialize(type, Encoding.UTF8.GetString(value.ToArray()));
        }

        public object Deserialize(Type type,string value)
        {
            return JsonConvert.DeserializeObject(value,type, _setting);
        }

        public ReadOnlyMemory<byte> SerializeToUtf8Bytes<T>(T value)
        {
            return new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(Serialize(value)));
        }
    }
}