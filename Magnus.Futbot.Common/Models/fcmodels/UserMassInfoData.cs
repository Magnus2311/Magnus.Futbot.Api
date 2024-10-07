namespace Magnus.Futbot.Common.fcmodels
{
    public class UserMassInfoData
    {
        public Errors errors { get; set; }
        public Settings settings { get; set; }
        public Userinfo userInfo { get; set; }
        public Purchaseditems purchasedItems { get; set; }
        public Loanplayerclientdata loanPlayerClientData { get; set; }
        public Squad squad { get; set; }
        public Clubuser clubUser { get; set; }
        public Activemessages activeMessages { get; set; }
        public Onboardingclientdata onboardingClientData { get; set; }
        public bool? isPlayerPicksTemporaryStorageNotEmpty { get; set; }
    }

    public class Errors
    {
    }

    public class Settings
    {
        public Config[] configs { get; set; }
    }

    public class Config
    {
        public long value { get; set; }
        public string type { get; set; }
    }

    public class Userinfo
    {
        public long personaId { get; set; }
        public string clubName { get; set; }
        public string clubAbbr { get; set; }
        public int draw { get; set; }
        public int loss { get; set; }
        public int credits { get; set; }
        public Bidtokens bidTokens { get; set; }
        public Currency[] currencies { get; set; }
        public int trophies { get; set; }
        public int won { get; set; }
        public Active[] actives { get; set; }
        public string established { get; set; }
        public int divisionOffline { get; set; }
        public int divisionOnline { get; set; }
        public string personaName { get; set; }
        public Squadlist squadList { get; set; }
        public Unopenedpacks unopenedPacks { get; set; }
        public bool? purchased { get; set; }
        public Reliability reliability { get; set; }
        public bool? seasonTicket { get; set; }
        public string accountCreatedPlatformName { get; set; }
        public int unassignedPileSize { get; set; }
        public Feature feature { get; set; }
        public int sessionCoinsBankBalance { get; set; }
        public Platform platform { get; set; }
        public int fcPointsFromLastYear { get; set; }
        public int fcPointsTransferredStatus { get; set; }
    }

    public class Bidtokens
    {
    }

    public class Squadlist
    {
        public Squad[] squad { get; set; }
        public int activeSquadId { get; set; }
    }

    public class Tactic
    {
        public int squadId { get; set; }
        public string tactic { get; set; }
        public int lastUpdateTime { get; set; }
        public string formation { get; set; }
        public Position[] positions { get; set; }
        public Instruction[] instructions { get; set; }
        public Style[] styles { get; set; }
    }

    public class Position
    {
        public int index { get; set; }
        public int value { get; set; }
    }

    public class Instruction
    {
        public int index { get; set; }
        public int value { get; set; }
    }

    public class Style
    {
        public int index { get; set; }
        public int value { get; set; }
    }

    public class Unopenedpacks
    {
        public int preOrderPacks { get; set; }
        public int recoveredPacks { get; set; }
    }

    public class Reliability
    {
        public int reliability { get; set; }
        public int startedMatches { get; set; }
        public int finishedMatches { get; set; }
        public int matchUnfinishedTime { get; set; }
    }

    public class Feature
    {
        public int trade { get; set; }
        public int rivals { get; set; }
        public int mtx { get; set; }
    }

    public class Platform
    {
        public bool? hasTid { get; set; }
    }

    public class Currency
    {
        public string name { get; set; }
        public int funds { get; set; }
        public int finalFunds { get; set; }
    }

    public class Active
    {
        public long id { get; set; }
        public int timestamp { get; set; }
        public string formation { get; set; }
        public bool? untradeable { get; set; }
        public int assetId { get; set; }
        public int rating { get; set; }
        public string itemType { get; set; }
        public int resourceId { get; set; }
        public int owners { get; set; }
        public int discardValue { get; set; }
        public string itemState { get; set; }
        public int cardsubtypeid { get; set; }
        public int lastSalePrice { get; set; }
        public object[] statsList { get; set; }
        public object[] lifetimeStats { get; set; }
        public object[] attributeList { get; set; }
        public int teamid { get; set; }
        public int rareflag { get; set; }
        public int leagueId { get; set; }
        public int pile { get; set; }
        public int cardassetid { get; set; }
        public int value { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int resourceGameYear { get; set; }
        public int[] attributeArray { get; set; }
        public bool? authenticity { get; set; }
        public int showCasePriority { get; set; }
        public int category { get; set; }
        public int year { get; set; }
        public bool? isPlatformSpecific { get; set; }
        public string biodescription { get; set; }
        public int stadiumid { get; set; }
        public int capacity { get; set; }
        public int tifoSupportType { get; set; }
        public bool? tifoRestricted { get; set; }
        public bool? bannerRestricted { get; set; }
        public bool? ballRestricted { get; set; }
        public int preferredTime1 { get; set; }
        public int preferredTime2 { get; set; }
        public int preferredWeather { get; set; }
        public bool? undiscardable { get; set; }
        public int tier { get; set; }
        public bool? myStadium { get; set; }
        public int weightrare { get; set; }
        public string header { get; set; }
        public int chantsCount { get; set; }
        public string manufacturer { get; set; }
    }

    public class Purchaseditems
    {
        public Itemdata[] itemData { get; set; }
    }

    public class Itemdata
    {
        public long id { get; set; }
        public int timestamp { get; set; }
        public string formation { get; set; }
        public bool? untradeable { get; set; }
        public int assetId { get; set; }
        public int rating { get; set; }
        public string itemType { get; set; }
        public int resourceId { get; set; }
        public int owners { get; set; }
        public int discardValue { get; set; }
        public string itemState { get; set; }
        public int cardsubtypeid { get; set; }
        public int lastSalePrice { get; set; }
        public string injuryType { get; set; }
        public int injuryGames { get; set; }
        public string preferredPosition { get; set; }
        public int contract { get; set; }
        public int teamid { get; set; }
        public int rareflag { get; set; }
        public int playStyle { get; set; }
        public int leagueId { get; set; }
        public int assists { get; set; }
        public int lifetimeAssists { get; set; }
        public int loyaltyBonus { get; set; }
        public int pile { get; set; }
        public int nation { get; set; }
        public int resourceGameYear { get; set; }
        public int[] attributeArray { get; set; }
        public int[] statsArray { get; set; }
        public int[] lifetimeStatsArray { get; set; }
        public int skillmoves { get; set; }
        public int weakfootabilitytypecode { get; set; }
        public int preferredfoot { get; set; }
        public int marketAverage { get; set; }
        public string[] possiblePositions { get; set; }
        public int gender { get; set; }
        public int[] baseTraits { get; set; }
        public int[] plusRoles { get; set; }
    }

    public class Loanplayerclientdata
    {
        public Entry[] entries { get; set; }
    }

    public class Entry
    {
        public int value { get; set; }
        public int key { get; set; }
    }

    public class Squad
    {
        public int id { get; set; }
        public bool? valid { get; set; }
        public long personaId { get; set; }
        public string formation { get; set; }
        public object rating { get; set; }
        public int chemistry { get; set; }
        public Manager[] manager { get; set; }
        public Player[] players { get; set; }
        public object dreamSquad { get; set; }
        public int changed { get; set; }
        public string squadName { get; set; }
        public int starRating { get; set; }
        public long captain { get; set; }
        public Kicktaker[] kicktakers { get; set; }
        public Active[] actives { get; set; }
        public object newSquad { get; set; }
        public string squadType { get; set; }
        public object custom { get; set; }
        public Tactic[] tactics { get; set; }
    }

    public class Manager
    {
        public long id { get; set; }
        public int timestamp { get; set; }
        public string formation { get; set; }
        public bool? untradeable { get; set; }
        public int assetId { get; set; }
        public int rating { get; set; }
        public string itemType { get; set; }
        public int resourceId { get; set; }
        public int owners { get; set; }
        public int discardValue { get; set; }
        public string itemState { get; set; }
        public int cardsubtypeid { get; set; }
        public int lastSalePrice { get; set; }
        public object[] statsList { get; set; }
        public object[] lifetimeStats { get; set; }
        public int contract { get; set; }
        public object[] attributeList { get; set; }
        public int teamid { get; set; }
        public int rareflag { get; set; }
        public int leagueId { get; set; }
        public int pile { get; set; }
        public int nation { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int negotiation { get; set; }
        public int resourceGameYear { get; set; }
        public int gender { get; set; }
        public int itemPronoun { get; set; }
    }

    public class Player
    {
        public int index { get; set; }
        public Itemdata itemData { get; set; }
        public int loyaltyBonus { get; set; }
        public int kitNumber { get; set; }
        public int chemistry { get; set; }
        public int rank { get; set; }
    }

    public class Academyattribute
    {
        public int id { get; set; }
        public int totalBonus { get; set; }
    }

    public class Kicktaker
    {
        public long id { get; set; }
        public int index { get; set; }
    }

    public class Clubuser
    {
    }

    public class Activemessages
    {
        public object[] activeMessage { get; set; }
    }

    public class Onboardingclientdata
    {
        public Entry[] entries { get; set; }
    }
}
