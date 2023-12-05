namespace CoreDocker.Shared
{
    public class RouteHelper
    {
        public const string ApiPrefix = "api/";
        public const string WithId = "{id}";
        public const string WithDetail = "detail";

        public const string PingController = ApiPrefix + "ping";
        public const string PingControllerHealthCheck = "hc";
    }
}