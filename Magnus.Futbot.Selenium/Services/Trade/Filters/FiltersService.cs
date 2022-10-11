using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Trade.Filters
{
    public class FiltersService : BaseService
    {
        public async Task InsertFilters(string email, BuyCardDTO buyCardDTO)
        {
            var driver = GetInstance(email).Driver;
            await driver.OpenSearchTransfer();

            var resetBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(1)"), 1000);
            resetBtn?.Click();

            if (buyCardDTO.Card is not null)
            {
                var playerNameInput = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-player-search-control > div > div.ut-player-search-control--input-container > input"), 5000);
                if (playerNameInput is null)
                {
                    Console.WriteLine("Name input ccanot be found!");
                    return;
                }

                playerNameInput.SendKeys(buyCardDTO.Card.Name);
                await Task.Delay(300);
                var playerRow = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-player-search-control.has-selection.contract-text-input.is-open > div > div.inline-list > ul > button"), TimeSpan.FromSeconds(10));
                if (playerRow is null)
                {
                    Console.WriteLine("Player row is null!");
                    await InsertFilters(email, buyCardDTO);
                }
                else
                {
                    playerRow.Click();
                }
                await Task.Delay(500);
            }

            if (buyCardDTO.Quality != "Any")
            {
                driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div:nth-child(2)"))
                    .Click();

                var options = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-search-filter-control.has-default.has-image.is-open > div > ul > li"));
                options.FirstOrDefault(o => o.Text == buyCardDTO.Quality)?.Click();
            }

            if (buyCardDTO.Rarity != "Any")
            {
                driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div:nth-child(3)"))
                    .Click();

                var options = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-search-filter-control.has-default.has-image.is-open > div > ul > li"));
                options.FirstOrDefault(o => o.Text == buyCardDTO.Rarity)?.Click();
            }

            if (buyCardDTO.Position != "Any")
            {
                driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div:nth-child(4)"))
                    .Click();

                var options = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-search-filter-control.has-default.has-image.is-open > div > ul > li"));
                options.FirstOrDefault(o => o.Text == buyCardDTO.Position)?.Click();
            }

            if (buyCardDTO.Chemistry != "Any")
            {
                driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div:nth-child(5)"))
                    .Click();

                var options = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-search-filter-control.has-default.has-image.is-open > div > ul > li"));
                options.FirstOrDefault(o => o.Text == buyCardDTO.Chemistry)?.Click();
            }

            if (buyCardDTO.Nationallity != "Any")
            {
                driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div:nth-child(6)"))
                    .Click();

                var options = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-search-filter-control.has-default.has-image.is-open > div > ul > li"));
                options.FirstOrDefault(o => o.Text == buyCardDTO.Nationallity)?.Click();
            }

            if (buyCardDTO.League != "Any")
            {
                driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div:nth-child(7)"))
                    .Click();

                var options = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-search-filter-control.has-default.has-image.is-open > div > ul > li"));
                options.FirstOrDefault(o => o.Text == buyCardDTO.League)?.Click();
            }

            var searchItems = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-player-search-control.is-open.has-selection.contract-text-input > div > div.inline-list > ul > button"), 1000);
            if (searchItems is null) return;

            foreach (var item in searchItems)
            {
                if (item is null) continue;

                var playerName = item.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-player-search-control.has-selection.contract-text-input.is-open > div > div.inline-list > ul > button > span.btn-text")).Text;
                _ = int.TryParse(item.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-player-search-control.has-selection.contract-text-input.is-open > div > div.inline-list > ul > button > span.btn-subtext")).Text, out var baseRating);

                if (baseRating == buyCardDTO?.Card?.Rating)
                {
                    item.Click();
                    break;
                }
            }
        }
    }
}
