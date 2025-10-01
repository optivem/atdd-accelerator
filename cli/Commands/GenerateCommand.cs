using System;
using System.IO;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator;

public class GenerateCommand
{
    private readonly TemplateService _templateService;

    public GenerateCommand(TemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error: Please specify a template name.");
            Console.WriteLine("Usage: atdd generate monorepo [options]");
            return 1;
        }

        var templateName = args[0];
        
        if (!templateName.Equals("monorepo", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Error: Unknown template '{templateName}'");
            Console.WriteLine("Available templates: monorepo");
            return 1;
        }

        var options = ParseMonorepoOptions(args);

        if (string.IsNullOrEmpty(options.RepositoryName))  // Fixed: Changed from ProjectName to RepositoryName
        {
            Console.WriteLine("Error: --repository-name is required.");
            Console.WriteLine("Usage: atdd generate monorepo --repository-name <name> [options]");
            return 1;
        }

        if (string.IsNullOrEmpty(options.SystemLanguage))
        {
            Console.WriteLine("Error: --system-language is required.");
            return 1;
        }

        if (string.IsNullOrEmpty(options.SystemTestLanguage))
        {
            Console.WriteLine("Error: --system-test-language is required.");
            return 1;
        }

        try
        {
            Console.WriteLine($"🚀 Generating monorepo template...");
            
            await _templateService.GenerateMonorepoAsync(options);
            
            Console.WriteLine($"✅ Monorepo template generated successfully!");
            Console.WriteLine($"📁 Output directory: {options.OutputPath}");
            Console.WriteLine($"📦 Repository name: {options.RepositoryName}");  // Fixed: Changed from ProjectName to RepositoryName
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating template: {ex.Message}");
            return 1;
        }
    }

    private MonorepoOptions ParseMonorepoOptions(string[] args)
    {
        var options = new MonorepoOptions
        {
            OutputPath = Directory.GetCurrentDirectory()
        };

        for (int i = 1; i < args.Length; i += 2)
        {
            if (i + 1 >= args.Length) break;

            switch (args[i])
            {
                case "--repository-name":
                    options.RepositoryName = args[i + 1];
                    break;
                case "--system-language":
                    options.SystemLanguage = args[i + 1];
                    break;
                case "--system-test-language":
                    options.SystemTestLanguage = args[i + 1];
                    break;
                case "--output-path":
                    options.OutputPath = Path.GetFullPath(args[i + 1]);
                    break;
            }
        }

        return options;
    }
}