using api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Infrastructure.Configurations;

public class JournalConfiguration  : IEntityTypeConfiguration<ExceptionJournalEntry>
{
    public void Configure(EntityTypeBuilder<ExceptionJournalEntry> builder)
    {
        builder.ToTable("ExceptionJournal");
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.EventId).IsUnique();
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
            
        builder.Property(e => e.ExceptionType).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Message).IsRequired();
        builder.Property(e => e.StackTrace).HasColumnType("text").HasMaxLength(5000); 

        builder.Property(e => e.RequestPath).HasMaxLength(1024);
        builder.Property(e => e.HttpMethod).HasMaxLength(10);
        
        builder.Property(e => e.QueryParameters).HasColumnType("text");
        builder.Property(e => e.BodyParameters).HasColumnType("text");
    }
}