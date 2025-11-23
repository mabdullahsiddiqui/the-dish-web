namespace TheDish.AI.ReviewAnalysis.Domain.Entities;

public class ReviewAnalysis
{
    public Guid ReviewId { get; set; }
    public Guid PlaceId { get; set; }
    public string Sentiment { get; set; } = string.Empty; // positive, neutral, negative
    public double SentimentConfidence { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, double> TagConfidences { get; set; } = new();
    public DateTime AnalyzedAt { get; set; }
    public string? ErrorMessage { get; set; }
}




