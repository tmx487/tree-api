using api.Domain;
using api.Domain.Entities;
using api.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure;

public class PsqlDbContext : DbContext
{
    public PsqlDbContext(DbContextOptions<PsqlDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Node> Nodes { get; init; }
    public DbSet<ExceptionJournalEntry> Journal { get; init; }
    public DbSet<Partner> Partners { get; init; }
    public DbSet<PartnerSecret> SecretCodes { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NodeConfiguration());
        modelBuilder.ApplyConfiguration(new JournalConfiguration());
        modelBuilder.ApplyConfiguration(new PartnerConfiguration());
        modelBuilder.ApplyConfiguration(new PartnerSecretConfiguration());
            
        base.OnModelCreating(modelBuilder);
    }
}