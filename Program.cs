using CardGames.Hubs;
using CardGames.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add SignalR service for real-time communication
builder.Services.AddSignalR();

// Add BlackjackService
builder.Services.AddSingleton<CardGames.Services.BlackjackService>();

builder.Services.AddLogging(config => 
{
    config.SetMinimumLevel(LogLevel.Debug);
    config.AddConsole();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map the SignalR Hub
app.MapHub<GameHub>("/gameHub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

