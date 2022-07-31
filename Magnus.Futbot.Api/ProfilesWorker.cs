using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Kafka.Consumers;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api
{
    public class ProfilesWorker : BackgroundService
    {
        private readonly ProfilesConsumer _profilesConsumer;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHub;

        public ProfilesWorker(ProfilesConsumer profilesConsumer,
            IHubContext<ProfilesHub, IProfilesClient> profilesHub)
        {
            _profilesConsumer = profilesConsumer;
            _profilesHub = profilesHub;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.Run(() =>
            {
                var message = _profilesConsumer.Consume();
                if (message is not null)
                {
                    var profile = message.Message.Value;
                    _profilesHub.Clients.User(profile.UserId).OnProfileUpdated(profile);
                }
            }, stoppingToken);
    }
}
