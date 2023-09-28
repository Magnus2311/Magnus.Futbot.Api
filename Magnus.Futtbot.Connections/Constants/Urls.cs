namespace Magnus.Futtbot.Connections.Constants
{
    public static class Urls
    {
        public static string Main => "https://www.ea.com/ea-sports-fc/ultimate-team/web-app/";

        public static string Nucleus => "https://accounts.ea.com/connect/auth?accessToken=&client_id=FC24_JS_WEB_APP&display=web2/login&hide_create=true&locale=en_US&prompt=login&redirect_uri=https://www.ea.com/ea-sports-fc/ultimate-team/web-app/auth.html&release_type=prod&response_type=token&scope=basic.identity+offline+signin+basic.entitlement+basic.persona";

        public static string Personas => "https://www.easports.com/fifa/api/personas";

        public static string Shards => "https://www.easports.com/iframe/fut17/p/ut/shards/v2";

        public static string Accounts => "https://www.easports.com/iframe/fut17/p/ut/game/fifa17/user/accountinfo?filterConsoleLogin=true&sku=FUT17WEB&returningUserGameYear=2016&_=";

        public static string Session => "https://www.easports.com/iframe/fut17/p/ut/auth";

        public static string Question => "https://www.easports.com/iframe/fut17/p/ut/game/fifa17/phishing/question?_=";

        public static string Validate => "https://www.easports.com/iframe/fut17/p/ut/game/fifa17/phishing/validate?_=";

        public static string Referer => "https://www.easports.com/iframe/fut17/?locale=en_US&baseShowoffUrl=https%3A%2F%2Fwww.easports.com%2Fde%2Ffifa%2Fultimate-team%2Fweb-app%2Fshow-off&guest_app_uri=http%3A%2F%2Fwww.easports.com%2Fde%2Ffifa%2Fultimate-team%2Fweb-app";
    }
}
