using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Infrastructure;
using Infrastructure.DotEnv;
using Infrastructure.Constants;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAWSService<IAmazonS3>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
        options.TokenLifespan = TimeSpan.FromHours(3));


//builder.Services.Configure<IdentityOptions>(opts => { opts.SignIn.RequireConfirmedEmail= true; });

//builder.Services.AddAuthentication()
//    .AddGoogle("google", googleOptions =>
//    {
//        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
//        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
//    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseForwardedHeaders();
}

app.UseCors("GeneralPolicy");

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
    var token = context.Session.GetString("Token");
    if (!string.IsNullOrEmpty(token))
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Add("Authorization", "Bearer " + token);
        }
    }

    await next();
});

app.MapControllers();

//app.MigrateDatabase();
app.Seeder();
app.MigrateDatabase();
app.Run();
