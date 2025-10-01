using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

public static class UpdateReadmeBadges
{
    public static bool UpdateReadmeBadgesMethod(string systemLanguage, string repositoryOwner, string repositoryName, string systemTestLanguage)
    {
        Console.WriteLine("Updating README badges...");
        if (!File.Exists("README.md"))
        {
            Console.WriteLine("README.md not found, skipping badge update");
            return false;
        }
        var readmeContent = File.ReadAllText("README.md");
        var originalContent = readmeContent;

        var badgesToRemove = new System.Collections.Generic.List<string>();
        switch (systemLanguage.ToLower())
        {
            case "java":
                badgesToRemove.Add("commit-stage-monolith-dotnet");
                badgesToRemove.Add("commit-stage-monolith-typescript");
                break;
            case "dotnet":
                badgesToRemove.Add("commit-stage-monolith-java");
                badgesToRemove.Add("commit-stage-monolith-typescript");
                break;
            case "typescript":
                badgesToRemove.Add("commit-stage-monolith-java");
                badgesToRemove.Add("commit-stage-monolith-dotnet");
                break;
        }
        foreach (var lang in new[] { "java", "dotnet", "typescript" })
        {
            if (lang != systemTestLanguage.ToLower())
            {
                badgesToRemove.Add($"local-acceptance-stage-test-{lang}");
                badgesToRemove.Add($"acceptance-stage-test-{lang}");
                badgesToRemove.Add($"qa-stage-test-{lang}");
                badgesToRemove.Add($"prod-stage-test-{lang}");
            }
        }
        foreach (var badge in badgesToRemove)
        {
            readmeContent = Regex.Replace(readmeContent, $@".*\[!\[{badge}\].*(?:\r?\n)?", "", RegexOptions.Multiline);
        }
        readmeContent = readmeContent.Replace("optivem/atdd-accelerator-template-mono-repo", $"{repositoryOwner}/{repositoryName}");

        if (readmeContent != originalContent)
        {
            File.WriteAllText("README.md", readmeContent);
            ProcessExecutor.RunProcess("git", "add README.md");
            Console.WriteLine("README badges updated successfully");
            return true;
        }
        Console.WriteLine("No changes needed to README badges");
        return false;
    }
}