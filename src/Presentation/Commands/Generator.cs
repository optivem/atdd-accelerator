using Optivem.AtddAccelerator.TemplateGenerator.Application;
using System;
using System.IO;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands;

public class Generator
{
    public async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Error: Please specify a template name.");
            Console.Error.WriteLine("Usage: atdd generate monorepo [options]");
            return 1;
        }

        var templateName = args[0];
        
        if (!templateName.Equals("monorepo", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine($"Error: Unknown template '{templateName}'");
            Console.Error.WriteLine("Available templates: monorepo");
            return 1;
        }

        var options = OptionsParser.ParseMonorepoOptions(args);

        var result = OptionsValidator.Validate(options);
        if(result != 0)
        {
            return result;
        }

        var context = OptionsConverter.Convert(options);

        var templateRepositoryGenerator = new TemplateRepositoryGenerator(context);
        await templateRepositoryGenerator.GenerateAsync();

        Console.WriteLine($"Repository was successfully created: {options.RepositoryName}");

        return 0;
    }
}
