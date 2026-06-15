namespace tools.Models
{
    public record PredictionResult(string Path, bool Success, string? Error, double? Score) {}
}