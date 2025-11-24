using MassTransit;
using Microsoft.Extensions.Logging;
using TheDish.AI.ReviewAnalysis.Application.Events;
using TheDish.AI.ReviewAnalysis.Application.Interfaces;

namespace TheDish.AI.ReviewAnalysis.Infrastructure.Consumers;

public class ReviewCreatedConsumer : IConsumer<ReviewCreatedEvent>
{
    private readonly ISentimentAnalysisService _sentimentAnalysisService;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<ReviewCreatedConsumer> _logger;

    public ReviewCreatedConsumer(
        ISentimentAnalysisService sentimentAnalysisService,
        IElasticsearchService elasticsearchService,
        ILogger<ReviewCreatedConsumer> logger)
    {
        _sentimentAnalysisService = sentimentAnalysisService;
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReviewCreatedEvent> context)
    {
        var reviewEvent = context.Message;
        
        try
        {
            _logger.LogInformation("Processing ReviewCreatedEvent for ReviewId: {ReviewId}, PlaceId: {PlaceId}", 
                reviewEvent.ReviewId, reviewEvent.PlaceId);

            // Analyze review text
            var analysisResult = await _sentimentAnalysisService.AnalyzeReviewAsync(
                reviewEvent.Text, 
                context.CancellationToken);

            // Index in Elasticsearch
            await _elasticsearchService.IndexReviewAnalysisAsync(
                reviewEvent.PlaceId,
                reviewEvent.ReviewId,
                analysisResult,
                context.CancellationToken);

            _logger.LogInformation("Successfully analyzed and indexed review {ReviewId} with {TagCount} tags", 
                reviewEvent.ReviewId, analysisResult.Tags.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ReviewCreatedEvent for ReviewId: {ReviewId}", reviewEvent.ReviewId);
            throw; // Re-throw to trigger retry mechanism
        }
    }
}






