using System.Collections.Concurrent;
using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Models
{
    public class DriverInstance
    {
        private ConcurrentQueue<Action> _pendingActions { get; set; } = new();

        public DriverInstance(ChromeDriver driver)
        {
            Driver = driver;
        }

        public ChromeDriver Driver { get; set; }

        public void AddAction(Action action)
        {
            if (!_pendingActions.IsEmpty)
            {
                _pendingActions.Enqueue(new Action(() =>
                {
                    action.Invoke();

                    if (_pendingActions.TryDequeue(out var nextAction)) nextAction.Invoke();
                }));
            }
            else
            {
                var tempAction = new Action(() =>
                {
                    action.Invoke();

                    _pendingActions.TryDequeue(out _);
                    if (_pendingActions.TryDequeue(out var nextAction)) nextAction.Invoke();
                });
                _pendingActions.Enqueue(tempAction);
                tempAction.Invoke();
            }
        }
    }
}