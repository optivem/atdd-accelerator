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
        
        // Scripts are packaged with the tool
        _scriptsPath = Path.Combine(toolDirectory, "scripts");
    }

    public async Task GenerateTemplateAsync(string templateName, TemplateOptions options)
    {
        ValidateTemplate(templateName);
        
        var scriptPath = GetScriptPath(templateName);
        
        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Template script not found: {scriptPath}");
        }

        await ExecutePowerShellScriptAsync(scriptPath, options);
    }

    private void ValidateTemplate(string templateName)
    {
        var validTemplates = new[] { "webapi", "console", "classlib" };
        
        if (!Array.Exists(validTemplates, t => t.Equals(templateName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Unknown template '{templateName}'. Available templates: {string.Join(", ", validTemplates)}");
        }
    }

    private string GetScriptPath(string templateName)
    {
        return Path.Combine(_scriptsPath, $"Generate-{templateName.ToTitleCase()}.ps1");
    }

    private async Task ExecutePowerShellScriptAsync(string scriptPath, TemplateOptions options)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = BuildPowerShellArguments(scriptPath, options),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Console.WriteLine($"🔧 Executing PowerShell script: {Path.GetFileName(scriptPath)}");
        
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

    private string BuildPowerShellArguments(string scriptPath, TemplateOptions options)
    {
        return $"-ExecutionPolicy Bypass -File \"{scriptPath}\" " +
               $"-ProjectName \"{options.ProjectName}\" " +
               $"-OutputPath \"{options.OutputPath}\" " +
               $"-RootNamespace \"{options.RootNamespace}\"";
    }
}

public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}