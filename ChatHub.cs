using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public readonly IChatService _chatService;
    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }
    public async Task SendMessage(string message)
    {
        await foreach (var token in _chatService.GetChatResponseStream(message))
        {
            await Clients.Caller.SendAsync("ReceiveToken", token);
        }
    }
}