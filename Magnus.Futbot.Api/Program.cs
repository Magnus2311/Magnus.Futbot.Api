using AutoMapper;
using Magnus.Futbot.Api;
using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Api.Services.Connections.SignalR;
using Magnus.Futbot.Common.Interfaces;
using Magnus.Futbot.Database.Repositories;
using Magnus.Futbot.Initializer;
using Magnus.Futbot.Initializer.Connections;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Automapper
builder.Services
    .AddSingleton(new MapperConfiguration(mc =>
    {
        mc.AddProfile(new MapperProfile());
    }).CreateMapper());

// SignalR Connections
builder.Services
    .AddTransient<ProfilesConnection>()
    .AddTransient<PlayersConnection>();

// Repositories
builder.Services
    .AddTransient<ProfilesRepository>()
    .AddTransient<PlayersRepository>();

// Http Clients
builder.Services
    .AddHttpClient<SsoConnectionService>();
builder.Services
    .AddHttpClient<EaConnectionService>();

// Services
builder.Services
    .AddTransient<ProfilesService>()
    .AddTransient<IPlayersService, PlayersService>();

// Caches
builder.Services
    .AddSingleton<PlayersCache>();

// Background workers
builder
    .Services
    .AddHostedService<RefreshPlayersWorker>();

builder.Services
    .AddSingleton<IUserIdProvider, UserProvider>();

builder.Services.AddSignalR();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("corsapp");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProfilesHub>("/hubs/profiles");
    endpoints.MapHub<PlayersHub>("/hubs/players");
});

app.Run();
