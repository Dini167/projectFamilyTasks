using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasOne(a => a.Round)
            .WithMany(r => r.Assignments)
            .HasForeignKey(a => a.RoundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.HouseTask)
            .WithMany()
            .HasForeignKey(a => a.HouseTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Child)
            .WithMany()
            .HasForeignKey(a => a.ChildId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => new { a.RoundId, a.HouseTaskId }).IsUnique();

        builder.Property(a => a.Version).IsConcurrencyToken();
    }
}
