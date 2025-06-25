using DotNetEnv;
using Infrastructure;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        options.TokenLifespan = TimeSpan.FromHours(3));

builder.Logging.AddConsole();

var app = builder.Build();
app.UseCors("GeneralPolicy");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseForwardedHeaders();
}



app.UseHsts();

app.Use((context, next) =>
{
    var host = Environment.GetEnvironmentVariable(EnvFileConstants.HOST);
    context.Request.Host = new HostString(host);
    context.Request.Scheme = Environment.GetEnvironmentVariable(EnvFileConstants.SCHEME)!;
    return next();
});

app.UseCookiePolicy();

app.UseAuthentication();
app.UseForwardedHeaders();
app.UseAuthorization();

app.UseSession();

app.Use(async (context, next) =>
{

    if (context.Request.Headers.TryGetValue("Cookie", out var cookieHeader))
    {
        var cookies = cookieHeader.ToString().Split(';');
        var sessionIdCookie = cookies.FirstOrDefault(c => c.Trim().StartsWith("SessionId="));

        if (!string.IsNullOrEmpty(sessionIdCookie))
        {
            var sessionId = sessionIdCookie.Split('=')[1];


            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + sessionId);
            }
        }
    }

    await next();
});

app.MapControllers();

app.Run();
