namespace Magnus.Futbot.Common.Events
{
    public class XUTSIDUpdatedEventArgs : EventArgs
    {
        public XUTSIDUpdatedEventArgs(string username, string xutsid)
        {
            Username = username;
            XUTSID = xutsid;
        }

        public string Username { get; }

        public string XUTSID { get; }
    }
}
