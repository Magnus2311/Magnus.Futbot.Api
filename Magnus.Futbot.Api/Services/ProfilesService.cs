using AutoMapper;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Services.Selenium;
using Magnus.Futbot.Common;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class ProfilesService
    {
        private readonly IMapper _mapper;
        private readonly ProfilesRepository _profilesRepository;
        private readonly LoginSeleniumService _loginSeleniumService;

        public ProfilesService(ProfilesRepository profilesRepository,
            IMapper mapper, LoginSeleniumService loginSeleniumService)
        {
            _profilesRepository = profilesRepository;
            _mapper = mapper;
            _loginSeleniumService = loginSeleniumService;
        }

        public async Task<IEnumerable<ProfileDTO>> GetAll(ObjectId userId)
            => _mapper.Map<IEnumerable<ProfileDTO>>(await _profilesRepository.GetAll(userId));

        public async Task<IEnumerable<AddProfileDTO>> GetAll()
            => _mapper.Map<IEnumerable<AddProfileDTO>>(await _profilesRepository.GetAll());

        public async Task<LoginResponseDTO> Add(AddProfileDTO profileDTO)
        {

            var entity = await _profilesRepository.Add(_mapper.Map<ProfileDocument>(profileDTO));

            LoginResponseDTO? loginResponseDTO;
            try
            {
                loginResponseDTO = _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
            }
            catch
            {
                loginResponseDTO = new LoginResponseDTO(ProfileStatusType.UnknownError, new ProfileDTO() { Email = profileDTO.Email });

            }

            entity.ProfilesStatus = loginResponseDTO.LoginStatus;
            await _profilesRepository.Update(entity);

            return loginResponseDTO;
        }

        public async Task<ConfirmationCodeResponseDTO> SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var profile = new ProfileDocument();
            var statusType = _loginSeleniumService.SubmitCode(submitCodeDTO.Email, submitCodeDTO.Code);
            if (statusType == ConfirmationCodeStatusType.Successful)
            {
                profile = (await _profilesRepository.GetAll(submitCodeDTO.UserId))
                    .FirstOrDefault(p => p.Email.ToUpper() == submitCodeDTO.Email.ToUpper());
                if (profile is not null)
                {
                    profile.ProfilesStatus = ProfileStatusType.Logged;
                    await _profilesRepository.Update(profile);

                }
            }

            return new ConfirmationCodeResponseDTO(statusType, _mapper.Map<ProfileDTO>(profile));
        }

        public async Task UpdateStatusByEmail(string email, ProfileStatusType profileStatus)
            => (await _profilesRepository.GetByEmail(email)).ToList().ForEach(async (p) => await _profilesRepository.Update(p));
    }
}
