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
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<Generator> _logger;

    public Generator(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<Generator>();
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

        var optionsValidator = new OptionsValidator(_loggerFactory);
        var result = optionsValidator.Validate(options);
        if(result != 0)
        {
            return result;
        }

        var context = OptionsConverter.Convert(options);

        _logger.LogDebug("Context created:");
        _logger.LogDebug("  Repository: {RepositoryOwner}/{RepositoryName}", context.RepositoryOwner, context.RepositoryName);
        _logger.LogDebug("  System Language: {SystemLanguage}", context.SystemLanguage);
        _logger.LogDebug("  System Test Language: {SystemTestLanguage}", context.SystemTestLanguage);
        _logger.LogDebug("  Output Path: {OutputPath}", context.OutputPath);

        var processExecutor = new ProcessExecutor(_loggerFactory);
        var templateRepositoryGenerator = new TemplateRepositoryGenerator(context, processExecutor, _loggerFactory);
        await templateRepositoryGenerator.GenerateAsync();

        _logger.LogInformation("Repository '{RepositoryName}' created successfully under owner '{RepositoryOwner}'.", context.RepositoryName, context.RepositoryOwner);
        _logger.LogInformation("GitHub URL: https://github.com/{RepositoryOwner}/{RepositoryName}", context.RepositoryOwner, context.RepositoryName);

        return 0;
    }
}
