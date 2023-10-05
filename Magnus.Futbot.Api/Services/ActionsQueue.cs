using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.Selenium.Actions;

namespace Magnus.Futbot
{
    public class ActionsQueue
    {
        private readonly IActionsService _actionsService;
        private readonly object _locker = new();

        public ActionsQueue(IActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        public PriorityQueue<TradeAction, int> PendingActions { get; set; } = new();

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
                                await _actionsService.DeactivateAction(nextAction.Id, TradeActionType.Buy);
                            }
                        }
                    }), action.CancellationTokenSource, action.BuyCardDTO);
                    tradeAction.Id = action.Id;

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
                    tradeAction.Id = action.Id;

                    PendingActions.Enqueue(tradeAction, tradeAction.Priority);
                    Task.Run(async () =>
                    {
                        await tradeAction.Action.Invoke();
                        await _actionsService.DeactivateAction(tradeAction.Id, TradeActionType.Buy);
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
                                await _actionsService.DeactivateAction(nextAction.Id, TradeActionType.Sell);
                            }
                        }
                    }), action.CancellationTokenSource, action.SellCardDTO);
                    tempAction.Id = action.Id;
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
                        await _actionsService.DeactivateAction(tempAction.Id, TradeActionType.Sell);
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
                                await _actionsService.DeactivateAction(nextAction.Id, TradeActionType.Move);
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
                        await _actionsService.DeactivateAction(tempAction.Id, TradeActionType.Move);
                    });
                    return tempAction;
                }
            }
        }
    }
}
