using AutoMapper;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Responses;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using Magnus.Futbot.Services;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ProfilesService
    {
        private readonly IMapper _mapper;
        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesHub;
        private readonly IConfiguration _configuration;
        private readonly ProfilesRepository _profilesRepository;

        public ProfilesService(IHubContext<ProfilesHub, IProfilesClient> profilesHub,
            ProfilesRepository profilesRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _profilesHub = profilesHub;
            _configuration = configuration;
            _mapper = mapper;
            _profilesRepository = profilesRepository;
        }

        public async Task<IEnumerable<ProfileDTO>> GetAll(string userId)
            => _mapper.Map<IEnumerable<ProfileDTO>>(await _profilesRepository.GetAll(new ObjectId(userId)));

        public async Task<LoginResponseDTO> Add(AddProfileDTO profileDTO)
        {
            var response = InitProfileService.InitProfile(profileDTO);
            await _profilesRepository.Add(_mapper.Map<ProfileDocument>(response));

            return new LoginResponseDTO(ProfileStatusType.WrongCredentials, _mapper.Map<ProfileDTO>(profileDTO));
        }

        public async Task<ProfileDTO> SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var response = LoginSeleniumService.SubmitCode(submitCodeDTO);
            return _mapper.Map<ProfileDTO>(await _profilesRepository.UpdateSubmitCodeStatus(submitCodeDTO.Email, response));
        }
    }
}
