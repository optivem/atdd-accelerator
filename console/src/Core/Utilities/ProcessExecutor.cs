using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivem.AtddAccelerator.TemplateGenerator.Core.Utilities
{
    public static class ProcessExecutor
    {
        public static string RunProcess(string fileName, string arguments, bool captureOutput = false)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = captureOutput,
                    RedirectStandardError = captureOutput,
                    UseShellExecute = false
                }
            };
            process.Start();
            string output = captureOutput ? process.StandardOutput.ReadToEnd() : null;
            process.WaitForExit();
            return output;
        }
    }
}
