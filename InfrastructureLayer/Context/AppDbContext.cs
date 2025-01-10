using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.Context;

public class AppDbContext : IdentityDbContext
{

    public AppDbContext(DbContextOptions options):base(options)
    {

    }

    public DbSet<AppUser> Users { get; set; } = default!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(@"Server=DESKTOP-60DBTKK;Database=LeagueApp;Trusted_Connection=True;TrustServerCertificate=True")
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
            LogLevel.Information);
    }
}
