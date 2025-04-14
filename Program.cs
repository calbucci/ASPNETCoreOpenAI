var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddHttpClient<IChatService, ChatService>();

DotNetEnv.Env.Load();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ChatHub>("/chatHub");

app.Run();
