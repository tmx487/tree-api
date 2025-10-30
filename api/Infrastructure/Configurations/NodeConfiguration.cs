using api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Infrastructure.Configurations;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.ToTable("Nodes");
            
        builder.HasKey(n => n.Id);
            
        builder.Property(n => n.Name).IsRequired().HasMaxLength(255);
        builder.Property(n => n.TreeName).IsRequired().HasMaxLength(255);
        
        builder.HasOne(n => n.Parent)
            .WithMany(n => n.Children)
            .HasForeignKey(n => n.ParentNodeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => new { n.TreeName, n.ParentNodeId, n.Name })
            .IsUnique();
    }
}