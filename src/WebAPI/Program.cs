using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Infrastructure;
using Infrastructure.Constants;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// ✅ Pour Cloud Run (écoute sur port 8080)
builder.WebHost.UseUrls("http://+:8080");

// ✅ Charger .env uniquement en développement
if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

// Ajout des services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromHours(3));

builder.Logging.AddConsole();

var app = builder.Build();

app.UseCors("GeneralPolicy");

// Swagger uniquement en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseForwardedHeaders();
}

app.UseHsts();

// ✅ Utilisation sécurisée de HOST / SCHEME (évite plantage si manquant)
app.Use((context, next) =>
{
    var host = Environment.GetEnvironmentVariable(EnvFileConstants.HOST);
    var scheme = Environment.GetEnvironmentVariable(EnvFileConstants.SCHEME);

    if (!string.IsNullOrEmpty(host))
    {
        context.Request.Host = new HostString(host);
    }

    if (!string.IsNullOrEmpty(scheme))
    {
        context.Request.Scheme = scheme;
    }

    return next();
});

app.UseCookiePolicy();
app.UseAuthentication();
app.UseForwardedHeaders();
app.UseAuthorization();
app.UseSession();

// Ajout token à partir du cookie
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

// ✅ Log visible dans les logs Cloud Run
Console.WriteLine("✅ WebAPI démarrée sur http://+:8080");

app.Run();
