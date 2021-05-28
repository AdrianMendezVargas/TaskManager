using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Models.Domain;

namespace TaskManager.Models.Data {
    public class ApplicationDbContext : DbContext { //1. Using IdentityDbContext and specifying the user class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) {
        
        }

        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.EmailVerifications)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .Ignore(u => u.Tasks)
                .Ignore(u => u.EmailVerifications);

            base.OnModelCreating(modelBuilder);
        }

    }
}
