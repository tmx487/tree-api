using api.Domain;
using api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Infrastructure.Configurations;

public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("Partners");

        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Role)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();
        
        builder.HasIndex(p => p.Name)
            .IsUnique(true);
               
        builder.Property(p => p.CreatedAt)
            .IsRequired();
    }
}