using Newtonsoft.Json;

namespace CoreDocker.Utilities.Helpers
{
  public static class CastHelper
  {
    
    public static T2 DynamicCastTo<T2>(this object val)
    {
      return JsonConvert.DeserializeObject<T2>(JsonConvert.SerializeObject(val));
    }

    
  }
}