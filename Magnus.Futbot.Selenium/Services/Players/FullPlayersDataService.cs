using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Trading;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Players
{
    public class FullPlayersDataService : BaseService
    {
        private readonly ICardsHelper _cardsHelper;

        public FullPlayersDataService(
            IActionsService actionsService,
            ICardsHelper cardsHelper) : base(actionsService)
        {
            _cardsHelper = cardsHelper;
        }

        public static ProfileDTO FetchTradingPlayers(ProfileDTO profile)
        {
            return profile;
        }

        public async Task<TradePile> GetTransferPile(ProfileDTO profileDTO)
        {
            var transferPile = new TradePile
            {
                TransferTargets = (await GetTransferTargets(profileDTO)).ToList(),
                TransferList = (await GetTransferListCards(profileDTO)).ToList(),
                UnassignedItems = (await GetUnassignedItems(profileDTO)).ToList()
            };

            return transferPile;
        }

        public IEnumerable<TransferCard> GetUnsoldTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            var cards = _cardsHelper.GetAllCards();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > ul > li"));

            foreach (var player in players)
                yield return player.ConvertCardToTransferCard(cards);
        }

        public IEnumerable<TransferCard> GetAvailableTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            var cards = _cardsHelper.GetAllCards();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

            foreach (var player in players)
                yield return player.ConvertCardToTransferCard(cards);
        }

        public IEnumerable<TransferCard> GetActiveTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            var cards = _cardsHelper.GetAllCards();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(4) > ul > li"));

            foreach (var player in players)
                yield return player.ConvertCardToTransferCard(cards);
        }

        public async Task<IEnumerable<TransferCard>> GetTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            await driver.OpenHomePage(profileDTO);
            await driver.OpenTransferList();

            var transferList = new List<TransferCard>();

            transferList.AddRange(GetUnsoldTransferListCards(profileDTO));
            transferList.AddRange(GetAvailableTransferListCards(profileDTO));
            transferList.AddRange(GetActiveTransferListCards(profileDTO));

            return transferList;
        }

        public async Task<IEnumerable<TransferCard>> GetUnassignedItems(ProfileDTO profileDTO)
        {
            var outputCards = new List<TransferCard>();

            var driver = GetInstance(profileDTO.Email).Driver;
            if (await driver.OpenUnassignedItems(profileDTO))
            {
                var cards = _cardsHelper.GetAllCards();

                var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-unassigned-view.ui-layout-left > section > ul > li"));

                foreach (var player in players)
                    outputCards.Add(player.ConvertCardToTransferCard(cards));
            }

            return outputCards;
        }

        public async Task<IEnumerable<TransferCard>> GetTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            await driver.OpenTransferTargets();
            var transferTargets = new List<TransferCard>();
            transferTargets.AddRange(GetLostTransferTargets(profileDTO));
            transferTargets.AddRange(GetWonTransferTargets(profileDTO));
            transferTargets.AddRange(GetActiveTransferTargets(profileDTO));
            return transferTargets;
        }

        public IEnumerable<TransferCard> GetLostTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            var cards = _cardsHelper.GetAllCards();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(4) > ul > li"));
            Thread.Sleep(5000);

            foreach (var player in players)
                yield return player.ConvertCardToTransferCard(cards);
        }

        public IEnumerable<TransferCard> GetWonTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            var cards = _cardsHelper.GetAllCards();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

            foreach (var player in players)
                yield return player.ConvertCardToTransferCard(cards);
        }

        public IEnumerable<TransferCard> GetActiveTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            var cards = _cardsHelper.GetAllCards();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(1) > ul > li"));

            foreach (var player in players)
                yield return player.ConvertCardToTransferCard(cards);
        }
    }
}
