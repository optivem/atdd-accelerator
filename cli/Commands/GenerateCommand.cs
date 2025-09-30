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
            Console.WriteLine("Usage: atdd generate <template> [options]");
            Console.WriteLine("Available templates: webapi, console, classlib");
            return 1;
        }

        var templateName = args[0];
        var options = ParseOptions(args);

        try
        {
            Console.WriteLine($"🚀 Generating {templateName} template...");
            
            await _templateService.GenerateTemplateAsync(templateName, options);
            
            Console.WriteLine($"✅ {templateName} template generated successfully!");
            Console.WriteLine($"📁 Output directory: {options.OutputPath}");
            Console.WriteLine($"📦 Project name: {options.ProjectName}");
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error generating template: {ex.Message}");
            return 1;
        }
    }

    private TemplateOptions ParseOptions(string[] args)
    {
        var options = new TemplateOptions
        {
            ProjectName = Path.GetFileName(Directory.GetCurrentDirectory()),
            OutputPath = Directory.GetCurrentDirectory()
        };

        for (int i = 1; i < args.Length; i += 2)
        {
            if (i + 1 >= args.Length) break;

            switch (args[i])
            {
                case "--name":
                    options.ProjectName = args[i + 1];
                    break;
                case "--output":
                    options.OutputPath = Path.GetFullPath(args[i + 1]);
                    break;
                case "--namespace":
                    options.RootNamespace = args[i + 1];
                    break;
            }
        }

        // Set default namespace to project name if not specified
        options.RootNamespace ??= options.ProjectName;

        return options;
    }
}

public class TemplateOptions
{
    public string ProjectName { get; set; } = "";
    public string OutputPath { get; set; } = "";
    public string? RootNamespace { get; set; }
}