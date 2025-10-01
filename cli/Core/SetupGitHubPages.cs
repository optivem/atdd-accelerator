using System;
using System.Diagnostics;

public static class SetupGitHubPages
{
    public static bool EnableGitHubPages(string repositoryOwner, string repositoryName)
    {
        Console.WriteLine("Enabling GitHub Pages...");
        var result = RunProcess("gh", $"api -X POST \"repos/{repositoryOwner}/{repositoryName}/pages\" -f \"source[branch]=main\" -f \"source[path]=/docs\"");
        if (!string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine("GitHub Pages enabled successfully");
            return true;
        }
        Console.WriteLine($"Failed to enable GitHub Pages: {result}");
        return false;
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