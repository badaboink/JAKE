using Server.GameData;
using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGameDataService, InMemoryGameDataService>();

// Add services to the container.
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHub<GameHub>("/gamehub");

app.Run();