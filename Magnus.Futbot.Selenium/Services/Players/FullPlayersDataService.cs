using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Interfaces.Helpers;
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

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public IEnumerable<TransferCard> GetAvailableTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            var cards = _cardsHelper.GetAllCards();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

            foreach (var player in players)
            {
                var transferCard = new TransferCard();
                var (Name, Rating) = player.ConvertPlayerElementToPlayerCard();
                IWebElement? canvas = null;

                var availableCards = cards.Where(c => c.Rating == Rating && c.Name.Contains(Name));
                if (availableCards.Count() == 1) 
                    yield return new TransferCard
                    {
                        Card = availableCards.FirstOrDefault()!,
                        PlayerCardStatus = PlayerCardStatus.Won,
                    };


                canvas ??= player.TryFindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.specials.ut-item-loaded > canvas"));
                if (canvas is not null)
                {
                    var currentCards = availableCards.Where(c => c.)
                }


                if (canvas is null)
                {

                    try
                    {
                        canvas = player.FindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.common.ut-item-loaded > canvas"));
                    }
                    catch { }
                }

                if (canvas is null)
                {
                    canvas = player.FindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.rare.ut-item-loaded > canvas"));

                }

                // Getting card photo and should check it later for player type
                // current.PlayerType should be set accordingly
                var image = driver.ExecuteScript("return arguments[0].toDataURL('image/png').substring(22);", canvas);
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                currentPlayer.PromoType = PlayerCardType.TOTW;
                yield return currentPlayer;
            }
        }

        public IEnumerable<TransferCard> GetActiveTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(4) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
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

        public static IEnumerable<TransferCard> GetUnassignedItems(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenUnassignedItems();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-unassigned-view.ui-layout-left > section > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<TransferCard> GetTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferTargets();
            var transferTargets = new List<PlayerCard>();
            transferTargets.AddRange(GetLostTransferTargets(profileDTO));
            transferTargets.AddRange(GetWonTransferTargets(profileDTO));
            transferTargets.AddRange(GetActiveTransferTargets(profileDTO));
            return transferTargets;
        }

        public static IEnumerable<TransferCard> GetLostTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(4) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Outbidded;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<TransferCard> GetWonTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<TransferCard> GetActiveTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(1) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Pending;
                yield return currentPlayer;
            }
        }
    }
}
