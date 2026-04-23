using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrderService.Data;
using Microsoft.AspNetCore.SignalR;
using OrderService.Hubs;
var builder = WebApplication.CreateBuilder(args);

// ? READ JWT KEY FROM appsettings.json
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

// ? AUTHENTICATION
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,           // ? SAME AS PRODUCT SERVICE
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    // 🔥 ADD THIS BLOCK (CRITICAL FOR SIGNALR)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
     path.StartsWithSegments("/orderHub", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
// ? AUTHORIZATION
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
// ? DB
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? CONTROLLERS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // ✅ MUST
        });
});


var app = builder.Build();
// 🔥 IMPORTANT: must be before MapHub

// ? SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend"); // ✅ BEFORE MapHub
// ✅ Enable WebSockets (FIX for 1006)
app.UseWebSockets();
// ?? VERY IMPORTANT ORDER
app.UseAuthentication();   // ? ADD THIS
app.UseAuthorization();    // ? KEEP THIS

app.MapControllers();
app.MapHub<OrderHub>("/orderHub");
app.Run();