using System.Collections.Concurrent;

namespace Magnus.Futbot.Api.Services
{
    public class UserActionsService
    {
        private readonly ActionsService _actionsService;

        public UserActionsService(ActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        public static ConcurrentDictionary<string, ActionsQueue> UserActions { get; } = new();

        public ActionsQueue GetActionsQueueByUsername(string username)
        {
            if (!UserActions.ContainsKey(username))
                UserActions.TryAdd(username, new ActionsQueue(_actionsService));

            return UserActions[username];
        }
    }
}
