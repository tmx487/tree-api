using Microsoft.EntityFrameworkCore;
using TreeAPI.Domain.Entities;

namespace TreeAPI.Infrastructure.Persistence
{
    public class TreeApiDbContext : DbContext
    {
        public TreeApiDbContext(DbContextOptions<TreeApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; } = null!;
        public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
        public DbSet<Tree> Trees { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>(entity =>
            {
                entity.Property(n => n.Id)
                    .ValueGeneratedOnAdd();

                entity.HasKey(n => n.Id);

                entity.HasIndex(n => new {n.TreeId, n.Name}).IsUnique();

                entity.Property(n => n.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity
                .HasOne(n => n.Tree)
                .WithMany(t => t.Nodes)
                .HasForeignKey(n => n.TreeId);

                entity
                .HasOne(n => n.Parent)
                .WithMany(n => n.Children)
                .HasForeignKey(n => n.ParentNodeId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
