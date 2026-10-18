using projectFamilyTasks.Application.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Configurations;

public class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMember>
{
    public void Configure(EntityTypeBuilder<FamilyMember> builder)
    {
        builder.HasDiscriminator<string>("MemberType")
            .HasValue<Parent>(Roles.Parent)
            .HasValue<Child>(Roles.Child);

        builder.HasIndex(m => m.UserName).IsUnique();
    }
}
