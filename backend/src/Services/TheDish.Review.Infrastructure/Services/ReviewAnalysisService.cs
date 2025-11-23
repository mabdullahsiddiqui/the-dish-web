using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using TheDish.Review.Application.DTOs;
using TheDish.Review.Application.Interfaces;

namespace TheDish.Review.Infrastructure.Services;

public class ReviewAnalysisService : IReviewAnalysisService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ReviewAnalysisService> _logger;

    public ReviewAnalysisService(IConfiguration configuration, ILogger<ReviewAnalysisService> logger)
    {
        _logger = logger;
        
        var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"] ?? "http://localhost:9200"))
            .DefaultIndex("places");

        _elasticClient = new ElasticClient(settings);
    }

    public async Task<ReviewAnalysisDto?> GetReviewAnalysisAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchResponse = await _elasticClient.SearchAsync<PlaceDocument>(s => s
                .Index("places")
                .Query(q => q
                    .Nested(n => n
                        .Path(p => p.ReviewSentimentScores)
                        .Query(nq => nq
                            .Term(t => t
                                .Field(f => f.ReviewSentimentScores.First().ReviewId)
                                .Value(reviewId.ToString())
                            )
                        )
                    )
                )
                .Size(1), cancellationToken);

            if (!searchResponse.IsValid || !searchResponse.Documents.Any())
            {
                return null;
            }

            var placeDoc = searchResponse.Documents.First();
            var reviewSentiment = placeDoc.ReviewSentimentScores?
                .FirstOrDefault(r => r.ReviewId == reviewId);

            if (reviewSentiment == null)
            {
                return null;
            }

            return new ReviewAnalysisDto
            {
                Sentiment = reviewSentiment.Sentiment,
                SentimentConfidence = reviewSentiment.Confidence,
                Tags = reviewSentiment.Tags.Select(t => new ReviewTagDto
                {
                    Tag = t,
                    Confidence = reviewSentiment.Confidence
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving review analysis for ReviewId: {ReviewId}", reviewId);
            return null;
        }
    }
}

// Reuse the same document models
public class PlaceDocument
{
    public Guid Id { get; set; }
    public List<string>? AiTags { get; set; }
    public List<ReviewSentimentData>? ReviewSentimentScores { get; set; }
    public List<AggregatedTag>? AggregatedTags { get; set; }
}

public class ReviewSentimentData
{
    public Guid ReviewId { get; set; }
    public List<string> Tags { get; set; } = new();
    public double Confidence { get; set; }
    public string Sentiment { get; set; } = string.Empty;
}

public class AggregatedTag
{
    public string Tag { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public double AvgConfidence { get; set; }
}




