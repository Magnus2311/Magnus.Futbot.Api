using AutoMapper;
using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Responses;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using Magnus.Futbot.Selenium.Services.Players;
using Magnus.Futbot.Services;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ProfilesService
    {
        private readonly IMapper _mapper;
        private readonly ProfilesRepository _profilesRepository;

        public ProfilesService(
            ProfilesRepository profilesRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _profilesRepository = profilesRepository;
        }

        public async Task<IEnumerable<ProfileDTO>> GetAll(string userId)
            => _mapper.Map<IEnumerable<ProfileDTO>>(await _profilesRepository.GetAll(new ObjectId(userId)));

        public async Task<ProfileDTO> GetByEmail(string email)
            => _mapper.Map<ProfileDTO>((await _profilesRepository.GetByEmail(email))?.FirstOrDefault());

        public async Task<LoginResponseDTO> Add(AddProfileDTO profileDTO)
        {
            await _profilesRepository.Add(_mapper.Map<ProfileDocument>(profileDTO));
            if ((await _profilesRepository.GetByEmail(profileDTO.Email)).Any())
                return new LoginResponseDTO(ProfileStatusType.AlreadyAdded, _mapper.Map<ProfileDTO>(profileDTO));

            var response = InitProfileService.InitProfile(profileDTO);
            await _profilesRepository.Update(_mapper.Map<ProfileDocument>(response));

            return new LoginResponseDTO(response.Status, _mapper.Map<ProfileDTO>(profileDTO));
        }

        public async Task<ProfileDTO> SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var response = LoginSeleniumService.SubmitCode(submitCodeDTO);
            return _mapper.Map<ProfileDTO>(await _profilesRepository.UpdateSubmitCodeStatus(submitCodeDTO.Email, response));
        }

        public async Task<ProfileDTO> RefreshProfile(string profileId, string userId)
        {
            var profile = await _profilesRepository.Get(new ObjectId(profileId), new ObjectId(userId));
            var refreshedProfile = InitProfileService.InitProfile(_mapper.Map<ProfileDTO>(profile));
            refreshedProfile.TradePile = FullPlayersDataService.GetTransferPile(refreshedProfile);
            await _profilesRepository.Update(_mapper.Map<ProfileDocument>(refreshedProfile));
            return refreshedProfile;
        }
    }
}
