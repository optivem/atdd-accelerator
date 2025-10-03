using Microsoft.Extensions.Logging;
using Optivem.AtddAccelerator.TemplateGenerator.Application;
using Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Optivem.AtddAccelerator.TemplateGenerator.Presentation.Commands;

public class Generator
{
    private readonly ILogger<Generator> _logger;
    private readonly ILogger<TemplateRepositoryGenerator> _templateRepositoryGeneratorLogger;
    private readonly ILogger<OptionsValidator> _optionsValidatorLogger;

    public Generator(ILogger<Generator> logger, 
        ILogger<TemplateRepositoryGenerator> templateRepositoryGeneratorLogger,
        ILogger<OptionsValidator> optionsValidatorLogger)
    {
        _logger = logger;
        _templateRepositoryGeneratorLogger = templateRepositoryGeneratorLogger;
        _optionsValidatorLogger = optionsValidatorLogger;
    }

    public async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            _logger.LogError("Error: Please specify a template name.");
            _logger.LogError("Usage: atdd generate monorepo [options]");
            return 1;
        }

        var templateName = args[0];
        
        if (!templateName.Equals("monorepo", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError("Error: Unknown template '{TemplateName}'", templateName);
            _logger.LogError("Available templates: monorepo");
            return 1;
        }

        var options = OptionsParser.ParseMonorepoOptions(args);

        var optionsValidator = new OptionsValidator(_optionsValidatorLogger);
        var result = optionsValidator.Validate(options);
        if(result != 0)
        {
            return result;
        }

        var context = OptionsConverter.Convert(options);

        var templateRepositoryGenerator = new TemplateRepositoryGenerator(context, _templateRepositoryGeneratorLogger);
        await templateRepositoryGenerator.GenerateAsync();

        _logger.LogInformation("Repository '{RepositoryName}' created successfully under owner '{RepositoryOwner}'.", context.RepositoryName, context.RepositoryOwner);
        _logger.LogInformation("GitHub URL: https://github.com/{RepositoryOwner}/{RepositoryName}", context.RepositoryOwner, context.RepositoryName);

        return 0;
    }
}
