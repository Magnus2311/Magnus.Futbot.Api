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
                    var tradeAction = new BuyAction(action.ProfileId, new Func<Task>(async () =>
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
                    var tradeAction = new BuyAction(action.ProfileId, new Func<Task>(async () =>
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
                    Task.Run(async () =>
                    {
                        await tradeAction.Action.Invoke();
                    });
                    return tradeAction;
                }
            }
        }

        public TradeAction AddAction(SellAction action)
        {
            lock (_locker)
            {
                if (PendingActions.Count > 0)
                {
                    var tempAction = new SellAction(action.ProfileId, new Func<Task>(async () =>
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
                    var tempAction = new SellAction(action.ProfileId, new Func<Task>(async () =>
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
                    Task.Run(async () =>
                    {
                        await tempAction.Action.Invoke();
                    });
                    return tempAction;
                }
            }
        }

        public TradeAction AddAction(MoveAction action)
        {
            lock (_locker)
            {
                if (PendingActions.Count > 0)
                {
                    var tempAction = new MoveAction(action.ProfileId, new Func<Task>(async () =>
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
                    }), action.CancellationTokenSource, action.Description);
                    PendingActions.Enqueue(tempAction, action.Priority);
                    return tempAction;
                }
                else
                {
                    var tempAction = new MoveAction(action.ProfileId, new Func<Task>(async () =>
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
                    }), action.CancellationTokenSource, action.Description);

                    PendingActions.Enqueue(tempAction, tempAction.Priority);
                    Task.Run(async () =>
                    {
                        await tempAction.Action.Invoke();
                    });
                    return tempAction;
                }
            }
        }
    }
}