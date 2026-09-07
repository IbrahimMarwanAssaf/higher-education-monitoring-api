using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Models;

namespace UNIOOP.App.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;
        public DataContextEF(DbContextOptions<DataContextEF> options, IConfiguration config) : base(options)
        {
            _config = config;
        }
        public DbSet<Personnel> Personnels => Set<Personnel>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<GovernmentOfficer> GovernmentOfficers => Set<GovernmentOfficer>();
        public DbSet<University> Universities => Set<University>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<StudentCourse> StudentCourses => Set<StudentCourse>();

        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _config.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection was not found.");

                optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("public");

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(DataContextEF).Assembly);
        }
    }
}