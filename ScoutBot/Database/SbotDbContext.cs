using Microsoft.EntityFrameworkCore;
using ScoutBot.Database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutBot.Database;

public class SbotDbContext : DbContext
{
    public DbSet<DbGuild> Guilds { get; set; }
    public DbSet<DbUnit> Units { get; set; }
    public DbSet<DbUser> Users { get; set; }
    public DbSet<DbGuildUser> GuildUsers { get; set; }
    public DbSet<DbGuildUserProperty> GuildUserProperties { get; set; }
    public DbSet<DbUnitMember> UnitMembers { get; set; }
    public DbSet<DbPatrol> Patrols { get; set; }
    public DbSet<DbPatrolMember> PatrolMembers { get; set; }
    public DbSet<DbProperty> Properties { get; set; }

    string _connectionString;


    public SbotDbContext(DbContextOptions<SbotDbContext> options)
        : base(options)
    {
        _connectionString = string.Empty;
    }


    public SbotDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
