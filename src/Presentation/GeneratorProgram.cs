using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation;

public class GeneratorProgram
{
    public static async Task<int> Main(string[] args)
    {
        if (!await HasInternetConnection())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Error: No internet connection detected. Please check your network and try again.");
            Console.ResetColor();
            return 1;
        }

        await CheckForNewerNugetVersion();

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
            Console.Error.WriteLine($"Error: Command '{args[0]}' is not recognized. Use 'atdd --help' for usage information.");
            return 1;
        }
        catch (ExecutionException ex)
        {
            Console.Error.WriteLine($"Error: {ex.CustomMessage}");
            return 1;
        }
        catch (Exception)
        {
            Console.Error.WriteLine($"Unexpected error occurred.");
            return 1;
        }
    }

    static async Task<bool> HasInternetConnection()
    {
        try
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync("https://github.com", HttpCompletionOption.ResponseHeadersRead);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    static async Task CheckForNewerNugetVersion()
    {
        var packageId = "atdd-accelerator"; // Use your actual PackageId
        var currentVersion = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";

        var latestVersion = await GetLatestNugetVersionAsync(packageId);

        if (latestVersion != null && IsOlderVersion(currentVersion, latestVersion))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Warning: You are using version {currentVersion}, but a newer version {latestVersion} is available on NuGet.org.");
            Console.ResetColor();
        }
    }

    static bool IsOlderVersion(string current, string latest)
    {
        // Use NuGet.Versioning for robust comparison if available
        // Otherwise, fallback to System.Version
        if (Version.TryParse(current.TrimStart('v'), out var currentVer) &&
            Version.TryParse(latest.TrimStart('v'), out var latestVer))
        {
            return currentVer < latestVer;
        }
        return false;
    }

    static async Task<string?> GetLatestNugetVersionAsync(string packageId)
    {
        using var http = new HttpClient();
        var url = $"https://api.nuget.org/v3-flatcontainer/{packageId.ToLowerInvariant()}/index.json";
        var response = await http.GetFromJsonAsync<NugetVersionsResponse>(url);
        return response?.Versions?.LastOrDefault();
    }

    class NugetVersionsResponse
    {
        public List<string>? Versions { get; set; }
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