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

    public async Task GenerateMonorepoAsync(MonorepoOptions options)
    {
        var scriptPath = Path.Combine(_scriptsPath, "Generate-Monorepo.ps1");
        
        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Monorepo script not found: {scriptPath}");
        }

        await ExecutePowerShellScriptAsync(scriptPath, options);
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
            CreateNoWindow = true,
            WorkingDirectory = options.OutputPath
        };

        Console.WriteLine($"🔧 Executing: {Path.GetFileName(scriptPath)}");
        Console.WriteLine($"📁 Working directory: {options.OutputPath}");
        
        using var process = new Process { StartInfo = startInfo };
        
        process.Start();
        
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"PowerShell script failed with exit code {process.ExitCode}. Error: {error}");
        }

        if (!string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine(output);
        }
    }

    private string BuildPowerShellArguments(string scriptPath, MonorepoOptions options)
    {
        return $"-ExecutionPolicy Bypass -File \"{scriptPath}\" " +
               $"-RepositoryName \"{options.RepositoryName}\" " +
               $"-SystemLanguage \"{options.SystemLanguage}\" " +
               $"-SystemTestLanguage \"{options.SystemTestLanguage}\"";
    }
}