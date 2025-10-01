using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

class GenerateMonorepo
{
    static readonly string[] ValidLanguages = { "java", "dotnet", "typescript" };

    static int Main(string[] args)
    {
        try
        {
            // Parse arguments (use a proper parser for production)
            string repositoryName = GetArg(args, "--repositoryName", true);
            string githubUsername = GetArg(args, "--githubUsername", false);
            string systemLanguage = GetArg(args, "--systemLanguage", true);
            string systemTestLanguage = GetArg(args, "--systemTestLanguage", true);
            string outputPath = GetArg(args, "--outputPath", false);

            Console.WriteLine("ATDD Accelerator Setup Script");
            Console.WriteLine($"Repository Name: {repositoryName}");
            Console.WriteLine($"System Language: {systemLanguage}");
            Console.WriteLine($"System Test Language: {systemTestLanguage}");

            TestSystemLanguage(systemLanguage);
            TestSystemLanguage(systemTestLanguage);

            githubUsername = GetGitHubUsername(githubUsername);

            string targetDirectory = GetOutputDirectory(repositoryName, outputPath);

            NewRepositoryFromTemplate(repositoryName, targetDirectory, systemLanguage, systemTestLanguage);

            // TODO: Implement helper methods below
            // RemoveUnusedLanguageFolders(systemLanguage, systemTestLanguage, githubUsername, repositoryName, targetDirectory);
            // UpdateReadmeBadges(systemLanguage, githubUsername, repositoryName, systemTestLanguage, targetDirectory);
            // EnableGitHubPages(githubUsername, repositoryName, targetDirectory);
            // UpdateDockerComposeFiles(systemLanguage, githubUsername, repositoryName, targetDirectory);

            PushChangesToRemote(targetDirectory);

            Console.WriteLine("\nRepository setup completed successfully!");
            Console.WriteLine($"Repository: {githubUsername}/{repositoryName}");
            Console.WriteLine($"Local path: {targetDirectory}");
            Console.WriteLine($"System Language: {systemLanguage}");
            Console.WriteLine($"System Test Language: {systemTestLanguage}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Setup failed: {ex.Message}");
            return 1;
        }
    }

    static string GetArg(string[] args, string name, bool required)
    {
        var value = args.SkipWhile(a => a != name).Skip(1).FirstOrDefault();
        if (required && string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Missing required argument: {name}");
        return value;
    }

    static void TestSystemLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language) || !ValidLanguages.Contains(language.ToLower()))
            throw new ArgumentException($"Invalid SystemLanguage: '{language}'. Valid options: {string.Join(", ", ValidLanguages)}");
        Console.WriteLine($"SystemLanguage '{language}' is valid");
    }

    static string GetGitHubUsername(string providedUsername)
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

    static string GetOutputDirectory(string repositoryName, string outputPath)
    {
        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            var targetDir = Path.Combine(outputPath, repositoryName);
            Console.WriteLine($"Using specified output path: {targetDir}");
            return targetDir;
        }
        var tempDir = Path.GetTempPath();
        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
        var atddTempDir = Path.Combine(tempDir, $"ATDD-Accelerator-{uniqueId}");
        var targetDirectory = Path.Combine(atddTempDir, repositoryName);
        Directory.CreateDirectory(atddTempDir);
        Console.WriteLine($"Using temp directory: {targetDirectory}");
        return targetDirectory;
    }

    static void NewRepositoryFromTemplate(string repositoryName, string targetDirectory, string systemLanguage, string systemTestLanguage, string templateName = "optivem/atdd-accelerator-template-mono-repo")
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
            Console.WriteLine("This will create a local directory structure only (not a GitHub repository).");
            Console.Write("Do you want to continue with local setup only? (y/N): ");
            var response = Console.ReadLine();
            if (!response.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Operation cancelled by user.");
                Directory.SetCurrentDirectory(originalLocation);
                Environment.Exit(1);
            }

            Directory.CreateDirectory(targetDirectory);
            Directory.SetCurrentDirectory(targetDirectory);

            var readmeContent = $@"# {repositoryName}

Generated with ATDD Accelerator

- System Language: {systemLanguage}
- System Test Language: {systemTestLanguage}
";
            File.WriteAllText("README.md", readmeContent);

            try
            {
                RunProcess("git", "init");
                RunProcess("git", "add .");
                RunProcess("git", "commit -m \"Initial commit from ATDD Accelerator\"");
                Console.WriteLine("Git repository initialized");
            }
            catch
            {
                Console.WriteLine("Git not available or failed to initialize repository");
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(originalLocation);
        }
    }

    static void PushChangesToRemote(string targetDirectory)
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
                Console.WriteLine(pushResult.Contains("error") ? "⚠️ Failed to push changes to GitHub" : "✅ Changes pushed successfully to GitHub");
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

    static string RunProcess(string fileName, string arguments, bool captureOutput = false)
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