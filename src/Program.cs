using System.Text.Json;
using System.CommandLine;

using Tools.Models;
using Microsoft.VisualBasic.FileIO;

namespace Tools;

internal class Program
{
    private static void ExecuteTool(Tool tool, List<PredictionResult> predictionResults, double? threshold, string? destination)
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
            switch (tool)
            {
                case Tool.Remove:
                    FileSystem.DeleteFile(result.Path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    break;
                case Tool.Move:
                    File.Move(result.Path, destination!);
                    break;
                case Tool.Copy:
                    File.Move(result.Path, destination!);
                    break;
                default:
                    break;
            }
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
        var destination = result.GetValue<string?>("--destination");

        if (predictionResults is null)
        {
            throw new NullReferenceException(nameof(predictionResults));
        }
        if (destination is null && tool is not Tool.Remove)
        {
            throw new InvalidOperationException("A destination should be provided to move/copy the prediction results with '--destination'");
        }
        if (threshold is null && predictionResults.Any(r => r.Score is not null))
        {
            throw new InvalidOperationException("A threshold shall be provided with '--threshold' when using an ML detection module.");
        }
        ExecuteTool(tool, predictionResults, threshold, destination);
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
        var destinationOptions = new Option<string?>("--destination")
        {
            Arity = ArgumentArity.ExactlyOne,
            Recursive = true,
            Description = "The destination where the file should be moved/copied.",
        };

        var rootCommand = new RootCommand("Tools module for Quadra.")
        {
            positionalOptions,
            toolOptions,
            thresholdOptions,
            destinationOptions
        };
        rootCommand.SetAction(HandleAction);
        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}