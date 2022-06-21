using System.Diagnostics;
using AutoMapper;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Services.Connections.SignalR;
using Magnus.Futbot.Api.Services.Selenium;

namespace Magnus.Futbot.Api.Helpers
{
    public class Initializer
    {
        private readonly ProfilesService _profilesService;
        private readonly LoginSeleniumService _loginSeleniumService;
        private readonly InitProfileSeleniumService _initProfileSeleniumService;
        private readonly DataSeleniumService _dataSeleniumService;
        private readonly IMapper _mapper;
        private readonly ProfilesConnection _profilesConnection;

        public Initializer(ProfilesService profilesService,
            LoginSeleniumService loginSeleniumService,
            InitProfileSeleniumService initProfileSeleniumService,
            DataSeleniumService dataSeleniumService,
            IMapper mapper,
            ProfilesConnection profilesConnection)
        {
            _profilesService = profilesService;
            _loginSeleniumService = loginSeleniumService;
            _initProfileSeleniumService = initProfileSeleniumService;
            _dataSeleniumService = dataSeleniumService;
            _mapper = mapper;
            _profilesConnection = profilesConnection;
        }

        public void Init()
        {
            //CloseAllChromeInstances();
            InitSeleniumProfiles();
        }

        public void CloseAllChromeInstances()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            foreach (var chrome in procsChrome)
            {
                chrome.Kill();
                chrome.WaitForExit();
                chrome.Dispose();
            }
        }

        public async void InitSeleniumProfiles()
        {
            var profiles = new HashSet<AddProfileDTO>(await _profilesService.GetAll());
            var tasks = new List<Task>();
            foreach (var addProfileDTO in profiles)
                tasks.Add(Task.Run(async () =>
                {
                    var response = _initProfileSeleniumService.InitProfile(addProfileDTO);
                    await _profilesService.UpdateStatusByEmail(addProfileDTO.Email, response.PofileStatus);
                    if (response.PofileStatus == Common.ProfileStatusType.Logged)
                    {
                        var profileDTO = _dataSeleniumService.GetBasicData(_mapper.Map<ProfileDTO>(addProfileDTO));
                        await _profilesService.Update(profileDTO);
                        await _profilesConnection.UpdateProfile(profileDTO);
                    }
                }));

            await Task.WhenAll(tasks);
        }
    }
}