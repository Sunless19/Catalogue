﻿using Microsoft.EntityFrameworkCore;
using Catalog.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;


namespace Catalog.AppDBContext
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;port=3306;database=MyAppDb;user=myuser;password=mypass";

            optionsBuilder.UseMySql(connectionString,
                new MySqlServerVersion(new Version(8, 0, 34)));

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure TPH (Table Per Hierarchy) inheritance
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Teacher>("Teacher")
                .HasValue<Student>("Student");

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<StudentClass>()
        .HasKey(sc => new { sc.StudentId, sc.ClassId });

            modelBuilder.Entity<StudentClass>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentClasses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentClass>()
                .HasOne(sc => sc.Class)
                .WithMany(c => c.StudentClasses)
                .HasForeignKey(sc => sc.ClassId); ;


            base.OnModelCreating(modelBuilder);
        }
    }
}
