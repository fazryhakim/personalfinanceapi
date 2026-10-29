using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceAPI.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Tambahkan Layanan Controller & API Explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Konfigurasi CORS untuk Vercel
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercelFrontend", policy =>
    {
        policy.WithOrigins("https://personalfinance-tau.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Konfigurasi JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? "KunciRahasiaJWTSuperAmanDanPanjangMinimal32Karakter";
var issuer = jwtSettings["Issuer"] ?? "PersonalFinanceAPI";
var audience = jwtSettings["Audience"] ?? "PersonalFinanceClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// 4. Konfigurasi Database SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));

var app = builder.Build();

// 5. Urutan Middleware (PENTING: CORS harus di paling atas sebelum request diproses)
app.UseCors("AllowVercelFrontend");

// Middleware File Statis & Swagger
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonalFinanceAPI v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();