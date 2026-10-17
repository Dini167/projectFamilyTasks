using Microsoft.EntityFrameworkCore;
using projectFamilyTasks.Domain.Common;
using projectFamilyTasks.Domain.Entities;

namespace projectFamilyTasks.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<FamilyMember> Members { get; set; } = null!;
    public DbSet<Parent> Parents { get; set; } = null!;
    public DbSet<Child> Children { get; set; } = null!;
    public DbSet<ParentAccess> ParentAccesses { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<HouseTask> HouseTasks { get; set; } = null!;
    public DbSet<Round> Rounds { get; set; } = null!;
    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<TaskCompletion> TaskCompletions { get; set; } = null!;
    public DbSet<SwapRequest> SwapRequests { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var changedEntries = ChangeTracker.Entries<IHasVersion>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in changedEntries)
        {
            entry.Entity.Version = Guid.NewGuid();
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
