using Magnus.Futtbot.Connections.Models.Auth;

namespace Magnus.Futtbot.Connections.Utils
{
    public static class Utils
    {
        public static bool IsApiMessage(PhishingResponseBody? phishingResponseBody)
         => phishingResponseBody != null
                && phishingResponseBody.String != null
                && phishingResponseBody.Debug != null
                && phishingResponseBody.Reason != null
                && phishingResponseBody.Code != null;
    }
}
