using AutoMapper;
using Magnus.Futbot.Api;
using Magnus.Futbot.Api.Caches;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Kafka.Consumers;
using Magnus.Futbot.Api.Kafka.Fetchers;
using Magnus.Futbot.Api.Kafka.Producers;
using Magnus.Futbot.Api.Kafka.Producers.Requests;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Api.Services.Connections.SignalR;
using Magnus.Futbot.Database.Repositories;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddSwaggerGen();
builder.Services
    .AddTransient<ProfilesConnection>()
    .AddTransient<PlayersConnection>()
    .AddTransient<ProfilesService>()
    .AddSingleton<PlayersCache>();

builder.Services
    .AddTransient<ProfilesRepository>()
    .AddTransient<PlayersRepository>();

builder.Services
    .AddTransient<ProfilesConsumer>();

builder.Services
    .AddSingleton<ProfilesFetcher>();

builder.Services
    .AddSingleton<UserProfilesConsumer>();

builder.Services
    .AddSingleton<ProfileProducer>()
    .AddSingleton<ProfilesRequest>();

builder.Services
    .AddHttpClient<SsoConnectionService>();

builder
    .Services
    .AddHostedService<ProfilesWorker>();

builder.Services.AddSingleton<IUserIdProvider, UserProvider>();

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
