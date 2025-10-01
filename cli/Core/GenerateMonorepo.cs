using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator;

public class GenerateMonorepo
{
    private static readonly string[] ValidLanguages = { "java", "dotnet", "typescript" };

    public async Task<int> GenerateAsync(MonorepoOptions options)
    {
        string targetDirectory = null;
        try
        {
            Console.WriteLine("ATDD Accelerator Setup Script");
            Console.WriteLine($"Repository Name: {options.RepositoryName}");
            Console.WriteLine($"System Language: {options.SystemLanguage}");
            Console.WriteLine($"System Test Language: {options.SystemTestLanguage}");

            // Validate languages
            TestSystemLanguage(options.SystemLanguage);
            TestSystemLanguage(options.SystemTestLanguage);

            var githubUsername = GetGitHubUsername(options.GitHubUsername);

            // Get target directory
            targetDirectory = GetOutputDirectory(options.RepositoryName, options.OutputPath);

            // Create repository from template
            NewRepositoryFromTemplate(options.RepositoryName, targetDirectory, options.SystemLanguage, options.SystemTestLanguage);

            // Call imported functions (converted to C#)
            Console.WriteLine("Removing unused language folders...");
            RemoveUnusedLanguageFolders.RemoveUnusedLanguageFoldersMethod(
                options.SystemLanguage,
                options.SystemTestLanguage,
                githubUsername,
                options.RepositoryName);

            Console.WriteLine("Updating README badges...");
            UpdateReadmeBadges.UpdateReadmeBadgesMethod(
                options.SystemLanguage,
                githubUsername,
                options.RepositoryName,
                options.SystemTestLanguage);

            Console.WriteLine("Setting up GitHub Pages...");
            SetupGitHubPages.EnableGitHubPages(
                githubUsername,
                options.RepositoryName);

            Console.WriteLine("Updating Docker Compose...");
            UpdateDockerComposeFiles.UpdateDockerComposeFilesMethod(
                options.SystemLanguage,
                githubUsername,
                options.RepositoryName);

            // Optionally: Call workflow triggers if needed
            // InvokeBuildWorkflows.WaitForBuildWorkflows(...);
            // InvokeSystemTestReleaseWorkflows.InvokeSystemTestWorkflows(...);

            // Push all changes to remote repository (if it's a GitHub repository)
            Console.WriteLine();
            Console.WriteLine("Pushing changes to remote repository...");
            PushChangesToRemote(targetDirectory);

            Console.WriteLine();
            Console.WriteLine("Repository setup completed successfully!");
            Console.WriteLine($"Repository: {githubUsername}/{options.RepositoryName}");
            Console.WriteLine($"Local path: {targetDirectory}");
            Console.WriteLine($"System Language: {options.SystemLanguage}");
            Console.WriteLine($"System Test Language: {options.SystemTestLanguage}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Setup failed: {ex.Message}");

            // Clean up any partially created directories when the script fails
            if (!string.IsNullOrEmpty(targetDirectory) && Directory.Exists(targetDirectory))
            {
                try
                {
                    Console.WriteLine($"Cleaning up failed generation directory: {targetDirectory}");
                    Directory.SetCurrentDirectory(Path.GetTempPath());
                    Directory.Delete(targetDirectory, true);
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine($"Could not clean up directory: {targetDirectory} - {cleanupEx.Message}");
                }
            }
            return 1;
        }
    }

    private void TestSystemLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language) || Array.IndexOf(ValidLanguages, language.ToLower()) < 0)
            throw new ArgumentException($"Invalid SystemLanguage: '{language}'. Valid options: {string.Join(", ", ValidLanguages)}");
        Console.WriteLine($"SystemLanguage '{language}' is valid");
    }

    private string GetGitHubUsername(string providedUsername)
    {
        if (!string.IsNullOrWhiteSpace(providedUsername))
            return providedUsername;

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "gh",
                    Arguments = "auth status",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var match = System.Text.RegularExpressions.Regex.Match(output, @"Logged in to github\.com as (\S+)");
            if (match.Success)
                return match.Groups[1].Value.Trim();
        }
        catch
        {
            Console.WriteLine("GitHub CLI not authenticated. Using default username 'user'");
        }
        return "user";
    }

    private string GetOutputDirectory(string repositoryName, string outputPath)
    {
        //if (!string.IsNullOrWhiteSpace(outputPath))
        //{
        //    var targetDir = Path.Combine(outputPath, repositoryName);
        //    Console.WriteLine($"Using specified output path: {targetDir}");
        //    return targetDir;
        //}
        var tempDir = Path.GetTempPath();
        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
        var atddTempDir = Path.Combine(tempDir, $"ATDD-Accelerator-{uniqueId}");
        var targetDirectory = Path.Combine(atddTempDir, repositoryName);
        Directory.CreateDirectory(atddTempDir);
        Console.WriteLine($"Using temp directory: {targetDirectory}");
        return targetDirectory;
    }

    private void NewRepositoryFromTemplate(string repositoryName, string targetDirectory, string systemLanguage, string systemTestLanguage, string templateName = "optivem/atdd-accelerator-template-mono-repo")
    {
        Console.WriteLine("Creating repository from template...");
        Console.WriteLine($"Target directory: {targetDirectory}");

        var parentDir = Directory.GetParent(targetDirectory).FullName;
        Directory.CreateDirectory(parentDir);

        if (Directory.Exists(targetDirectory))
        {
            Console.WriteLine($"Removing existing directory: {targetDirectory}");
            Directory.Delete(targetDirectory, true);
        }

        var originalLocation = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(parentDir);

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "gh",
                    Arguments = $"repo create {repositoryName} --template {templateName} --public --clone",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            string stdOut = process.StandardOutput.ReadToEnd();
            string stdErr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine("Repository created successfully with GitHub CLI");
                var clonedDir = Path.Combine(parentDir, repositoryName);
                if (clonedDir != targetDirectory)
                    Directory.Move(clonedDir, targetDirectory);
                Directory.SetCurrentDirectory(targetDirectory);
                return;
            }
            else if (stdErr.Contains("already exists") || stdOut.Contains("already exists"))
            {
                throw new Exception($"Repository name '{repositoryName}' already exists on this GitHub account. Please choose a different name.");
            }
            else
            {
                throw new Exception($"GitHub CLI failed with exit code {process.ExitCode}. Error: {stdErr}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GitHub CLI not available or failed: {ex.Message}");
        }
        finally
        {
            Directory.SetCurrentDirectory(originalLocation);
        }
    }

    private void PushChangesToRemote(string targetDirectory)
    {
        Directory.SetCurrentDirectory(targetDirectory);
        try
        {
            var remoteOrigin = RunProcess("git", "remote get-url origin", true);
            if (!string.IsNullOrWhiteSpace(remoteOrigin))
            {
                var status = RunProcess("git", "status --porcelain", true);
                if (!string.IsNullOrWhiteSpace(status))
                {
                    Console.WriteLine("Committing final changes...");
                    RunProcess("git", "add .");
                    RunProcess("git", "commit -m \"Final setup and configuration changes\"");
                }
                Console.WriteLine("Pushing to remote repository...");
                var pushResult = RunProcess("git", "push origin main", true);
                Console.WriteLine(pushResult.Contains("error") ? " Failed to push changes to GitHub" : " Changes pushed successfully to GitHub");
            }
            else
            {
                Console.WriteLine("No remote origin found - skipping push (local repository only)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to push changes: {ex.Message}");
        }
    }

    private string RunProcess(string fileName, string arguments, bool captureOutput = false)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = captureOutput,
                RedirectStandardError = captureOutput,
                UseShellExecute = false
            }
        };
        process.Start();
        string output = captureOutput ? process.StandardOutput.ReadToEnd() : null;
        process.WaitForExit();
        return output;
    }
}

// Extend MonorepoOptions to include GitHubUsername if needed
public class MonorepoOptions
{
    public string RepositoryName { get; set; } = "";
    public string SystemLanguage { get; set; } = "";
    public string SystemTestLanguage { get; set; } = "";
    public string OutputPath { get; set; } = "";
    public string GitHubUsername { get; set; } = "";
}
