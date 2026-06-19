using System.Text.Json;
using System.CommandLine;

using Tools.Models;

namespace Tools;

internal class Program
{
    private static void ExecuteTool(Tool tool, List<PredictionResult> predictionResults, double? threshold)
    {
        foreach (var result in predictionResults)
        {
            if (result.Success is false)
            {
                continue;
            }
            if (threshold is not null && result.Score < threshold)
            {
                continue;
            }
            // exec tool
        }
    }

    private static void HandleAction(ParseResult result)
    {
        if (result.Errors.Count > 0)
        {
            foreach (var e in result.Errors)
            {
                Console.Error.WriteLine(e);
            }
            return;
        }

        var predictionResults = result.GetValue<List<PredictionResult>>("prediction");
        var tool = result.GetValue<Tool>("--tool");
        var threshold = result.GetValue<double?>("--threshold");

        if (predictionResults is null)
        {
            throw new NullReferenceException(nameof(predictionResults));
        }
        ExecuteTool(tool, predictionResults, threshold);
        return;
    }
    private static int Main(string[] args)
    {
        var positionalOptions = new Argument<List<PredictionResult>>("prediction")
        {
            Description = "Result of the prediction made by the detection module.",
            Arity = ArgumentArity.ExactlyOne,
            CustomParser = r => JsonSerializer.Deserialize(r.Tokens[0].Value, JsonContext.Default.ListPredictionResult) ?? [],
        };
        var toolOptions = new Option<Tool>("--tool")
        {
            Arity = ArgumentArity.ExactlyOne,
            Recursive = true,
            Description = "Tool that will be used by this module.",
        };
        var thresholdOptions = new Option<double?>("--threshold")
        {
            Arity = ArgumentArity.ExactlyOne,
            Recursive = true,
            Description = "Threshold that should be used to filter the scores. Only provide this option if the predictions contain a score.",
        };

        var rootCommand = new RootCommand("Tools module for Quadra.")
        {
            positionalOptions,
            toolOptions,
            thresholdOptions,
        };
        rootCommand.SetAction(HandleAction);
        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}