namespace GlobalIntelligence.Config;

public static class ResourceConstants
{
    public static class Colors
    {
        public const string Primary = "#0a0f0a";
        public const string Secondary = "#1a1f1a";
        public const string Accent = "#00c8ff";
        public const string Warning = "#ff6b6b";
        public const string Success = "#00d084";
        public const string Info = "#3498db";
    }

    public static class Fonts
    {
        public const string RegularSize = 14;
        public const string LargeSize = 18;
        public const string SmallSize = 12;
        public const string TinySize = 10;
    }

    public static class Urls
    {
        public const string PrivacyPolicy = "https://globalintelligence.monitor/privacy";
        public const string TermsOfService = "https://globalintelligence.monitor/terms";
        public const string CommunityForum = "https://forum.globalintelligence.monitor";
        public const string Documentation = "https://docs.globalintelligence.monitor";
        public const string BugReport = "https://github.com/globalintelligence/issues";
    }

    public static class ApiEndpoints
    {
        public const string BaseUrl = "https://api.globalintelligence.monitor";
        public const string Earthquakes = "/v1/earthquakes";
        public const string News = "/v1/news";
        public const string Threats = "/v1/threats";
        public const string Flights = "/v1/flights";
        public const string Ships = "/v1/ships";
    }
}
