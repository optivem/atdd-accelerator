using Optivem.AtddAccelerator.TemplateGenerator.Commands;
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

        var options = OptionsParser.ParseMonorepoOptions(args);

        OptionsValidator.Validate(options);

        try
        {
            Console.WriteLine($" Generating monorepo template...");
            
            await _templateService.GenerateMonorepoAsync(options);
            
            Console.WriteLine($" Monorepo template generated successfully!");
            Console.WriteLine($" Repository name: {options.RepositoryName}");
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Error generating template: {ex.Message}");
            return 1;
        }
    }
}
