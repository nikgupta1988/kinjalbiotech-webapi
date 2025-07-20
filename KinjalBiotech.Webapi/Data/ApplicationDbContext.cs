using KinjalBiotech.Webapi.Models;
using Microsoft.EntityFrameworkCore;

namespace KinjalBiotech.Webapi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Medicine> Medicines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary keys
            modelBuilder.Entity<Department>().HasKey(d => d.DeptID);
            modelBuilder.Entity<Medicine>().HasKey(m => m.MedicineID);

            // Configure relationship: One Department to many Medicines
            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.Department)
                .WithMany()
                .HasForeignKey(m => m.DeptID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure table names
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<Medicine>().ToTable("Medicines");

            // Configure column constraints
            modelBuilder.Entity<Department>()
                .Property(d => d.DeptName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Medicine>()
                .Property(m => m.MedicineName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Medicine>()
                .Property(m => m.MedicineDesc)
                .HasMaxLength(500);

            modelBuilder.Entity<Medicine>()
                .Property(m => m.ImageUrl)
                .HasMaxLength(500);

            base.OnModelCreating(modelBuilder);
        }
    }
}
