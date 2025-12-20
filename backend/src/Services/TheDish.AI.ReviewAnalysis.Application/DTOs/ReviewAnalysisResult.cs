namespace TheDish.AI.ReviewAnalysis.Application.DTOs;

public class ReviewAnalysisResult
{
    public string Sentiment { get; set; } = string.Empty; // positive, neutral, negative
    public double SentimentConfidence { get; set; }
    public List<TagResult> Tags { get; set; } = new();
}

public class TagResult
{
    public string Tag { get; set; } = string.Empty;
    public double Confidence { get; set; }
}








