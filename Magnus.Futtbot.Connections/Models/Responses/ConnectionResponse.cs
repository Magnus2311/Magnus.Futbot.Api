using Magnus.Futtbot.Connections.Enums;

namespace Magnus.Futtbot.Connections.Models.Responses
{
    public class ConnectionResponse<T>
    {
        public ConnectionResponse(ConnectionResponseType connectionResponseType)
        {
            ConnectionResponseType = connectionResponseType;
        }

        public ConnectionResponse(ConnectionResponseType connectionResponseType, T? data)
        {
            ConnectionResponseType = connectionResponseType;
            Data = data;
        }

        public ConnectionResponseType ConnectionResponseType { get; }

        public T? Data { get; }
    }
}
