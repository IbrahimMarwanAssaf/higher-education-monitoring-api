using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Data.Seed;
using UNIOOP.App.Helpers;
using UNIOOP.App.Mappings;
using UNIOOP.App.Repositories.Implementations;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services;
using UNIOOP.App.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors", (corsBuilder) =>
            {
                corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
    options.AddPolicy("ProdCors", (corsBuilder) =>
        {
            corsBuilder.WithOrigins("https://myProductionSite.com")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddDbContext<DataContextEF>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();

builder.Services.AddScoped<IUniversityService, UniversityService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IGovernmentOfficerService, GovernmentOfficerService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddTransient<IDatabaseValidationHelper, DatabaseValidationHelper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    using IServiceScope scope = app.Services.CreateScope();

    DataContextEF context =
        scope.ServiceProvider
            .GetRequiredService<DataContextEF>();

    // Applies any pending migrations.
    await context.Database.MigrateAsync();

    // Inserts test data if it has not already been inserted.
    await HigherEducationDbSeeder.SeedAsync(context);

    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();