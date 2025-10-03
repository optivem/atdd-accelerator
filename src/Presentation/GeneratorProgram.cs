using Microsoft.Extensions.Logging;
using Optivem.AtddAccelerator.TemplateGenerator.Application;
using Optivem.AtddAccelerator.TemplateGenerator.Domain.Exceptions;
using Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation;

public class GeneratorProgram
{
    private static ILogger? _logger;

    private static string GetLogDirectoryPath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"{ApplicationConstants.PackageId}-logs");
    }

    public static async Task<int> Main(string[] args)
    {
        // Create logs directory in user's home folder
        var logDirectory = GetLogDirectoryPath();
        Directory.CreateDirectory(logDirectory);
        
        var logFilePath = Path.Combine(logDirectory, $"{ApplicationConstants.PackageId}-{DateTime.Now:yyyy-MM-dd}.log");

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(logFilePath, 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog();
        });
        _logger = loggerFactory.CreateLogger<GeneratorProgram>();

        _logger.LogInformation("Application started.");
        _logger.LogInformation("Log file location: {LogFilePath}", logFilePath);

        if (!await HasInternetConnection())
        {
            _logger.LogError("Error: No internet connection detected. Please check your network and try again.");
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
                var generatorLogger = loggerFactory.CreateLogger<Generator>();
                var templateRepositoryGeneratorLogger = loggerFactory.CreateLogger<Application.TemplateRepositoryGenerator>();
                var generator = new Generator(generatorLogger, templateRepositoryGeneratorLogger);
                var generatorArgs = args.Skip(1).ToArray();
                return await generator.ExecuteAsync(generatorArgs);
            }

            // Unknown command
            _logger.LogError("Error: Command '{Command}' is not recognized. Use 'atdd --help' for usage information.", args[0]);
            return 1;
        }
        catch (ExecutionException ex)
        {
            _logger.LogError("Error: {CustomMessage}", ex.CustomMessage);
            return 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
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
        var packageId = ApplicationConstants.PackageId; // Use your actual PackageId
        var currentVersion = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";

        var latestVersion = await GetLatestNugetVersionAsync(packageId);

        if (latestVersion != null && IsOlderVersion(currentVersion, latestVersion))
        {
            _logger?.LogWarning("Warning: You are using version {CurrentVersion}, but a newer version {LatestVersion} is available on NuGet.org.", currentVersion, latestVersion);
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
        var logDirectory = GetLogDirectoryPath();

        _logger?.LogInformation("ATDD Accelerator Template Generator");
        _logger?.LogInformation("Version: {Version}", version);
        _logger?.LogInformation("");
        _logger?.LogInformation("Copyright (c) Optivem");
        _logger?.LogInformation("Licensed under MIT License");
        _logger?.LogInformation("Repository: https://github.com/optivem/atdd-accelerator");
        _logger?.LogInformation("Log files stored in: {LogDirectory}", logDirectory);
    }

    static void ShowHelp()
    {

        var logDirectory = GetLogDirectoryPath();

        _logger?.LogInformation("ATDD Accelerator CLI");
        _logger?.LogInformation("");
        _logger?.LogInformation("Usage:");
        _logger?.LogInformation("  atdd generate monorepo [options]");
        _logger?.LogInformation("  atdd --version");
        _logger?.LogInformation("  atdd --help");
        _logger?.LogInformation("");
        _logger?.LogInformation("Commands:");
        _logger?.LogInformation("  generate monorepo       Generate a monorepo with ATDD structure");
        _logger?.LogInformation("");
        _logger?.LogInformation("Options:");
        _logger?.LogInformation("  --repository-owner <name>       Repository owner (required) - Your GitHub username or organization name");
        _logger?.LogInformation("  --repository-name <name>        Repository name (required) - Unique name for the new repository you want to create");
        _logger?.LogInformation("  --system-language <language>    System language (required) - Choose the language for source code: java, dotnet, typescript");
        _logger?.LogInformation("  --system-test-language <lang>   System test language (required) - Choose the language for system tests: java, dotnet, typescript");
        _logger?.LogInformation("  --version, -v                   Show version information");
        _logger?.LogInformation("  --help, -h                      Show help information");
        _logger?.LogInformation("");
        _logger?.LogInformation("Examples:");
        _logger?.LogInformation("  atdd generate monorepo --repository-owner jsmith --repository-name eshop99 --system-language java --system-test-language typescript");
        _logger?.LogInformation("");
        _logger?.LogInformation("For more information, visit:");
        _logger?.LogInformation("https://github.com/optivem/atdd-accelerator");

        _logger?.LogInformation("Troubleshooting:");
        _logger?.LogInformation("  Log files are stored in: {LogDirectory}", logDirectory);
        _logger?.LogInformation("  When reporting issues, please include the relevant log file.");
    }
}