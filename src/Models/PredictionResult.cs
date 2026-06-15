using System.Text.Json.Serialization;

namespace Tools.Models
{
    /// <summary>
    /// Represent the prediction result made by the detection module to take action on.
    /// </summary>
    /// <param name="Path">Path to the media analyzed by the detection module.</param>
    /// <param name="Success">If the analysis was a success or not.</param>
    /// <param name="Error">The error message (not needed)</param>
    /// <param name="Score">The score of detection made by the detection module. Used when "--threshold" is provided</param>
    public record PredictionResult(string Path, bool Success, string? Error, double? Score) { }

    [JsonSerializable(typeof(List<PredictionResult>))]
    internal partial class JsonContext : JsonSerializerContext { }
}
