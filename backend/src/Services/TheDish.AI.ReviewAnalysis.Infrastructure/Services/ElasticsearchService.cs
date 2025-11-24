using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using TheDish.AI.ReviewAnalysis.Application.DTOs;
using TheDish.AI.ReviewAnalysis.Application.Interfaces;

namespace TheDish.AI.ReviewAnalysis.Infrastructure.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticsearchService> _logger;
    private const string PlacesIndexName = "places";
    private const string ReviewsIndexName = "reviews";

    public ElasticsearchService(IConfiguration configuration, ILogger<ElasticsearchService> logger)
    {
        _logger = logger;
        
        var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Url"] ?? "http://localhost:9200"))
            .DefaultIndex(PlacesIndexName);

        _elasticClient = new ElasticClient(settings);
    }

    public async Task IndexReviewAnalysisAsync(Guid placeId, Guid reviewId, ReviewAnalysisResult analysis, CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure index exists with proper mapping
            await EnsurePlacesIndexExistsAsync(cancellationToken);

            // Get existing place document
            var placeResponse = await _elasticClient.GetAsync<PlaceDocument>(placeId.ToString(), ct: cancellationToken);
            
            if (!placeResponse.IsValid && placeResponse.ServerError?.Status != 404)
            {
                _logger.LogWarning("Place {PlaceId} not found in Elasticsearch, creating new document", placeId);
                // Create new place document if it doesn't exist
                await CreatePlaceDocumentAsync(placeId, reviewId, analysis, cancellationToken);
                return;
            }

            var placeDoc = placeResponse.Source ?? new PlaceDocument { Id = placeId };

            // Add review sentiment data
            var reviewSentiment = new ReviewSentimentData
            {
                ReviewId = reviewId,
                Tags = analysis.Tags.Select(t => t.Tag).ToList(),
                Confidence = analysis.Tags.Average(t => t.Confidence),
                Sentiment = analysis.Sentiment
            };

            if (placeDoc.ReviewSentimentScores == null)
                placeDoc.ReviewSentimentScores = new List<ReviewSentimentData>();
            
            // Remove existing entry for this review if present
            placeDoc.ReviewSentimentScores.RemoveAll(r => r.ReviewId == reviewId);
            placeDoc.ReviewSentimentScores.Add(reviewSentiment);

            // Update aggregated tags
            UpdateAggregatedTags(placeDoc, analysis);

            // Update AI tags list (unique tags from all reviews)
            var allTags = placeDoc.ReviewSentimentScores
                .SelectMany(r => r.Tags)
                .Distinct()
                .ToList();
            placeDoc.AiTags = allTags;

            // Index the updated document
            var indexResponse = await _elasticClient.IndexAsync(placeDoc, idx => idx
                .Index(PlacesIndexName)
                .Id(placeId.ToString()), cancellationToken);

            if (!indexResponse.IsValid)
            {
                _logger.LogError("Failed to index place document: {Error}", indexResponse.ServerError?.Error);
                throw new Exception($"Failed to index place document: {indexResponse.ServerError?.Error}");
            }

            _logger.LogInformation("Indexed review analysis for PlaceId: {PlaceId}, ReviewId: {ReviewId}", placeId, reviewId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing review analysis for PlaceId: {PlaceId}", placeId);
            throw;
        }
    }

    public async Task<ReviewAnalysisResult?> GetReviewAnalysisAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchResponse = await _elasticClient.SearchAsync<PlaceDocument>(s => s
                .Index(PlacesIndexName)
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

            return new ReviewAnalysisResult
            {
                Sentiment = reviewSentiment.Sentiment,
                SentimentConfidence = reviewSentiment.Confidence,
                Tags = reviewSentiment.Tags.Select(t => new TagResult
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

    private async Task EnsurePlacesIndexExistsAsync(CancellationToken cancellationToken)
    {
        var indexExists = await _elasticClient.Indices.ExistsAsync(PlacesIndexName, ct: cancellationToken);
        
        if (!indexExists.Exists)
        {
            var createIndexResponse = await _elasticClient.Indices.CreateAsync(PlacesIndexName, c => c
                .Map<PlaceDocument>(m => m
                    .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Keyword(k => k.Name(n => n.AiTags))
                        .Nested<ReviewSentimentData>(n => n
                            .Name(nn => nn.ReviewSentimentScores)
                            .Properties(np => np
                                .Keyword(k => k.Name(nn => nn.ReviewId))
                                .Keyword(k => k.Name(nn => nn.Tags))
                                .Double(d => d.Name(nn => nn.Confidence))
                                .Keyword(k => k.Name(nn => nn.Sentiment))
                            )
                        )
                        .Nested<AggregatedTag>(n => n
                            .Name(nn => nn.AggregatedTags)
                            .Properties(np => np
                                .Keyword(k => k.Name(nn => nn.Tag))
                                .Number(nn => nn.Name(n => n.Frequency))
                                .Double(d => d.Name(nn => nn.AvgConfidence))
                            )
                        )
                    )
                ), cancellationToken);

            if (!createIndexResponse.IsValid)
            {
                _logger.LogWarning("Index may already exist or creation failed: {Error}", createIndexResponse.ServerError?.Error);
            }
        }
    }

    private async Task CreatePlaceDocumentAsync(Guid placeId, Guid reviewId, ReviewAnalysisResult analysis, CancellationToken cancellationToken)
    {
        var placeDoc = new PlaceDocument
        {
            Id = placeId,
            AiTags = analysis.Tags.Select(t => t.Tag).ToList(),
            ReviewSentimentScores = new List<ReviewSentimentData>
            {
                new ReviewSentimentData
                {
                    ReviewId = reviewId,
                    Tags = analysis.Tags.Select(t => t.Tag).ToList(),
                    Confidence = analysis.Tags.Average(t => t.Confidence),
                    Sentiment = analysis.Sentiment
                }
            },
            AggregatedTags = analysis.Tags.Select(t => new AggregatedTag
            {
                Tag = t.Tag,
                Frequency = 1,
                AvgConfidence = t.Confidence
            }).ToList()
        };

        await _elasticClient.IndexAsync(placeDoc, idx => idx
            .Index(PlacesIndexName)
            .Id(placeId.ToString()), cancellationToken);
    }

    private void UpdateAggregatedTags(PlaceDocument placeDoc, ReviewAnalysisResult analysis)
    {
        if (placeDoc.AggregatedTags == null)
            placeDoc.AggregatedTags = new List<AggregatedTag>();

        foreach (var tag in analysis.Tags)
        {
            var existingTag = placeDoc.AggregatedTags.FirstOrDefault(t => t.Tag == tag.Tag);
            if (existingTag != null)
            {
                existingTag.Frequency++;
                existingTag.AvgConfidence = (existingTag.AvgConfidence * (existingTag.Frequency - 1) + tag.Confidence) / existingTag.Frequency;
            }
            else
            {
                placeDoc.AggregatedTags.Add(new AggregatedTag
                {
                    Tag = tag.Tag,
                    Frequency = 1,
                    AvgConfidence = tag.Confidence
                });
            }
        }
    }
}

// Document models for Elasticsearch
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






