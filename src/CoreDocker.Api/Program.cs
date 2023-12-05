using CoreDocker.Api;
using CoreDocker.Api.AppStartup;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Api.WebApi.Filters;
using CoreDocker.Core.Framework.Settings;
using CoreDocker.Core.Startup;
using CoreDocker.Api.Swagger;
using CoreDocker.Core;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFilesAndEnvironment();
builder.AddSerilog();

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssemblyContaining<Program>()
    .RegisterServicesFromAssemblyContaining<Settings>()
);
builder.Services.AddCoreIoc();
builder.Services.AddApiIoc();
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddGraphQl();
builder.Services.AddCors();
builder.Services.AddIdentityService(builder.Configuration);
var openIdSettings = new OpenIdSettings(builder.Configuration);
builder.Services.AddAuthenticationClient(openIdSettings);
builder.Services.AddMvc(config => { config.Filters.Add(new CaptureExceptionFilter()); });
builder.Services.AddSwagger1(openIdSettings);
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();
app.UseCors(policy =>
{
    policy.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetPreflightMaxAge(TimeSpan.FromMinutes(10)) // Cache the OPTIONS calls.
        .WithOrigins(openIdSettings.GetOriginList());
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.AddIdentityService(openIdSettings);
app.UseBearerAuthentication();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();
app.AddGraphQl();

app.Run();


namespace CoreDocker.Api
{
    public partial class Program
    {
    }
}
