using System.Text.Json;
using System.CommandLine;

using Tools.Models;

namespace Tools;

internal class Program
{
    private static void Main(string[] args)
    {
        var positionalOptions = new Argument<List<PredictionResult>>("prediction")
        {
            Description = "Result of the prediction made by the detection module.",
            Arity = ArgumentArity.ExactlyOne,
            CustomParser = r => JsonSerializer.Deserialize(r.Tokens[0].Value, JsonContext.Default.ListPredictionResult) ?? []
        };
        var toolOptions = new Option<Tool>("--tool")
        {
            Arity = ArgumentArity.ExactlyOne,
            Recursive = true,
            Description = "Tool that will be used by this module.",
        };
        var thresholdOptions = new Option<double>("--threshold")
        {
            Arity = ArgumentArity.ExactlyOne,
            Recursive = true,
            Description = "Threshold that should be used to filter the scores. Only provide this option if the predictions contain a score."
        };

        var rootCommand = new RootCommand("Tools module for Quadra.")
        {
            positionalOptions,
            toolOptions,
            thresholdOptions
        };
    }
}