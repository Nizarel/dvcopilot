using Microsoft.AspNetCore.Mvc;
using Omni.Copilot.Services;
using Omni.Copilot.Models;

namespace OmniPilot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OmniChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public OmniChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    // [HttpGet("sessionssessions/{tenantId}")]
    // public async Task<ActionResult<List<Session>>> GetAllSessions()
    // {
    //     var sessions = await _chatService.GetAllChatSessionsAsync();
    //     return Ok(sessions);
    // }

    [HttpGet("sessions/{tenantId}/{userId}")]
    public async Task<IActionResult> GetAllChatSessions(string tenantId, string userId)
    {
        var sessions = await _chatService.GetAllChatSessionsAsync(tenantId, userId);
        return Ok(sessions);
    }

    [HttpGet("sessions/{tenantId}/{userId}/{sessionId}")]
    public async Task<IActionResult> GetChatSessionMessages(string tenantId, string userId, string sessionId)
    {
        var messages = await _chatService.GetChatSessionMessagesAsync(tenantId, userId, sessionId);
        return Ok(messages);
    }

    [HttpPost("sessions/{tenantId}/{userId}")]
    public async Task<IActionResult> CreateNewChatSession(string tenantId, string userId)
    {
        var session = await _chatService.CreateNewChatSessionAsync(tenantId, userId);
        return Ok(session);
    }

    [HttpPost("sessions/{tenantId}/{userId}/{sessionId}/rename")]
    public async Task<IActionResult> RenameChatSession(string tenantId, string userId, string sessionId, [FromBody] string newChatSessionName)
    {
        await _chatService.RenameChatSessionAsync(tenantId, userId, sessionId, newChatSessionName);
        return Ok();
    }

    [HttpDelete("sessions/{tenantId}/{userId}/{sessionId}")]
    public async Task<IActionResult> DeleteChatSession(string tenantId, string userId, string sessionId)
    {
        await _chatService.DeleteChatSessionAsync(tenantId, userId, sessionId);
        return Ok();
    }

    [HttpPost("completion")]
    public async Task<IActionResult> GetChatCompletion([FromBody] ChatRequest request)
    {
        var chatMessage = await _chatService.GetChatCompletionAsync(request.TenantId, request.UserId, request.SessionId, request.PromptText);
        return Ok(chatMessage);
    }

    [HttpPost("summarize/{tenantId}/{userId}/{sessionId}")]
    public async Task<IActionResult> SummarizeChatSessionName(string tenantId, string userId, string sessionId)
    {
        var summary = await _chatService.SummarizeChatSessionNameAsync(tenantId, userId, sessionId);
        return Ok(summary);
    }

    [HttpPost("cache/clear")]
    public async Task<IActionResult> ClearCache()
    {
        await _chatService.ClearCacheAsync();
        return Ok();
    }

    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize()
    {
        await _chatService.InitializeAsync();
        return Ok();
    }
}

