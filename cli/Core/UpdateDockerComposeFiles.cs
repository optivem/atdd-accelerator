using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

public static class UpdateDockerComposeFiles
{
    public static bool UpdateDockerComposeFilesMethod(string systemLanguage, string repositoryOwner, string repositoryName)
    {
        Console.WriteLine("Updating Docker Compose files using templates...");
        var systemTestFolders = Directory.GetDirectories(Directory.GetCurrentDirectory(), "system-test-*");
        if (systemTestFolders.Length == 0)
        {
            Console.WriteLine("No system-test folder found - skipping Docker Compose update");
            return false;
        }
        if (systemTestFolders.Length > 1)
        {
            Console.WriteLine("Multiple system-test folders found: " + string.Join(", ", systemTestFolders));
            return false;
        }
        var systemTestFolder = systemTestFolders[0];
        var systemTestLanguage = systemTestFolder.Split('-').Last().ToLower();

        var templatePath = Path.Combine("temp", systemLanguage, "docker-compose.yml");
        if (!File.Exists(templatePath))
        {
            Console.WriteLine($"Template Docker Compose file not found: {templatePath}");
            return false;
        }
        var targetDockerCompose = Path.Combine(systemTestFolder, "docker-compose.yml");
        if (!File.Exists(targetDockerCompose))
        {
            Console.WriteLine($"Docker Compose file not found in system-test folder: {targetDockerCompose}");
            return false;
        }
        var templateContent = File.ReadAllText(templatePath);
        var updatedContent = templateContent
            .Replace("ghcr.io/optivem/atdd-accelerator-template-mono-repo/", $"ghcr.io/{repositoryOwner}/{repositoryName}/")
            .Replace($"monolith-{systemTestLanguage}", $"monolith-{systemLanguage.ToLower()}");

        var currentContent = File.ReadAllText(targetDockerCompose);
        if (currentContent == updatedContent)
        {
            Console.WriteLine($"No changes needed for: {targetDockerCompose}");
            return false;
        }
        File.WriteAllText(targetDockerCompose, updatedContent);
        RunProcess("git", $"add {targetDockerCompose}");
        Console.WriteLine($" Updated Docker Compose file: {targetDockerCompose}");
        return true;
    }

    private static string RunProcess(string fileName, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }
}
