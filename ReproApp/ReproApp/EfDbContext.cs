using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ReproApp
{
    public class EfDbContext : DbContext
    {
        private readonly string connectionString;

        public EfDbContext(string connectionString) : base()
        {
            this.connectionString = connectionString;
        }

        public EfDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseSqlServer(connectionString, (builder) => ApplyContextConfiguration(builder));
        }

        public static void ApplyContextConfiguration(SqlServerDbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.CommandTimeout(20)
                          .EnableRetryOnFailure(5, TimeSpan.FromMilliseconds(300), Array.Empty<int>());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>();
        }
    }
}