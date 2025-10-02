using Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation;

public class GeneratorProgram
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            // Handle version command
            if (args.Length > 0 && (args[0] == "--version" || args[0] == "-v"))
            {
                ShowVersion();
                return 0;
            }

            // Handle help command
            if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
            {
                ShowHelp();
                return 0;
            }

            // Handle generate command
            if (args[0] == "generate")
            {
                var generator = new Generator();
                var generatorArgs = args.Skip(1).ToArray();
                return await generator.ExecuteAsync(generatorArgs);
            }

            // Unknown command
            Console.WriteLine($"Unknown command: {args[0]}");
            Console.WriteLine("Use 'atdd --help' for usage information.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to generate repository. {ex.Message}");
            return 1;
        }
    }

    static void ShowVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";

        Console.WriteLine($"ATDD Accelerator Template Generator");
        Console.WriteLine($"Version: {version}");
        Console.WriteLine();
        Console.WriteLine("Copyright (c) Optivem");
        Console.WriteLine("Licensed under MIT License");
        Console.WriteLine("Repository: https://github.com/optivem/atdd-accelerator");
    }

    static void ShowHelp()
    {
        Console.WriteLine("ATDD Accelerator Template Generator");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  atdd generate monorepo [options]");
        Console.WriteLine("  atdd --version");
        Console.WriteLine("  atdd --help");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  generate monorepo       Generate a monorepo with ATDD structure");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --repository-name <name>        Repository name (required)");
        Console.WriteLine("  --system-language <language>    System language (required)");
        Console.WriteLine("  --system-test-language <lang>   System test language (required)");
        Console.WriteLine("  --output-path <path>            Output directory (default: current directory)");
        Console.WriteLine("  --version, -v                   Show version information");
        Console.WriteLine("  --help, -h                      Show help information");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  atdd generate monorepo --repository-name MyRepo --system-language CSharp --system-test-language Java");
        Console.WriteLine();
        Console.WriteLine("For more information, visit:");
        Console.WriteLine("https://github.com/optivem/atdd-accelerator");
    }
}