using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000"
                                  )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
            .AllowCredentials();
        });
});*/
// ? CORS FIX
// ✅ ADD THIS
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddOcelot();
builder.WebHost.UseUrls(
    "http://0.0.0.0:7000",   // mobile access
    "https://localhost:7000" // web access
);
var app = builder.Build();
app.Urls.Add($"http://*:{port}");
//app.UseCors("AllowReactApp");
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
await app.UseOcelot();

app.Run();