using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Plutus.Infrastructure.Data;

namespace Plutus.Infrastructure.Date.Migrations;

// public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
// {
//     private const string ConnectionString =
//         "Server=(localdb)\\LocalDB;Database=db;Integrated Security=true;";
//
//     public AppDbContext CreateDbContext(string[] args)
//     {
//         var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
//         // optionsBuilder.UseSqlServer(ConnectionString);
//
//         return new AppDbContext(optionsBuilder.Options);
//     }
// }
//
