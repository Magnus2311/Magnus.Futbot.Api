using System.Collections.Concurrent;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Models
{
    public class DriverInstance
    {
        public DriverInstance(ChromeDriver driver)
        {
            Driver = driver;
        }

        public ConcurrentQueue<TradeAction> PendingActions { get; set; } = new();

        public ChromeDriver Driver { get; set; }

        public void AddAction(BuyAction action)
        {
            if (!PendingActions.IsEmpty)
            {
                PendingActions.Enqueue(new BuyAction(new Func<Task>(async () =>
                {
                    try
                    {
                        await action.Action.Invoke();
                    }
                    catch
                    {

                    }

                    if (PendingActions.TryDequeue(out var nextAction))
                    {
                        if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            await nextAction.Action.Invoke();
                        }
                    }
                }), action.CancellationTokenSource, action.BuyCardDTO));
            }
            else
            {
                var tempAction = new BuyAction(new Func<Task>(async () =>
                {
                    try
                    {
                        await action.Action.Invoke();
                    }
                    catch
                    {

                    }
                    PendingActions.TryDequeue(out _);
                    if (PendingActions.TryDequeue(out var nextAction))
                    {
                        if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            await nextAction.Action.Invoke();
                        }
                    }
                }), action.CancellationTokenSource, action.BuyCardDTO);

                PendingActions.Enqueue(tempAction);
                tempAction.Action.Invoke();
            }
        }

        public void AddAction(SellCardAction action)
        {
            if (!PendingActions.IsEmpty)
            {
                PendingActions.Enqueue(new SellCardAction(new Func<Task>(async () =>
                {
                    try
                    {
                        await action.Action.Invoke();
                    }
                    catch
                    {

                    }

                    if (PendingActions.TryDequeue(out var nextAction))
                    {
                        if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            await nextAction.Action.Invoke();
                        }
                    }
                }), action.CancellationTokenSource, action.SellCardDTO));
            }
            else
            {
                var tempAction = new SellCardAction(new Func<Task>(async () =>
                {
                    try
                    {
                        await action.Action.Invoke();
                    }
                    catch
                    {

                    }
                    PendingActions.TryDequeue(out _);
                    if (PendingActions.TryDequeue(out var nextAction))
                    {
                        if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            await nextAction.Action.Invoke();
                        }
                    }
                }), action.CancellationTokenSource, action.SellCardDTO);

                PendingActions.Enqueue(tempAction);
                tempAction.Action.Invoke();
            }
        }
    }
}