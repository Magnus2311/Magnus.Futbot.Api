using AutoMapper;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Services.Selenium;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;

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

        public async Task<IEnumerable<ProfileDTO>> GetAll()
            => _mapper.Map<IEnumerable<ProfileDTO>>(await _profilesRepository.GetAll());

        public async Task<LoginResponseDTO> Add(ProfileDTO profileDTO)
        {
            await _profilesRepository.Add(_mapper.Map<ProfileDocument>(profileDTO));

            LoginResponseDTO? loginResponseDTO;
            try
            {
                loginResponseDTO = _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
            }
            catch
            {
                loginResponseDTO = new LoginResponseDTO(LoginStatusType.UnknownError);
            }

            return loginResponseDTO;
        }

        public async Task<ConfirmationCodeResponseDTO> SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var response = _loginSeleniumService.SubmitCode(submitCodeDTO.Email, submitCodeDTO.Code);
            if (response.Status == ConfirmationCodeStatusType.Successful)
            {
                var profile = (await _profilesRepository.GetAll())
                    .FirstOrDefault(p => p.Email.ToUpper() == submitCodeDTO.Email.ToUpper());
                if (profile is not null)
                {
                    profile.IsCodeConfirmed = true;
                    await _profilesRepository.Update(profile);
                }
            }

            return response;
        }
    }
}
