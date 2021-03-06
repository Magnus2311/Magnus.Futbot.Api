using AutoMapper;
using Magnus.Futbot.Api.Kafka.Producers;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Responses;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ProfilesService
    {
        private readonly IMapper _mapper;
        private readonly ProfilesRepository _profilesRepository;
        private readonly ProfileProducer _profileProducer;

        public ProfilesService(ProfilesRepository profilesRepository,
            ProfileProducer profileProducer,
            IMapper mapper)
        {
            _profilesRepository = profilesRepository;
            _profileProducer = profileProducer;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProfileDTO>> GetAll(ObjectId userId)
            => _mapper.Map<IEnumerable<ProfileDTO>>(await _profilesRepository.GetAll(userId));

        public async Task<IEnumerable<AddProfileDTO>> GetAll()
            => _mapper.Map<IEnumerable<AddProfileDTO>>(await _profilesRepository.GetAll());

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
