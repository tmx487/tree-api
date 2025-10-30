using api.Domain;
using api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Infrastructure.Configurations;

public class PartnerSecretConfiguration : IEntityTypeConfiguration<PartnerSecret>
{
    public void Configure(EntityTypeBuilder<PartnerSecret> builder)
    {
        builder.ToTable("PartnerSecretCodes");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.HasIndex(c => c.SecretCode)
            .IsUnique();

        builder.Property(c => c.SecretCode)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasOne<Partner>()
            .WithMany()
            .HasForeignKey(c => c.PartnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(c => c.CreatedAt)
            .IsRequired();
    }
}