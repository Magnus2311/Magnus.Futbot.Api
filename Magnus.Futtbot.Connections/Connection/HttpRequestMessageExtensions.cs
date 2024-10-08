﻿using Magnus.Futtbot.Connections.Utils;

public static class HttpRequestMessageExtensions
{
    public static void SetCommonHeaders(this HttpRequestMessage request, string username)
    {
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Host", "utas.mob.v4.prd.futc-ext.gcp.ea.com");
        request.Headers.Add("Origin", "https://www.ea.com");
        request.Headers.Add("Referer", "https://www.ea.com/");
        request.Headers.Add("Sec-Fetch-Dest", "empty");
        request.Headers.Add("Sec-Fetch-Mode", "cors");
        request.Headers.Add("Sec-Fetch-Site", "same-site");
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
        request.Headers.Add("X-UT-SID", EaData.UserXUTSIDs[username]);
        request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
    }
}