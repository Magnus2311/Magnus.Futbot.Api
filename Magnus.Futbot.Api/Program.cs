using AutoMapper;
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
using Magnus.Futbot.Initializer;
using Magnus.Futbot.Initializer.Connections;
using Magnus.Futbot.Selenium.Services.Players;
using Magnus.Futbot.Selenium.Services.Trade.Buy;
using Magnus.Futbot.Selenium.Services.Trade.Filters;
using Magnus.Futbot.Selenium.Services.Trade.Sell;
using Magnus.Futbot.Selenium.Trading.Connections;
using Magnus.Futbot.Services;
using Magnus.Futbot.Services.Trade.Buy;
using Magnus.Futbot.Storage;
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
    .AddTransient<PauseActionRepository>();

// Http Clients
builder.Services
    .AddHttpClient<SsoConnectionService>();
builder.Services
    .AddHttpClient<EaConnectionService>();
builder.Services
    .AddHttpClient<TradePileConnection>();

// Services
builder.Services
    .AddTransient<ProfilesService>()
    .AddTransient<IActionsService, ActionsService>()
    .AddTransient<IPlayersService, PlayersService>()
    .AddTransient<ITradingService, TradingService>()
    .AddTransient<ICardsHelper, CardsHelper>()
    .AddTransient<IActionsNotifier, ActionsNotifier>()
    .AddTransient<ActionsDeactivator>();

// Selenium Services
builder.Services
    .AddTransient<BidService>()
    .AddTransient<FullPlayersDataService>()
    .AddTransient<MovePlayersService>()
    .AddTransient<SellService>()
    .AddTransient<FiltersService>()
    .AddTransient<BinService>()
    .AddTransient<DataSeleniumService>()
    .AddTransient<InitProfileService>()
    .AddTransient<LoginSeleniumService>();

// Caches
builder.Services
    .AddSingleton<PlayersCache>()
    .AddSingleton<CardsCache>();

// Background workers
builder.Services
    //.AddHostedService<RefreshPlayersWorker>()
    .AddHostedService<RelistPlayersWorker>()
    .AddHostedService<DeactivateAllActionsOnStartUp>();

// Azure
builder.Services
    .AddSingleton<AzureStorage>();

builder.Services
    .AddSingleton<IUserIdProvider, UserProvider>();

builder.Services.AddSignalR();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("corsapp");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProfilesHub>("/hubs/profiles");
    endpoints.MapHub<PlayersHub>("/hubs/players");
    endpoints.MapHub<CardsHub>("/hubs/cards");
    endpoints.MapHub<ActionsHub>("/hubs/actions");
});

app.Run();
