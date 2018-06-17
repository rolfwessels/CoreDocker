using System;

namespace CoreDocker.Shared.Models.Shared
{
  public class ErrorMessage
  {
    
    public ErrorMessage()
    {
        Message = String.Empty;
    }

    public ErrorMessage(string message)
    {
        Message = message;
    }

    public string Message { get; set; }

    public string AdditionalDetail { get; set; }
  }
}