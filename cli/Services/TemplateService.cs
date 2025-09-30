using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator;

public class TemplateService
{
    private readonly string _scriptsPath;

    public TemplateService()
    {
        // Get the directory where the tool is installed
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var toolDirectory = Path.GetDirectoryName(assemblyLocation);

        // Scripts are in the same directory as the executable
        _scriptsPath = Path.Combine(toolDirectory, "scripts");
    }

    private string BuildPowerShellArguments(string scriptPath, MonorepoOptions options)
    {
        // Don't pass OutputPath - let PowerShell script decide where to put files
        return $"-ExecutionPolicy Bypass -File \"{scriptPath}\" " +
               $"-RepositoryName \"{options.RepositoryName}\" " +
               $"-SystemLanguage \"{options.SystemLanguage}\" " +
               $"-SystemTestLanguage \"{options.SystemTestLanguage}\"";
    }

    private async Task ExecutePowerShellScriptAsync(string scriptPath, MonorepoOptions options)
    {
        var arguments = BuildPowerShellArguments(scriptPath, options);

        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
            // Remove WorkingDirectory entirely - let PowerShell script handle it
        };

        Console.WriteLine($"Executing PowerShell script: {Path.GetFileName(scriptPath)}");

        using var process = new Process { StartInfo = startInfo };

        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine(output);
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.WriteLine("Errors/Warnings:");
            Console.WriteLine(error);
        }

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"PowerShell script failed with exit code {process.ExitCode}. Error: {error}");
        }
    }

    public async Task GenerateMonorepoAsync(MonorepoOptions options)
    {
        var scriptPath = Path.Combine(_scriptsPath, "Generate-Monorepo.ps1");

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Monorepo script not found: {scriptPath}");
        }

        await ExecutePowerShellScriptAsync(scriptPath, options);

        // Don't report a specific output directory since PowerShell handles it
        Console.WriteLine("Monorepo template generated successfully!");
    }
}