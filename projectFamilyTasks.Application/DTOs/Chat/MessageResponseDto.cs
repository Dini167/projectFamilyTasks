namespace projectFamilyTasks.Application.DTOs.Chat;

public class MessageResponseDto
{
    public int Id { get; init; }
    public int SenderId { get; init; }
    public string SenderName { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
    public int? SwapRequestId { get; init; }
}
