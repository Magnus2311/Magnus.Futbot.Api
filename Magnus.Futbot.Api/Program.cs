using AutoMapper;
using Magnus.Futbot;
using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Api.Services.Connections.SignalR;
using Magnus.Futbot.Api.Services.Helpers;
using Magnus.Futbot.Api.Services.Interfaces;
using Magnus.Futbot.Api.Services.Notifiers;
using Magnus.Futbot.Api.Workers;
using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Interfaces.Notifiers;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Database.Repositories;
using Magnus.Futbot.Database.Repositories.Actions;
using Magnus.Futbot.Initializer.Connections;
using Magnus.Futbot.Selenium.Trading.Connections;
using Magnus.Futbot.Services;
using Magnus.Futtbot.Connections.Connection;
using Magnus.Futtbot.Connections.Connection.Moving;
using Magnus.Futtbot.Connections.Connection.Trading;
using Magnus.Futtbot.Connections.Connection.Trading.Buy;
using Magnus.Futtbot.Connections.Connection.Trading.Sell;
using Magnus.Futtbot.Connections.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;

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
    .AddTransient<PlayersConnection>()
    .AddTransient<CardsConnection>();

// Repositories
builder.Services
    .AddTransient<ProfilesRepository>()
    .AddTransient<PlayersRepository>()
    .AddTransient<CardsRepository>()
    .AddTransient<MoveActionRepository>()
    .AddTransient<BuyActionRepository>()
    .AddTransient<SellActionRepository>()
    .AddTransient<PauseActionRepository>()
    .AddTransient<TradesRepository>()
    .AddTransient<PriceRepository>();

builder.Services.AddMemoryCache();

// Http Clients
builder.Services.AddHttpClient<SsoConnectionService>();
builder.Services.AddHttpClient<EaConnectionService>();
builder.Services.AddHttpClient<TradePileConnection>();
builder.Services.AddHttpClient<BidConnection>();
builder.Services.AddHttpClient<TransferMarketCardsConnection>();
builder.Services.AddHttpClient<SendItemsConnection>();
builder.Services.AddHttpClient<SellConnection>();
builder.Services.AddHttpClient<GetUserPileConnection>();
builder.Services.AddHttpClient<ClearSoldConnection>();

// Services
builder.Services
    .AddTransient<ProfilesService>()
    .AddTransient<IActionsService, ActionsService>()
    .AddTransient<IPlayersService, PlayersService>()
    .AddTransient<ITradingService, TradingService>()
    .AddTransient<ICardsHelper, CardsHelper>()
    .AddTransient<IActionsNotifier, ActionsNotifier>()
    .AddTransient<ProfilesNotifier>()
    .AddTransient<ActionsDeactivator>()
    .AddTransient<TradeHistoryService>()
    .AddTransient<PriceService>();

// Selenium Services
builder.Services
    .AddTransient<LoginSeleniumService>();

// Trading Services
builder.Services
    .AddTransient<BuyService>()
    .AddTransient<MoveService>()
    .AddTransient<SellService>()
    .AddTransient<ProfileService>()
    .AddTransient<UserActionsService>();

// Caches
builder.Services
    .AddSingleton<PlayersCache>()
    .AddSingleton<CardsCache>();

// Background workers
builder.Services
    //.AddHostedService<RefreshPlayersWorker>()
    //.AddHostedService<RelistPlayersWorker>()
    .AddHostedService<DeactivateAllActionsOnStartUp>();

builder.Services
    .AddSingleton<IUserIdProvider, UserProvider>()
    .AddSingleton<Initializer>();

builder.Services.AddSignalR();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Services.GetRequiredService<Initializer>();

app.UseCors("corsapp");
app.UseHttpsRedirection();

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProfilesHub>("/hubs/profiles");
    endpoints.MapHub<PlayersHub>("/hubs/players");
    endpoints.MapHub<CardsHub>("/hubs/cards");
    endpoints.MapHub<ActionsHub>("/hubs/actions");
});
app.MapControllers();

app.Run();
