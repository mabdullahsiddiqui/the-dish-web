namespace TheDish.Review.Application.DTOs;

public class ReviewAnalysisDto
{
    public string Sentiment { get; set; } = string.Empty;
    public double SentimentConfidence { get; set; }
    public List<ReviewTagDto> Tags { get; set; } = new();
}

public class ReviewTagDto
{
    public string Tag { get; set; } = string.Empty;
    public double Confidence { get; set; }
}








