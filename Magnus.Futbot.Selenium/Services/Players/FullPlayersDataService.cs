using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Trading;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Players
{
    public class FullPlayersDataService : BaseService
    {
        public static ProfileDTO FetchTradingPlayers(ProfileDTO profile)
        {
            return profile;
        }

        public static TradePile GetTransferPile(ProfileDTO profileDTO)
        {
            var transferPile = new TradePile
            {
                TransferTargets = GetTransferTargets(profileDTO).ToList(),
                TransferList = GetTransferListCards(profileDTO).ToList(),
                UnassignedItems = GetUnassignedItems(profileDTO).ToList()
            };

            return transferPile;
        }

        public static IEnumerable<PlayerCard> GetUnsoldTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<PlayerCard> GetAvailableTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

            foreach (var player in players)
            {
                var canvas = player.FindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.specials.ut-item-loaded > canvas"));
                // Getting card photo and should check it later for player type
                var image = driver.ExecuteScript("return arguments[0].toDataURL('image/png').substring(22);", canvas);
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<PlayerCard> GetActiveTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(4) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<PlayerCard> GetTransferListCards(ProfileDTO profileDTO)
        {
            var transferList = new List<PlayerCard>();

            transferList.AddRange(GetUnsoldTransferListCards(profileDTO));
            transferList.AddRange(GetAvailableTransferListCards(profileDTO));
            transferList.AddRange(GetActiveTransferListCards(profileDTO));

            return transferList;
        }

        public static IEnumerable<PlayerCard> GetUnassignedItems(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-unassigned-view.ui-layout-left > section > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<PlayerCard> GetTransferTargets(ProfileDTO profileDTO)
        {
            var transferTargets = new List<PlayerCard>();
            transferTargets.AddRange(GetLostTransferTargets(profileDTO));
            transferTargets.AddRange(GetWonTransferTargets(profileDTO));
            transferTargets.AddRange(GetActiveTransferTargets(profileDTO));
            return transferTargets;
        }

        public static IEnumerable<PlayerCard> GetLostTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(4) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Outbidded;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<PlayerCard> GetWonTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

            foreach (var player in players)
            {
                var currentPlayer = player.ConvertPlayerElementToPlayerCard();
                currentPlayer.PlayerCardStatus = PlayerCardStatus.Won;
                yield return currentPlayer;
            }
        }

        public static IEnumerable<PlayerCard> GetActiveTransferTargets(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

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
