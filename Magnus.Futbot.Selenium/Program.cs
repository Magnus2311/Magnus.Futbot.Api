using Magnus.Futbot.Selenium.Consumers;
using Magnus.Futbot.Selenium.Procucers;
using Magnus.Futbot.Selenium.Workers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<InitProfileWorker>()
            .AddHostedService<SecurityCodeWorker>();

        services
            .AddSingleton<ProfilesProducer>()
            .AddSingleton<SecurityCodeProducer>()
            .AddSingleton<InitProfileConsumer>()
            .AddSingleton<SecurityCodeConsumer>();
    })
    .Build();

await host.RunAsync();
