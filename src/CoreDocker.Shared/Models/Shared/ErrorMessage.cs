namespace CoreDocker.Shared.Models.Shared
{
    public record ErrorMessage(string Message)
    {
        public string? AdditionalDetail { get; set; }
    }
}