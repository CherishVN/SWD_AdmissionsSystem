using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.Repositories.Implements;
using AdmissionInfoSystem.Repositories.Interface;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Services.Implements;
using AdmissionInfoSystem.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AdmissionInfoSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình để lắng nghe trên tất cả interfaces và port từ biến môi trường (cho Railway)
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure PostgreSQL connection (Supabase - với hỗ trợ biến môi trường cho Railway)
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING") ??
                builder.Configuration.GetConnectionString("DefaultConnection") ??
                "User Id=postgres.rgjnylthyxydbcghbllq;Password=IloveYou3000!123;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres";

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                }));

            // Add repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
            builder.Services.AddScoped<IAdmissionNewRepository, AdmissionNewRepository>();
            builder.Services.AddScoped<IAdmissionMethodRepository, AdmissionMethodRepository>();
            builder.Services.AddScoped<IAdmissionCriteriaRepository, AdmissionCriteriaRepository>();
            builder.Services.AddScoped<IAcademicProgramRepository, AcademicProgramRepository>();
            builder.Services.AddScoped<IMajorRepository, MajorRepository>();
            builder.Services.AddScoped<IScholarshipRepository, ScholarshipRepository>();
            builder.Services.AddScoped<IAdmissionScoreRepository, AdmissionScoreRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add services
            builder.Services.AddScoped<IUniversityService, UniversityService>();
            builder.Services.AddScoped<IAdmissionNewService, AdmissionNewService>();
            builder.Services.AddScoped<IAdmissionMethodService, AdmissionMethodService>();
            builder.Services.AddScoped<IAdmissionCriteriaService, AdmissionCriteriaService>();
            builder.Services.AddScoped<IAcademicProgramService, AcademicProgramService>();
            builder.Services.AddScoped<IMajorService, MajorService>();
            builder.Services.AddScoped<IScholarshipService, ScholarshipService>();
            builder.Services.AddScoped<IAdmissionScoreService, AdmissionScoreService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            // Cấu hình JWT Authentication (với hỗ trợ biến môi trường cho Railway)
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? 
                builder.Configuration["Jwt:Key"] ?? 
                "ThisIsADefaultSecretKeyForJWTAuthentication12345";
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? 
                builder.Configuration["Jwt:Issuer"] ?? 
                "AdmissionInfoSystem";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? 
                builder.Configuration["Jwt:Audience"] ?? 
                "AdmissionInfoSystemClient";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            
            // Cấu hình Swagger để hỗ trợ JWT Bearer Token
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Admission Info System API", Version = "v1" });
                
                // Thêm cấu hình Bearer token
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Nhập token JWT ",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Luôn enable Swagger để có thể test API trên Railway
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admission Info System API V1");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.RoutePrefix = "swagger"; // Đặt Swagger tại /swagger
            });

            //app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowAll");

            // Thêm authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.Run();
        }
    }
} 