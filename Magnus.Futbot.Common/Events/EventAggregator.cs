namespace Magnus.Futbot.Common.Events
{
    public class EventAggregator
    {
        private static readonly Lazy<EventAggregator> _instance = new(() => new EventAggregator());

        public static EventAggregator Instance => _instance.Value;

        public event EventHandler<XUTSIDUpdatedEventArgs> XUTSIDUpdated;

        public void OnXUTSIDUpdated(object sender, XUTSIDUpdatedEventArgs e)
        {
            XUTSIDUpdated?.Invoke(sender, e);
        }
    }
}
