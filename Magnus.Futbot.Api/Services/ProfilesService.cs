using AutoMapper;
using Confluent.Kafka;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Kafka.Consumers;
using Magnus.Futbot.Api.Kafka.Producers;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Responses;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Microsoft.AspNetCore.SignalR;

namespace Magnus.Futbot.Api.Services
{
    public class ProfilesService
    {
        private readonly IMapper _mapper;
        private readonly ProfileProducer _profileProducer;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHub;
        private readonly IConfiguration _configuration;

        public ProfilesService(ProfileProducer profileProducer,
            IHubContext<ProfilesHub, IProfilesClient> profilesHub,
            IConfiguration configuration,
            IMapper mapper)
        {
            _profileProducer = profileProducer;
            _profilesHub = profilesHub;
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<ProfileDTO> GetAll(string userId)
        {
            var profilesConsumer = new UserProfilesConsumer(_configuration, userId);
            var profiles = new Dictionary<string, ProfileDTO>();

            profilesConsumer.Subscribe();

            while (true)
            {
                var message = profilesConsumer.Consumer.Consume(TimeSpan.FromMilliseconds(150));
                if (message is null) break;

                if (profiles.ContainsKey(message.Message.Value.Email))
                    profiles[message.Message.Value.Email] = message.Message.Value;
                else
                    profiles.Add(message.Message.Value.Email, message.Message.Value);
            }

            return profiles.Values;
        }

        public async Task<LoginResponseDTO> Add(AddProfileDTO profileDTO)
        {
            await _profileProducer.Produce(profileDTO);

            return new LoginResponseDTO(ProfileStatusType.WrongCredentials, _mapper.Map<ProfileDTO>(profileDTO));
        }

        public async Task<ConfirmationCodeResponseDTO> SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var profile = new ProfileDocument();

            return new ConfirmationCodeResponseDTO(ConfirmationCodeStatusType.WrongCode, _mapper.Map<ProfileDTO>(profile));
        }
    }
}
