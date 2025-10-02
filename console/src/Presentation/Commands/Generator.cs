using Optivem.AtddAccelerator.TemplateGenerator.Application;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands;

public class Generator
{
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

        var result = OptionsValidator.Validate(options);
        if(result != 0)
        {
            return result;
        }

        try
        {
            Console.WriteLine($" Generating monorepo template...");
            var context = OptionsConverter.Convert(options);

            var templateRepositoryGenerator = new TemplateRepositoryGenerator(context);
            await templateRepositoryGenerator.GenerateAsync();
            
            Console.WriteLine($" Monorepo template generated successfully!");
            Console.WriteLine($" Repository name: {options.RepositoryName}");
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($" Error generating template: {ex.Message} \n \n {ex.ToString()}");
            return 1;
        }
    }
}
