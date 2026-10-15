using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Configurations;

public class SwapRequestConfiguration : IEntityTypeConfiguration<SwapRequest>
{
    public void Configure(EntityTypeBuilder<SwapRequest> builder)
    {
        builder.HasOne(s => s.RequesterAssignment)
            .WithMany()
            .HasForeignKey(s => s.RequesterAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.TargetAssignment)
            .WithMany()
            .HasForeignKey(s => s.TargetAssignmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
