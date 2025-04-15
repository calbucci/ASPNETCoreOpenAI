using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var apiKey = Environment.GetEnvironmentVariable("OpenAIKey");

builder.Services.AddSignalR();
//builder.Services.AddHttpClient<IChatService, ChatService>();

var chatClient = new OpenAIClient(apiKey).GetChatClient("gpt-4o-2024-11-20").AsIChatClient();
builder.Services.AddChatClient(chatClient);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.Run();
