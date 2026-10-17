using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projectFamilyTasks.Application.Exceptions;
using projectFamilyTasks.Application.Interfaces.Repositories;

namespace projectFamilyTasks.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var entities = string.Join(", ", ex.Entries.Select(entry => entry.Metadata.ClrType.Name));
            _logger.LogWarning("Concurrency conflict while saving changes to: {Entities}", entities);

            throw new ConflictException(
                "הנתונים שונו בינתיים על ידי משתמש אחר, למשל אח שתפס את אותה משימה. יש לרענן ולנסות שוב.");
        }
    }
}
