using AutoMapper;
using Confluent.Kafka;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Kafka.Serialization;
using Magnus.Futbot.Initializer;
using Magnus.Futbot.Initializer.Connections;
using Magnus.Futbot.Initializer.Helpers;
using Magnus.Futbot.Initializer.Producers;


var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddSingleton(mapper)
            .AddSingleton<PlayersProducer>()
            .AddSingleton<ISerializer<PlayerDTO>, Serializer<PlayerDTO>>();

        services
            .AddHttpClient<EaConnectionService>();

        services
            .AddHostedService<RefreshPlayersWorker>();
    })
    .Build();

await host.RunAsync();
