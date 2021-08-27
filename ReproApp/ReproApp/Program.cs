using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ReproApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const string connString = @"data source=.;initial catalog=TransactionIssueRepro;integrated security=True";

            using(var dbContext = new EfDbContext(connString))
            {
                dbContext.Database.EnsureCreated();

                dbContext.Add(new Entity() { Name = "Entity" + DateTime.Now.Ticks });
                dbContext.SaveChanges();
            }

            using(var connection = new SqlConnection(connString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                var options = new DbContextOptionsBuilder<EfDbContext>()
                                .UseSqlServer(connection, (builder) => EfDbContext.ApplyContextConfiguration(builder))
                                .Options;
                var dbContext = new EfDbContext(options);

                /*
                Next line throws:
                System.InvalidOperationException: 
                'ExecuteReader requires the command to have a transaction when the connection assigned to the command is in a pending local transaction.  The Transaction property of the command has not been initialized.'
                */
                var result = dbContext.Set<Entity>().ToList();
                Console.WriteLine($"Found {result.Count} matching records");

                transaction.Commit();
            }
        }
    }
}