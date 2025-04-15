using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;

public class ChatHub : Hub
{
    public readonly IChatClient _chatClient;
    private List<ChatMessage> _chatHistory = new List<ChatMessage>();

    public ChatHub(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task SendMessage(string message)
    {
        _chatHistory.Add(new ChatMessage(ChatRole.User, message));

        var response = new StringBuilder();
        await foreach (ChatResponseUpdate item in _chatClient.GetStreamingResponseAsync(_chatHistory))
        {
            response.Append(item.Text);
            await Clients.Caller.SendAsync("ReceiveToken", item.Text);
        }

        await Clients.Caller.SendAsync("ReceiveMessage", "[BREAK]");
        _chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.ToString()));

    }
}