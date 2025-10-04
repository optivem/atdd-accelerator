using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public class ProcessExecutor
    {
        private readonly ILogger _logger;

        public ProcessExecutor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProcessExecutor>();
        }

        public ProcessResult RunProcess(string fileName, string arguments)
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

            _logger.LogDebug("Executing command: {FileName} {Arguments}", fileName, arguments);

            process.Start();

            _logger.LogDebug("Process started with PID: {ProcessId}", process.Id);

            var output = process.StandardOutput.ReadToEnd();
            var errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            _logger.LogDebug("Process exited with code: {ExitCode}", process.ExitCode);
            _logger.LogDebug("Process output: {Output}", output);
            _logger.LogDebug("Process errors: {Errors}", errors);

            return new ProcessResult(process.ExitCode, output, errors);
        }
    }
}
