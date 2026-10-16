namespace projectFamilyTasks.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
