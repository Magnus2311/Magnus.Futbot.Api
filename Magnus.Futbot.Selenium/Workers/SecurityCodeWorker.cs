using Magnus.Futbot.Common.Models.Results;
using Magnus.Futbot.Selenium.Consumers;
using Magnus.Futbot.Selenium.Procucers;
using Magnus.Futbot.Services;

namespace Magnus.Futbot.Selenium.Workers
{
    public class SecurityCodeWorker : BackgroundService
    {
        private readonly SecurityCodeProducer _securityCodeProducer;
        private readonly SecurityCodeConsumer _securityCodeConsumer;

        public SecurityCodeWorker(SecurityCodeProducer securityCodeProducer,
            SecurityCodeConsumer securityCodeConsumer)
        {
            _securityCodeProducer = securityCodeProducer;
            _securityCodeConsumer = securityCodeConsumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var profileDTO = _securityCodeConsumer.Consumer.Consume(stoppingToken);
                    var response = LoginSeleniumService.SubmitCode(profileDTO.Message.Value);
                    _securityCodeProducer.Produce(new SubmitCodeResult(profileDTO.Message.Value, response));
                }

                _securityCodeConsumer.Consumer.Dispose();
                _securityCodeConsumer.Consumer.Close();
            }, stoppingToken);
    }
}
