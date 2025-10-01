using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Optivem.AtddAccelerator.TemplateGenerator;

public static class RemoveUnusedLanguageFolders
{
    private static readonly string[] Languages = { "java", "dotnet", "typescript" };

    private class LanguageItems
    {
        public List<string> AllItems { get; set; } = new();
        public Dictionary<string, string> LanguageToItemMapping { get; set; } = new();
    }

    private static LanguageItems GetLanguageItems(string pathTemplate)
    {
        var allItems = new List<string>();
        var languageToItemMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var language in Languages)
        {
            var item = pathTemplate.Replace("{language}", language);
            allItems.Add(item);
            languageToItemMapping[language] = item;
        }

        return new LanguageItems
        {
            AllItems = allItems,
            LanguageToItemMapping = languageToItemMapping
        };
    }

    private static List<string> RemoveLanguageSpecificItems(
        string language,
        List<string> allItems,
        Dictionary<string, string> languageToItemMapping,
        List<string> removedItems,
        bool isFolder)
    {
        var keepItem = languageToItemMapping[language.ToLower()];
        Console.WriteLine($"Keeping: {keepItem}");

        foreach (var item in allItems)
        {
            if (!string.Equals(item, keepItem, StringComparison.OrdinalIgnoreCase) && (File.Exists(item) || Directory.Exists(item)))
            {
                Console.WriteLine($"Removing: {item}");

                if (isFolder && Directory.Exists(item))
                {
                    Directory.Delete(item, true);
                    ProcessExecutor.RunProcess("git", $"rm -r {item}");
                }
                else if (File.Exists(item))
                {
                    File.Delete(item);
                    ProcessExecutor.RunProcess("git", $"rm {item}");
                }

                removedItems.Add(item);
            }
        }

        return removedItems;
    }

    private static List<string> RemoveItemsByTemplate(string pathTemplate, string language, List<string> removedItems, bool isFolder)
    {
        var items = GetLanguageItems(pathTemplate);

        return RemoveLanguageSpecificItems(language, items.AllItems, items.LanguageToItemMapping, removedItems, isFolder);
    }

    public static bool RemoveUnusedLanguageFoldersMethod(
        string systemLanguage,
        string systemTestLanguage,
        string repositoryOwner,
        string repositoryName)
    {
        Console.WriteLine("Removing unused language folders...");
        Console.WriteLine($"System Language: {systemLanguage}");
        Console.WriteLine($"System Test Language: {systemTestLanguage}");

        var removedItems = new List<string>();

        // Remove monolith folders
        removedItems = RemoveItemsByTemplate("monolith-{language}", systemLanguage, removedItems, true);

        // Remove system test folders
        removedItems = RemoveItemsByTemplate("system-test-{language}", systemTestLanguage, removedItems, true);

        // Remove commit stage workflow files
        removedItems = RemoveItemsByTemplate(".github/workflows/commit-stage-monolith-{language}.yml", systemLanguage, removedItems, false);

        // Remove local acceptance stage test workflow files
        removedItems = RemoveItemsByTemplate(".github/workflows/local-acceptance-stage-test-{language}.yml", systemTestLanguage, removedItems, false);

        // Remove acceptance stage test workflow files
        removedItems = RemoveItemsByTemplate(".github/workflows/acceptance-stage-test-{language}.yml", systemTestLanguage, removedItems, false);

        // Remove QA workflows
        removedItems = RemoveItemsByTemplate(".github/workflows/qa-stage-test-{language}.yml", systemTestLanguage, removedItems, false);

        // Remove production workflows
        removedItems = RemoveItemsByTemplate(".github/workflows/prod-stage-test-{language}.yml", systemTestLanguage, removedItems, false);

        // Update README badges
        var readmeUpdated = UpdateReadmeBadges.UpdateReadmeBadgesMethod(systemLanguage, repositoryOwner, repositoryName, systemTestLanguage);

        // Update Docker Compose files
        var dockerComposeUpdated = UpdateDockerComposeFiles.UpdateDockerComposeFilesMethod(systemLanguage, repositoryOwner, repositoryName);

        // Commit and return whether changes were made
        var hasChanges = removedItems.Count > 0 || readmeUpdated || dockerComposeUpdated;
        if (hasChanges)
        {
            var changes = new List<string>();
            if (removedItems.Count > 0) changes.Add("Remove unused folders/workflows");
            if (readmeUpdated) changes.Add("Update README badges");
            if (dockerComposeUpdated) changes.Add("Update Docker Compose files");

            var commitMessage = string.Join(", ", changes);
            ProcessExecutor.RunProcess("git", $"commit -m \"{commitMessage}\"");
            return true;
        }

        return false;
    }
}