using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Models.Database.Card;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Trading;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Players
{
    public class FullPlayersDataService : BaseService
    {
        private readonly ICardsHelper _cardsHelper;

        public FullPlayersDataService(ICardsHelper cardsHelper)
        {
            _cardsHelper = cardsHelper;
        }

        public static ProfileDTO FetchTradingPlayers(ProfileDTO profile)
        {
            return profile;
        }

        public TradePile GetTransferPile(ProfileDTO profileDTO)
        {
            var transferPile = new TradePile
            {
                TransferTargets = GetTransferTargets(profileDTO).ToList(),
                TransferList = GetTransferListCards(profileDTO).ToList(),
                UnassignedItems = GetUnassignedItems(profileDTO).ToList()
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

        public IEnumerable<TransferCard> GetTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenHomePage();
            driver.OpenTransferList();

            var transferList = new List<TransferCard>();

            transferList.AddRange(GetUnsoldTransferListCards(profileDTO));
            transferList.AddRange(GetAvailableTransferListCards(profileDTO));
            transferList.AddRange(GetActiveTransferListCards(profileDTO));

            return transferList;
        }

        public IEnumerable<TransferCard> GetUnassignedItems(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            if (driver.OpenUnassignedItems())
            {
                var cards = _cardsHelper.GetAllCards();

                var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-unassigned-view.ui-layout-left > section > ul > li"));

                foreach (var player in players)
                    yield return player.ConvertCardToTransferCard(cards);
            }
        }

        public IEnumerable<TransferCard> GetTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferTargets();
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
