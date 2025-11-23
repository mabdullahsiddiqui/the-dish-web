using Microsoft.Extensions.Logging;
using TheDish.AI.ReviewAnalysis.Application.DTOs;
using TheDish.AI.ReviewAnalysis.Application.Interfaces;
using System.Text.RegularExpressions;

namespace TheDish.AI.ReviewAnalysis.Infrastructure.Services;

public class SentimentAnalysisService : ISentimentAnalysisService
{
    private readonly ILogger<SentimentAnalysisService> _logger;
    private readonly Dictionary<string, string[]> _tagKeywords;

    public SentimentAnalysisService(ILogger<SentimentAnalysisService> logger)
    {
        _logger = logger;
        _tagKeywords = InitializeTagKeywords();
    }

    public Task<ReviewAnalysisResult> AnalyzeReviewAsync(string reviewText, CancellationToken cancellationToken = default)
    {
        try
        {
            var text = reviewText.ToLowerInvariant();
            
            // Determine sentiment (simplified - in production, use ML.NET model)
            var sentiment = DetermineSentiment(text);
            var sentimentConfidence = 0.75; // Placeholder - would come from model
            
            // Generate tags based on keywords and sentiment
            var tags = GenerateTags(text, sentiment);
            
            var result = new ReviewAnalysisResult
            {
                Sentiment = sentiment,
                SentimentConfidence = sentimentConfidence,
                Tags = tags
            };

            _logger.LogInformation("Analyzed review: Sentiment={Sentiment}, Tags={Tags}", 
                sentiment, string.Join(", ", tags.Select(t => t.Tag)));

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing review text");
            throw;
        }
    }

    private string DetermineSentiment(string text)
    {
        var positiveWords = new[] { "great", "excellent", "amazing", "wonderful", "delicious", "love", "best", "perfect", "fantastic", "awesome", "good", "nice", "enjoyed" };
        var negativeWords = new[] { "bad", "terrible", "awful", "horrible", "disappointed", "worst", "hate", "poor", "slow", "dirty", "rude", "overpriced" };

        var positiveCount = positiveWords.Count(word => text.Contains(word));
        var negativeCount = negativeWords.Count(word => text.Contains(word));

        if (positiveCount > negativeCount && positiveCount > 0)
            return "positive";
        else if (negativeCount > positiveCount && negativeCount > 0)
            return "negative";
        else
            return "neutral";
    }

    private List<TagResult> GenerateTags(string text, string sentiment)
    {
        var tags = new List<TagResult>();

        // Positive tags
        if (sentiment == "positive" || sentiment == "neutral")
        {
            if (ContainsKeywords(text, _tagKeywords["delicious"]))
                tags.Add(new TagResult { Tag = "delicious", Confidence = 0.85 });
            
            if (ContainsKeywords(text, _tagKeywords["family-friendly"]))
                tags.Add(new TagResult { Tag = "family-friendly", Confidence = 0.80 });
            
            if (ContainsKeywords(text, _tagKeywords["great-service"]))
                tags.Add(new TagResult { Tag = "great-service", Confidence = 0.82 });
            
            if (ContainsKeywords(text, _tagKeywords["cozy"]))
                tags.Add(new TagResult { Tag = "cozy", Confidence = 0.75 });
            
            if (ContainsKeywords(text, _tagKeywords["romantic"]))
                tags.Add(new TagResult { Tag = "romantic", Confidence = 0.78 });
            
            if (ContainsKeywords(text, _tagKeywords["affordable"]))
                tags.Add(new TagResult { Tag = "affordable", Confidence = 0.80 });
        }

        // Negative tags
        if (sentiment == "negative" || sentiment == "neutral")
        {
            if (ContainsKeywords(text, _tagKeywords["bad-service"]))
                tags.Add(new TagResult { Tag = "bad-service", Confidence = 0.85 });
            
            if (ContainsKeywords(text, _tagKeywords["slow-service"]))
                tags.Add(new TagResult { Tag = "slow-service", Confidence = 0.82 });
            
            if (ContainsKeywords(text, _tagKeywords["noisy"]))
                tags.Add(new TagResult { Tag = "noisy", Confidence = 0.80 });
            
            if (ContainsKeywords(text, _tagKeywords["overpriced"]))
                tags.Add(new TagResult { Tag = "overpriced", Confidence = 0.83 });
            
            if (ContainsKeywords(text, _tagKeywords["dirty"]))
                tags.Add(new TagResult { Tag = "dirty", Confidence = 0.85 });
            
            if (ContainsKeywords(text, _tagKeywords["rude-staff"]))
                tags.Add(new TagResult { Tag = "rude-staff", Confidence = 0.80 });
        }

        // Neutral/Contextual tags
        if (ContainsKeywords(text, _tagKeywords["busy"]))
            tags.Add(new TagResult { Tag = "busy", Confidence = 0.75 });
        
        if (ContainsKeywords(text, _tagKeywords["casual"]))
            tags.Add(new TagResult { Tag = "casual", Confidence = 0.70 });
        
        if (ContainsKeywords(text, _tagKeywords["outdoor-seating"]))
            tags.Add(new TagResult { Tag = "outdoor-seating", Confidence = 0.75 });
        
        if (ContainsKeywords(text, _tagKeywords["parking-available"]))
            tags.Add(new TagResult { Tag = "parking-available", Confidence = 0.75 });

        // Return top tags with confidence > 0.6
        return tags.Where(t => t.Confidence > 0.6)
                   .OrderByDescending(t => t.Confidence)
                   .Take(8)
                   .ToList();
    }

    private bool ContainsKeywords(string text, string[] keywords)
    {
        return keywords.Any(keyword => text.Contains(keyword));
    }

    private Dictionary<string, string[]> InitializeTagKeywords()
    {
        return new Dictionary<string, string[]>
        {
            ["delicious"] = new[] { "delicious", "amazing food", "tasty", "yummy", "flavorful", "mouth-watering" },
            ["family-friendly"] = new[] { "family", "kids", "children", "child-friendly", "family friendly" },
            ["great-service"] = new[] { "great service", "friendly staff", "excellent service", "attentive", "helpful staff" },
            ["cozy"] = new[] { "cozy", "intimate", "warm", "comfortable atmosphere" },
            ["romantic"] = new[] { "romantic", "date night", "intimate dinner" },
            ["affordable"] = new[] { "affordable", "cheap", "value", "good value", "reasonable price" },
            ["bad-service"] = new[] { "bad service", "poor service", "terrible service", "unhelpful" },
            ["slow-service"] = new[] { "slow service", "long wait", "slow", "waited forever" },
            ["noisy"] = new[] { "noisy", "loud", "too loud", "can't hear" },
            ["overpriced"] = new[] { "expensive", "overpriced", "pricey", "too expensive", "rip off" },
            ["dirty"] = new[] { "dirty", "unclean", "messy", "filthy" },
            ["rude-staff"] = new[] { "rude", "unfriendly", "rude staff", "impolite", "disrespectful" },
            ["busy"] = new[] { "busy", "crowded", "packed", "full" },
            ["casual"] = new[] { "casual", "informal", "laid back", "relaxed" },
            ["outdoor-seating"] = new[] { "outdoor", "patio", "terrace", "al fresco", "outside seating" },
            ["parking-available"] = new[] { "parking", "parking available", "free parking", "parking lot" }
        };
    }
}




