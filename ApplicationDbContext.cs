using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoUploadAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<usersdata> usersdatas { get; set; }
        public DbSet<userslisting> userslistings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=localhost;Port=3306;Database=users_data;User=root;Password=octavian0504Aa&;";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

            optionsBuilder.UseMySql(connectionString, serverVersion);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<usersdata>()
                .HasKey(u => new { u.Username, u.Userphone });

            modelBuilder.Entity<userslisting>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<userslisting>()
                .HasOne<usersdata>()
                .WithMany()
                .HasForeignKey(l => new { l.Username, l.Phone_number })
                .HasPrincipalKey(u => new { u.Username, u.Userphone });
        }
    }

    public class userslisting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Phone_number { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        [Required]
        public int Make_year { get; set; }
        public string Horsepower { get; set; }
        public string Mileage { get; set; }
        public string Video_path { get; set; }

        public string Price { get; set; }
    }

    public class usersdata
    {
        public string Username { get; set; }
        public string Userphone { get; set; }
        public string User_Email { get; set; }
        public string Userpasswd { get; set; }
    }
}
