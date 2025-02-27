namespace Omni.Copilot.Models;

public class ChatRequest
{
    public required string TenantId { get; set; }
    public required string UserId { get; set; }
    public required string SessionId { get; set; }
    public required string PromptText { get; set; }
}