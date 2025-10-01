using System;
using System.Diagnostics;
using System.Threading.Tasks;

public static class SystemTestReleaseWorkflows
{
    public static bool InvokeSystemTestWorkflows(string systemTestLanguage, string repositoryOwner, string repositoryName)
    {
        Console.WriteLine($"Triggering system test workflows for language: {systemTestLanguage}");
        string[] workflows = {
            $"local-acceptance-stage-test-{systemTestLanguage.ToLower()}",
            $"acceptance-stage-test-{systemTestLanguage.ToLower()}",
            $"qa-stage-test-{systemTestLanguage.ToLower()}",
            $"prod-stage-test-{systemTestLanguage.ToLower()}"
        };

        int triggered = 0, failed = 0;
        foreach (var workflow in workflows)
        {
            Console.WriteLine($"Triggering workflow: {workflow}");
            var result = RunProcess("gh", $"workflow run {workflow} --repo \"{repositoryOwner}/{repositoryName}\"");
            if (!string.IsNullOrWhiteSpace(result))
            {
                Console.WriteLine($"  ✅ Successfully triggered: {workflow}");
                triggered++;
            }
            else
            {
                Console.WriteLine($"  ❌ Failed to trigger: {workflow}");
                failed++;
            }
            Task.Delay(500).Wait();
        }
        Console.WriteLine("\nWorkflow trigger summary:");
        Console.WriteLine($"  ✅ Successfully triggered ({triggered})");
        Console.WriteLine($"  ❌ Failed to trigger ({failed})");
        return triggered > 0;
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