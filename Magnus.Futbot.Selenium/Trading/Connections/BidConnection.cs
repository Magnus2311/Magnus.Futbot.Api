using Magnus.Futbot.Selenium.Trading.Extensions;
using Magnus.Futbot.Selenium.Trading.Models;

namespace Magnus.Futbot.Selenium.Trading.Connections
{
    public class BidConnection : PutConnection<Bid>
    {
        public BidConnection(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override string UTSID => throw new NotImplementedException();

        protected override Uri BaseAddress { get; set; } = new Uri("");

        public async Task Bid(Bid bid)
        {
            BaseAddress = new Uri($"https://utas.external.s2.fut.ea.com/ut/game/fifa22/trade/{bid.TradeId}/bid");
            var response = await PutAsync(bid);

            if (response?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseStr = response.GetResponseAsString();
                //TO DO: Get data from response str and send kafka message to update profile
            }

            if (response?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //TO DO: Send new authorization request to get new UTSID
            }

            if (response != null && (int)response!.StatusCode == 626)
            {
                //TO DO: Most likely this trade has expired
            }
        }
    }
}
