using GlobalIntelligenceMonitor.Models;
using RestSharp;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.Reactive.Subjects;
using Serilog;

namespace GlobalIntelligenceMonitor.Services;

/// <summary>
/// Haber Agregator Servisi - Çoklu kaynaklardan haberler toplar
/// </summary>
public interface INewsAggregatorService
{
    Task<List<NewsItem>> GetLatestNewsAsync(int count = 50);
    Task<List<NewsItem>> GetNewsByCategoryAsync(NewsCategory category, int count = 20);
    Task<List<NewsItem>> GetNewsByCountryAsync(string countryCode, int count = 20);
    Task<List<NewsItem>> GetBreakingNewsAsync();
    Task<List<NewsItem>> GetNewsNearLocationAsync(double latitude, double longitude, double radiusKm);
    Task<NewsItem?> GetNewsDetailsAsync(string newsId);
    IObservable<NewsItem> SubscribeToBreakingNews();
    Task<List<string>> GetNewsSourcesToLocalCountryAsync();
}

public class NewsAggregatorService : INewsAggregatorService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabaseService _databaseService;
    private readonly IGeoLocationService _geoLocationService;
    private readonly Subject<NewsItem> _breakingNewsSubject = new();
    private readonly ILogger _logger = Log.ForContext<NewsAggregatorService>();

    // API Keys (environment variables'dan alınmalı)
    private const string NEWS_API_KEY = "YOUR_NEWS_API_KEY";
    private const string GUARDIAN_API_KEY = "YOUR_GUARDIAN_API_KEY";
    private const string NEWSAPI_ENDPOINT = "https://newsapi.org/v2";
    private const string GUARDIAN_ENDPOINT = "https://open-platform.theguardian.com/search";

    // Yerel haber kaynakları
    private readonly Dictionary<string, List<string>> _localNewsSources = new()
    {
        { "TR", new List<string> { "https://www.haberturk.com", "https://www.ntv.com.tr", "https://www.bbc.com/turkce" } },
        { "US", new List<string> { "https://www.nytimes.com", "https://www.washingtonpost.com", "https://www.bbc.com/news" } },
        { "GB", new List<string> { "https://www.bbc.com/news", "https://www.theguardian.com", "https://www.reuters.com" } },
        { "DE", new List<string> { "https://www.dw.com", "https://www.spiegel.de", "https://www.bild.de" } },
        { "FR", new List<string> { "https://www.france24.com", "https://www.bfmtv.com", "https://www.lemonde.fr" } }
    };

    public NewsAggregatorService(
        IHttpClientFactory httpClientFactory,
        IDatabaseService databaseService,
        IGeoLocationService geoLocationService)
    {
        _httpClientFactory = httpClientFactory;
        _databaseService = databaseService;
        _geoLocationService = geoLocationService;

        _ = StartBreakingNewsStreamAsync();
    }

    public async Task<List<NewsItem>> GetLatestNewsAsync(int count = 50)
    {
        try
        {
            var news = new List<NewsItem>();

            // NewsAPI.org'dan haberler
            var newsApiNews = await GetNewsFromNewsApiAsync(count);
            news.AddRange(newsApiNews);

            // The Guardian'dan haberler
            var guardianNews = await GetNewsFromGuardianAsync(count);
            news.AddRange(guardianNews);

            // BBC World News scraping
            var bbcNews = await GetNewsFromBBCAsync(count);
            news.AddRange(bbcNews);

            // Reuters scraping
            var reutersNews = await GetNewsFromReutersAsync(count);
            news.AddRange(reutersNews);

            // Duplikaları temizle ve sırala
            news = news
                .DistinctBy(n => n.Url)
                .OrderByDescending(n => n.PublishedAt)
                .Take(count)
                .ToList();

            // Veritabanına kaydet
            foreach (var item in news)
            {
                await _databaseService.SaveNewsItemAsync(item);
            }

            return news;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting latest news");
            return new List<NewsItem>();
        }
    }

    public async Task<List<NewsItem>> GetNewsByCategoryAsync(NewsCategory category, int count = 20)
    {
        try
        {
            var categoryKeywords = GetKeywordsForCategory(category);
            var client = _httpClientFactory.CreateClient();
            var news = new List<NewsItem>();

            foreach (var keyword in categoryKeywords)
            {
                var url = $"{NEWSAPI_ENDPOINT}/everything?q={keyword}&sortBy=publishedAt&language=en&apiKey={NEWS_API_KEY}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jObject = JObject.Parse(content);
                    var articles = jObject["articles"] as JArray;

                    if (articles != null)
                    {
                        foreach (var article in articles.Take(count))
                        {
                            var newsItem = new NewsItem
                            {
                                Title = article["title"]?.ToString() ?? "",
                                Description = article["description"]?.ToString() ?? "",
                                Content = article["content"]?.ToString() ?? "",
                                Url = article["url"]?.ToString() ?? "",
                                ImageUrl = article["urlToImage"]?.ToString() ?? "",
                                Source = article["source"]?["name"]?.ToString() ?? "Unknown",
                                PublishedAt = DateTime.Parse(article["publishedAt"]?.ToString() ?? DateTime.UtcNow.ToString()),
                                Category = category,
                                Language = "en"
                            };

                            news.Add(newsItem);
                        }
                    }
                }
            }

            return news
                .DistinctBy(n => n.Url)
                .OrderByDescending(n => n.PublishedAt)
                .Take(count)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news by category");
            return new List<NewsItem>();
        }
    }

    public async Task<List<NewsItem>> GetNewsByCountryAsync(string countryCode, int count = 20)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{NEWSAPI_ENDPOINT}/top-headlines?country={countryCode.ToLower()}&apiKey={NEWS_API_KEY}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<NewsItem>();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);
            var articles = jObject["articles"] as JArray;

            var news = new List<NewsItem>();

            if (articles != null)
            {
                foreach (var article in articles.Take(count))
                {
                    var newsItem = new NewsItem
                    {
                        Title = article["title"]?.ToString() ?? "",
                        Description = article["description"]?.ToString() ?? "",
                        Content = article["content"]?.ToString() ?? "",
                        Url = article["url"]?.ToString() ?? "",
                        ImageUrl = article["urlToImage"]?.ToString() ?? "",
                        Source = article["source"]?["name"]?.ToString() ?? "Unknown",
                        PublishedAt = DateTime.Parse(article["publishedAt"]?.ToString() ?? DateTime.UtcNow.ToString()),
                        Country = countryCode,
                        Language = "en",
                        Priority = PriorityLevel.Normal
                    };

                    news.Add(newsItem);
                }
            }

            return news;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news by country");
            return new List<NewsItem>();
        }
    }

    public async Task<List<NewsItem>> GetBreakingNewsAsync()
    {
        try
        {
            var news = await GetLatestNewsAsync(50);
            return news
                .Where(n => n.IsBreaking || n.Priority >= PriorityLevel.High)
                .OrderByDescending(n => n.PublishedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting breaking news");
            return new List<NewsItem>();
        }
    }

    public async Task<List<NewsItem>> GetNewsNearLocationAsync(double latitude, double longitude, double radiusKm)
    {
        try
        {
            var news = await GetLatestNewsAsync(100);

            return news
                .Where(n => n.Location != null && 
                    CalculateDistance(latitude, longitude, n.Location.Latitude, n.Location.Longitude) <= radiusKm)
                .OrderByDescending(n => n.PublishedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news near location");
            return new List<NewsItem>();
        }
    }

    public async Task<NewsItem?> GetNewsDetailsAsync(string newsId)
    {
        try
        {
            return await _databaseService.GetNewsDetailsByIdAsync(newsId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news details");
            return null;
        }
    }

    public IObservable<NewsItem> SubscribeToBreakingNews()
    {
        return _breakingNewsSubject.AsObservable();
    }

    public async Task<List<string>> GetNewsSourcesToLocalCountryAsync()
    {
        try
        {
            var userCountry = await _geoLocationService.GetUserCountryCodeAsync();
            
            if (_localNewsSources.TryGetValue(userCountry, out var sources))
            {
                return sources;
            }

            // Varsayılan kaynaklar
            return new List<string> { "https://www.bbc.com/news", "https://www.reuters.com" };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting local news sources");
            return new List<string>();
        }
    }

    // ==================== Özel Kaynak Metodları ====================

    private async Task<List<NewsItem>> GetNewsFromNewsApiAsync(int count)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{NEWSAPI_ENDPOINT}/everything?sortBy=publishedAt&language=en&pageSize={count}&apiKey={NEWS_API_KEY}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<NewsItem>();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);
            var articles = jObject["articles"] as JArray;

            var news = new List<NewsItem>();

            if (articles != null)
            {
                foreach (var article in articles)
                {
                    var newsItem = new NewsItem
                    {
                        Title = article["title"]?.ToString() ?? "",
                        Description = article["description"]?.ToString() ?? "",
                        Content = article["content"]?.ToString() ?? "",
                        Url = article["url"]?.ToString() ?? "",
                        ImageUrl = article["urlToImage"]?.ToString() ?? "",
                        Source = article["source"]?["name"]?.ToString() ?? "NewsAPI",
                        PublishedAt = DateTime.Parse(article["publishedAt"]?.ToString() ?? DateTime.UtcNow.ToString()),
                        Language = "en",
                        Priority = PriorityLevel.Normal
                    };

                    news.Add(newsItem);
                }
            }

            return news;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news from NewsAPI");
            return new List<NewsItem>();
        }
    }

    private async Task<List<NewsItem>> GetNewsFromGuardianAsync(int count)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{GUARDIAN_ENDPOINT}?page-size={count}&api-key={GUARDIAN_API_KEY}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<NewsItem>();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);
            var results = jObject["response"]?["results"] as JArray;

            var news = new List<NewsItem>();

            if (results != null)
            {
                foreach (var result in results)
                {
                    var newsItem = new NewsItem
                    {
                        Title = result["webTitle"]?.ToString() ?? "",
                        Url = result["webUrl"]?.ToString() ?? "",
                        Source = "The Guardian",
                        PublishedAt = DateTime.Parse(result["webPublicationDate"]?.ToString() ?? DateTime.UtcNow.ToString()),
                        Language = "en",
                        Priority = PriorityLevel.Normal
                    };

                    news.Add(newsItem);
                }
            }

            return news;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news from Guardian");
            return new List<NewsItem>();
        }
    }

    private async Task<List<NewsItem>> GetNewsFromBBCAsync(int count)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://www.bbc.com/news");

            if (!response.IsSuccessStatusCode)
                return new List<NewsItem>();

            var content = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var news = new List<NewsItem>();
            var newsNodes = doc.DocumentNode.SelectNodes("//a[@data-testid='internal-link']").Take(count);

            foreach (var node in newsNodes ?? Enumerable.Empty<HtmlNode>())
            {
                var href = node.GetAttributeValue("href", "");
                var title = node.InnerText.Trim();

                if (!string.IsNullOrEmpty(href) && !string.IsNullOrEmpty(title))
                {
                    var newsItem = new NewsItem
                    {
                        Title = title,
                        Url = "https://www.bbc.com" + href,
                        Source = "BBC News",
                        PublishedAt = DateTime.UtcNow,
                        Language = "en",
                        Priority = PriorityLevel.Normal
                    };

                    news.Add(newsItem);
                }
            }

            return news;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news from BBC");
            return new List<NewsItem>();
        }
    }

    private async Task<List<NewsItem>> GetNewsFromReutersAsync(int count)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://www.reuters.com");

            if (!response.IsSuccessStatusCode)
                return new List<NewsItem>();

            var content = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var news = new List<NewsItem>();
            var newsNodes = doc.DocumentNode.SelectNodes("//h3[@data-testid='Link']").Take(count);

            foreach (var node in newsNodes ?? Enumerable.Empty<HtmlNode>())
            {
                var link = node.SelectSingleNode(".//a");
                if (link != null)
                {
                    var href = link.GetAttributeValue("href", "");
                    var title = link.InnerText.Trim();

                    if (!string.IsNullOrEmpty(href) && !string.IsNullOrEmpty(title))
                    {
                        var newsItem = new NewsItem
                        {
                            Title = title,
                            Url = href.StartsWith("http") ? href : "https://www.reuters.com" + href,
                            Source = "Reuters",
                            PublishedAt = DateTime.UtcNow,
                            Language = "en",
                            Priority = PriorityLevel.Normal
                        };

                        news.Add(newsItem);
                    }
                }
            }

            return news;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting news from Reuters");
            return new List<NewsItem>();
        }
    }

    // ==================== Canlı Akış ====================

    private async Task StartBreakingNewsStreamAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var breakingNews = await GetBreakingNewsAsync();

                foreach (var news in breakingNews.Take(10))
                {
                    _breakingNewsSubject.OnNext(news);
                }

                await Task.Delay(60000, cancellationTokenSource.Token); // Her dakika kontrol et
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in breaking news stream");
                await Task.Delay(5000);
            }
        }
    }

    // ==================== Yardımcı Metotlar ====================

    private List<string> GetKeywordsForCategory(NewsCategory category)
    {
        return category switch
        {
            NewsCategory.Earthquake => new List<string> { "earthquake", "seismic", "tremor" },
            NewsCategory.Conflict => new List<string> { "conflict", "war", "military" },
            NewsCategory.Cyber => new List<string> { "cyber attack", "hacking", "cybersecurity" },
            NewsCategory.Nuclear => new List<string> { "nuclear", "atomic" },
            NewsCategory.Weather => new List<string> { "weather", "storm", "hurricane" },
            NewsCategory.Disaster => new List<string> { "disaster", "emergency", "emergency" },
            _ => new List<string> { "news" }
        };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}

// ==================== Diğer Servisler (Stub) ====================

public interface IGeoLocationService
{
    Task<string> GetUserCountryCodeAsync();
    Task<GeoPoint> GetUserLocationAsync();
}

public interface INewsScraperService
{
    Task<List<NewsItem>> ScrapeNewsSourceAsync(string url);
}

public interface IOsintService
{
    Task<List<string>> SearchAcrossSourcesAsync(string query);
    Task<Dictionary<string, object>> GetGeoIntelligenceAsync(double latitude, double longitude);
}
