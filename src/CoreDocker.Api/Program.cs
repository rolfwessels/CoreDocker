using System.Reflection;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Api.WebApi.Filters;
using CoreDocker.Core.Framework.Settings;
using CoreDocker.Core.Startup;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFilesAndEnvironment();


builder.Services.AddMediatR(cfg => cfg
        .RegisterServicesFromAssemblyContaining<Program>()
        // .RegisterServicesFromAssemblyContaining<not>()
    );
builder.Services.AddCoreIoc();
builder.Services.AddApiIoc();
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGraphQl();
builder.Services.AddCors();
builder.Services.UseIdentityService(builder.Configuration);
builder.Services.AddBearerAuthentication(new OpenIdSettings(builder.Configuration));
builder.Services.AddMvc(config => { config.Filters.Add(new CaptureExceptionFilter()); });
// builder.Services.AddSwagger();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();
// app.MapGraphQL();

app.Run();




namespace CoreDocker.Api
{
    public partial class Program
    {
    }
}

//
// namespace CoreDocker.Api
// {
//     public Startup(IConfiguration configuration)
//     {
//         Configuration = configuration;
//         Settings.Initialize(Configuration);
//     }
//
//     public IConfiguration Configuration { get; }
//
//     public IServiceProvider ConfigureServices(IServiceCollection services)
//     {
//         IocApi.Populate(services);
//         services.AddGraphQl();
//         services.AddCors();
//         services.UseIdentityService(Configuration);
//         services.AddBearerAuthentication();
//         services.AddMvc(config => { config.Filters.Add(new CaptureExceptionFilter()); });
//         services.AddSwagger();
//         services.AddSignalR();
//
//         return new AutofacServiceProvider(IocApi.Instance.Container);
//     }
//
//     public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//     {
//         PrintVersionNumber();
//         app.UseStaticFiles();
//         app.UseRouting();
//         var openIdSettings = new OpenIdSettings(Configuration);
//
//         app.UseCors(policy =>
//         {
//             policy.AllowAnyMethod()
//                 .AllowAnyHeader()
//                 .AllowCredentials()
//                 .SetPreflightMaxAge(TimeSpan.FromMinutes(10)) // Cache the OPTIONS calls.
//                 .WithOrigins(openIdSettings.GetOriginList());
//         });
//
//         if (env.IsDevelopment())
//         {
//             app.UseDeveloperExceptionPage();
//         }
//
//         app.UseIdentityService(openIdSettings);
//         app.UseBearerAuthentication();
//         app.UseAuthentication();
//         app.UseAuthorization();
//         app.AddGraphQl();
//         app.UseEndpoints(e => e.MapControllers());
//         app.UseSwagger();
//         SimpleFileServer.Initialize(app);
//     }
//
//     private static void PrintVersionNumber()
//     {
//         var log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
//         log.Information("Starting server {InformationalVersion}", InformationalVersion());
//     }
//
//     public static string InformationalVersion()
//     {
//         var assembly = Assembly.GetExecutingAssembly();
//         var productVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
//         return $"{productVersion}";
//     }
//     public class Program
//     {
//         public static void Main(string[] args)
//         {
//             Console.Title = "CoreDocker.Api";
//
//             Log.Logger = LoggingHelper.SetupOnce(() => new LoggerConfiguration().MinimumLevel.Debug()
//                 .WriteTo.File(@"c:\temp\logs\CoreDocker.Api.log", fileSizeLimitBytes: 10 * LoggingHelper.MB,
//                     rollOnFileSizeLimit: true)
//                 .WriteTo.Console(LogEventLevel.Information)
//                 //.ReadFrom.Configuration(BaseSettings.Config)
//                 .CreateLogger());
//
//             try
//             {
//                 BuildWebHost(args).Run();
//             }
//             finally
//             {
//                 Log.CloseAndFlush();
//             }
//         }
//
//         public static IWebHost BuildWebHost(string[] args)
//         {
//             return WebHost.CreateDefaultBuilder(args)
//                 .ConfigureServices((context, collection) =>
//                     collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory()))
//                 .UseKestrel()
//                 .UseUrls(args.FirstOrDefault() ?? "http://*:5010")
//                 .ConfigureAppConfiguration(SettingsFileReaderHelper)
//                 .UseStartup<Startup>()
//                 .Build();
//         }
//

//     }
// }