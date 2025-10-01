using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public static class BuildWorkflows
{
    public static bool WaitForBuildWorkflows(string systemLanguage, string repositoryOwner, string repositoryName, int timeoutMinutes = 10)
    {
        Console.WriteLine("Waiting for build workflows to complete...");
        if (!WaitForCommitStageWorkflow(systemLanguage, repositoryOwner, repositoryName, timeoutMinutes))
        {
            Console.Error.WriteLine(" Commit stage workflow did not complete successfully");
            return false;
        }
        Console.WriteLine(" Commit stage workflow completed successfully");

        Console.WriteLine("Checking if Docker image exists...");
        if (!TestDockerImageExists(systemLanguage, repositoryOwner, repositoryName))
        {
            Console.Error.WriteLine(" Docker image does not exist even after commit stage completion");
            return false;
        }
        Console.WriteLine(" Docker image exists");

        return TestContainerHealth(systemLanguage, repositoryOwner, repositoryName);
    }

    public static bool WaitForCommitStageWorkflow(string systemLanguage, string repositoryOwner, string repositoryName, int timeoutMinutes = 10)
    {
        string workflowName = $"commit-stage-monolith-{systemLanguage.ToLower()}";
        Console.WriteLine($"Waiting for workflow '{workflowName}' to complete...");
        DateTime startTime = DateTime.Now;
        DateTime timeout = startTime.AddMinutes(timeoutMinutes);

        while (DateTime.Now < timeout)
        {
            try
            {
                var result = RunProcess("gh", $"run list --repo \"{repositoryOwner}/{repositoryName}\" --workflow \"{workflowName}\" --limit 1 --json status,conclusion,createdAt");
                // Parse JSON and check status/conclusion (use Newtonsoft.Json or System.Text.Json)
                // For brevity, assume success if output contains "completed" and "success"
                if (result.Contains("completed") && result.Contains("success"))
                {
                    Console.WriteLine($" Workflow '{workflowName}' completed successfully");
                    return true;
                }
                else if (result.Contains("completed"))
                {
                    Console.Error.WriteLine($" Workflow '{workflowName}' completed but failed");
                    Console.Error.WriteLine($"Check workflow details at: https://github.com/{repositoryOwner}/{repositoryName}/actions/workflows/{workflowName}.yml");
                    return false;
                }
                else
                {
                    Console.WriteLine(" Workflow is still running...");
                    Task.Delay(30000).Wait();
                }
            }
            catch
            {
                Task.Delay(30000).Wait();
            }
        }
        Console.Error.WriteLine($" Timeout waiting for workflow '{workflowName}' to complete");
        return false;
    }

    public static bool TestDockerImageExists(string systemLanguage, string repositoryOwner, string repositoryName)
    {
        string imageName = $"ghcr.io/{repositoryOwner}/{repositoryName}/monolith-{systemLanguage.ToLower()}:latest";
        var result = RunProcess("docker", $"manifest inspect {imageName}");
        return !string.IsNullOrWhiteSpace(result);
    }

    public static bool TestContainerHealth(string systemLanguage, string repositoryOwner, string repositoryName)
    {
        Console.WriteLine("Checking container health...");
        var status = RunProcess("docker", "ps --filter \"name=monolith\" --format \"table {{.Names}}\\t{{.Status}}\\t{{.Ports}}\"");
        Console.WriteLine(status);

        var logs = RunProcess("docker", "logs --tail 20 $(docker ps -q --filter \"name=monolith\")");
        Console.WriteLine(logs);

        using var client = new HttpClient();
        for (int attempt = 0; attempt < 12; attempt++)
        {
            try
            {
                var response = client.GetAsync("http://localhost:8080/actuator/health").Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(" Application is healthy and responding");
                    return true;
                }
            }
            catch
            {
                Console.WriteLine($" Attempt {attempt + 1}/12 - Application not ready yet...");
                Task.Delay(10000).Wait();
            }
        }
        Console.Error.WriteLine(" Application is not responding after 2 minutes");
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
