using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using projectFamilyTasks.Application.Common.Constants;
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

            throw new ConflictException(ErrorMessages.ConcurrencyConflict);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            _logger.LogWarning("Unique value violation while saving changes");
            throw new ConflictException(ErrorMessages.DuplicateValue);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            _logger.LogWarning("Foreign key violation while saving changes");
            throw new ConflictException(ErrorMessages.InUse);
        }
    }
}