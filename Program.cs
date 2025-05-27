using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure PostgreSQL connection (Supabase)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                "Host=db.rgjnylthyxydbcghbllq.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=IloveYou3000!123";

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Add repositories
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
            builder.Services.AddScoped<IAdmissionNewRepository, AdmissionNewRepository>();
            builder.Services.AddScoped<IAdmissionMethodRepository, AdmissionMethodRepository>();
            builder.Services.AddScoped<IAdmissionCriteriaRepository, AdmissionCriteriaRepository>();
            builder.Services.AddScoped<IAcademicProgramRepository, AcademicProgramRepository>();
            builder.Services.AddScoped<IMajorRepository, MajorRepository>();
            builder.Services.AddScoped<IScholarshipRepository, ScholarshipRepository>();

            // Add services
            builder.Services.AddScoped<IUniversityService, UniversityService>();
            builder.Services.AddScoped<IAdmissionNewService, AdmissionNewService>();
            builder.Services.AddScoped<IAdmissionMethodService, AdmissionMethodService>();
            builder.Services.AddScoped<IAdmissionCriteriaService, AdmissionCriteriaService>();
            builder.Services.AddScoped<IAcademicProgramService, AcademicProgramService>();
            builder.Services.AddScoped<IMajorService, MajorService>();
            builder.Services.AddScoped<IScholarshipService, ScholarshipService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
} 