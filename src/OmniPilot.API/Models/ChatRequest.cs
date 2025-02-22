namespace Omni.Copilot.Models;

public class ChatRequest
{
    public string TenantId { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; }
    public string PromptText { get; set; }
}