using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Infrastructure;
using Infrastructure.DotEnv;
using Infrastructure.Constants;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        options.TokenLifespan = TimeSpan.FromHours(3));

builder.Logging.AddConsole();
var app = builder.Build();
app.UseCors("GeneralPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseForwardedHeaders();
}



app.UseHsts();

app.Use((context, next) =>
{
    var host = VariableBuilder.GetVariable(EnvFileConstants.HOST);
    context.Request.Host = new HostString(host);
    context.Request.Scheme = VariableBuilder.GetVariable(EnvFileConstants.SCHEME);
    return next();
});

app.UseCookiePolicy();

app.UseAuthentication();
app.UseForwardedHeaders();
app.UseAuthorization();

app.UseSession();

app.Use(async (context, next) =>
{
    // Check if the Cookie header is present
    if (context.Request.Headers.TryGetValue("Cookie", out var cookieHeader))
    {
        // Extract the SessionId from the Cookie header
        var cookies = cookieHeader.ToString().Split(';');
        var sessionIdCookie = cookies.FirstOrDefault(c => c.Trim().StartsWith("SessionId="));

        if (!string.IsNullOrEmpty(sessionIdCookie))
        {
            var sessionId = sessionIdCookie.Split('=')[1];

            // If the Authorization header is not already set, add it with the SessionId as a Bearer token
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + sessionId);
            }
        }
    }

    await next();
});

app.MapControllers();

//app.MigrateDatabase();
app.Seeder();
app.MigrateDatabase();
app.Run();
