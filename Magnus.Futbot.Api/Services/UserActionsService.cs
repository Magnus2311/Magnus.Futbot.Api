﻿using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using System.Collections.Concurrent;

namespace Magnus.Futbot
{
    public class UserActionsService
    {
        private readonly IActionsService _actionsService;

        public UserActionsService(IActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        private static ConcurrentDictionary<string, ActionsQueue> UserActions { get; } = new();

        public ActionsQueue GetActionsQueueByUsername(string username)
        {
            if (!UserActions.ContainsKey(username))
                UserActions.TryAdd(username, new ActionsQueue(_actionsService));

            return UserActions[username];
        }

        public void AddAction(string username, BuyAction tradeAction)
        {
            if (!UserActions.ContainsKey(username))
                UserActions.TryAdd(username, new ActionsQueue(_actionsService));

            UserActions[username].AddAction(tradeAction);
        }

        public void AddAction(string username, SellAction tradeAction)
        {
            if (!UserActions.ContainsKey(username))
                UserActions.TryAdd(username, new ActionsQueue(_actionsService));

            UserActions[username].AddAction(tradeAction);
        }

        public void AddAction(string username, MoveAction tradeAction)
        {
            if (!UserActions.ContainsKey(username))
                UserActions.TryAdd(username, new ActionsQueue(_actionsService));

            UserActions[username].AddAction(tradeAction);
        }
    }
}
