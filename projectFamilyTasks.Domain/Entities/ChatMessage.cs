using projectFamilyTasks.Domain.Common;

namespace projectFamilyTasks.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public int SenderId { get; set; }
    public FamilyMember Sender { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public int? SwapRequestId { get; set; }
    public SwapRequest? SwapRequest { get; set; }
}
