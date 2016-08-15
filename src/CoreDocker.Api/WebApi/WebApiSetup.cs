using MainSolutionTemplate.Api.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MainSolutionTemplate.Api.WebApi
{
  public class WebApiSetup
  {
    public static void Setup(MvcOptions config)
    {
      config.Filters.Add(new CaptureExceptionFilter());
    }
  }
}