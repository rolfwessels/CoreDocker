namespace CoreDocker.Shared
{
    public class RouteHelper
    {
        public const string ApiPrefix = "api/";
        public const string WithId = "{id}";
        public const string WithDetail = "/detail";

        public const string ProjectController = ApiPrefix + "config";
        public const string UserController = ApiPrefix + "config";
        public const string UserControllerRegister = "register";
        public const string UserControllerForgotPassword = "forgotpassword";
        public const string UserControllerWhoAmI = "whoami";
        public const string UserControllerRoles = "roles";
    }
}
