using Magnus.Futbot.Common.Interfaces.Notifiers;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Models
{
    public class DriverInstance
    {
        private readonly object _locker = new();

        public DriverInstance(ChromeDriver driver)
        {
            Driver = driver;
        }

        public PriorityQueue<TradeAction, int> PendingActions { get; set; } = new();

        public ChromeDriver Driver { get; set; }

        public TradeAction AddAction(BuyAction action)
        {
            lock (_locker)
            {
                if (PendingActions.Count > 0)
                {
                    var tradeAction = new BuyAction(new Func<Task>(async () =>
                    {
                        try
                        {
                            await action.Action.Invoke();
                        }
                        catch
                        {

                        }

                        if (PendingActions.TryDequeue(out var nextAction, out _))
                        {
                            if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                await nextAction.Action.Invoke();
                            }
                        }
                    }), action.CancellationTokenSource, action.BuyCardDTO);

                    PendingActions.Enqueue(tradeAction, action.Priority);
                    return tradeAction;
                }
                else
                {
                    var tradeAction = new BuyAction(new Func<Task>(async () =>
                    {
                        try
                        {
                            await action.Action.Invoke();
                        }
                        catch
                        {

                        }
                        PendingActions.TryDequeue(out _, out _);
                        if (PendingActions.TryDequeue(out var nextAction, out _))
                        {
                            if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                await nextAction.Action.Invoke();
                            }
                        }
                    }), action.CancellationTokenSource, action.BuyCardDTO);

                    PendingActions.Enqueue(tradeAction, tradeAction.Priority);
                    tradeAction.Action.Invoke();
                    return tradeAction;
                }
            }
        }

        public TradeAction AddAction(SellCardAction action)
        {
            lock (_locker)
            {
                if (PendingActions.Count > 0)
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

                        if (PendingActions.TryDequeue(out var nextAction, out _))
                        {
                            if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                await nextAction.Action.Invoke();
                            }
                        }
                    }), action.CancellationTokenSource, action.SellCardDTO);
                    PendingActions.Enqueue(tempAction, action.Priority);
                    return tempAction;
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
                        PendingActions.TryDequeue(out _, out _);
                        if (PendingActions.TryDequeue(out var nextAction, out _))
                        {
                            if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                await nextAction.Action.Invoke();
                            }
                        }
                    }), action.CancellationTokenSource, action.SellCardDTO);

                    PendingActions.Enqueue(tempAction, tempAction.Priority);
                    tempAction.Action.Invoke();
                    return tempAction;
                }
            }
        }
    }
}