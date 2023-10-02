using Magnus.Futbot.Common.Events;

namespace Magnus.Futtbot.Connections.Utils
{
    public class EaData
    {
        private static readonly Lazy<EaData> _instance = new(() => new EaData());

        public static Dictionary<string, string> UserXUTSIDs { get; } = new Dictionary<string, string>();

        public static EaData Instance => _instance.Value;

        static EaData()
        {
            UserXUTSIDs = new();
            EventAggregator.Instance.XUTSIDUpdated += OnHeaderReceived;
        }

        private static void OnHeaderReceived(object sender, XUTSIDUpdatedEventArgs e)
        {
            if (!UserXUTSIDs.ContainsKey(e.Username))
                UserXUTSIDs.Add(e.Username, e.XUTSID);

            UserXUTSIDs[e.Username] = e.XUTSID;
        }
    }
}
