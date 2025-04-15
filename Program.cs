using Microsoft.Extensions.AI;
using OpenAI;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddChatClient(
    new OpenAIClient(Environment.GetEnvironmentVariable("OpenAIKey")).GetChatClient("gpt-4o-2024-11-20").AsIChatClient());
builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.Run();
