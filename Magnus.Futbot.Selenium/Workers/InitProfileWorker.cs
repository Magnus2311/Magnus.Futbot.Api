using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Selenium.Consumers;
using Magnus.Futbot.Selenium.Procucers;

namespace Magnus.Futbot.Selenium.Workers
{
    public class InitProfileWorker : BackgroundService
    {
        private readonly InitProfileConsumer _initProfileConsumer;
        private readonly ProfilesProducer _profilesProducer;

        public InitProfileWorker(InitProfileConsumer initProfileConsumer, 
            ProfilesProducer profilesProducer)
        {
            _initProfileConsumer = initProfileConsumer;
            _profilesProducer = profilesProducer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var profileDTO = _initProfileConsumer.Consume(stoppingToken);
                    //var response = InitProfileService.InitProfile(profileDTO.Message.Value);
                    _profilesProducer.Produce(new ProfileDTO() { Coins = 123123123 });
                }

                _initProfileConsumer.Consumer.Dispose();
                _initProfileConsumer.Consumer.Close();
            }, stoppingToken);
    }
}
