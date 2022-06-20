using System.Diagnostics;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Services.Selenium;

namespace Magnus.Futbot.Api.Helpers
{
    public class Initializer
    {
        private readonly ProfilesService _profilesService;
        private readonly LoginSeleniumService _loginSeleniumService;
        private readonly InitProfileSeleniumService _initProfileSeleniumService;

        public Initializer(ProfilesService profilesService,
            LoginSeleniumService loginSeleniumService,
            InitProfileSeleniumService initProfileSeleniumService)
        {
            _profilesService = profilesService;
            _loginSeleniumService = loginSeleniumService;
            _initProfileSeleniumService = initProfileSeleniumService;
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
            foreach (var profileDTO in profiles)
                tasks.Add(Task.Run(async () =>
                {
                    var response = _initProfileSeleniumService.InitProfile(profileDTO);
                    await _profilesService.UpdateStatusByEmail(profileDTO.Email, response.PofileStatus);
                }));

            await Task.WhenAll(tasks);
        }
    }
}