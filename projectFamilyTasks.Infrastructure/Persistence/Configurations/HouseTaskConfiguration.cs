using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence.Configurations;

public class HouseTaskConfiguration : IEntityTypeConfiguration<HouseTask>
{
    public void Configure(EntityTypeBuilder<HouseTask> builder)
    {
        builder.HasMany(t => t.Categories)
            .WithMany(c => c.HouseTasks)
            .UsingEntity("HouseTaskCategories");
    }
}
