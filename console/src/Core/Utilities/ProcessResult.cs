using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public class ProcessResult
    {
        public ProcessResult(int exitCode, string output, string errors)
        {
            ExitCode = exitCode;
            Output = output;
            Errors = errors;
        }

        public int ExitCode { get; }
        public string Output { get; }
        public string Errors { get; }
        public bool IsSuccess => ExitCode == 0;
        public bool IsError => ExitCode != 0;   
    }
}
