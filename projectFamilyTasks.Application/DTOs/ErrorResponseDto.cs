namespace projectFamilyTasks.Application.DTOs;

public class ErrorResponseDto
{
    public int Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? CorrelationId { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }
}
