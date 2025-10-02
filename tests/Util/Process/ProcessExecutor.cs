using System.Diagnostics;
using System.Text;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process
{
    public static class ProcessExecutor
    {
        public static ProcessResult ExecuteProcess(params string[] command)
        {
            if (command.Length == 0)
                throw new ArgumentException("Command cannot be empty", nameof(command));

            var processInfo = new ProcessStartInfo
            {
                FileName = command[0],
                Arguments = string.Join(" ", command.Skip(1).Select(arg => $"\"{arg}\"")),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var output = new StringBuilder();
            var errors = new StringBuilder();

            try
            {
                using var process = new System.Diagnostics.Process { StartInfo = processInfo };
                
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        output.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        errors.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                return new ProcessResult(process.ExitCode, output.ToString(), errors.ToString());
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to execute script", e);
            }
        }

        public static void WaitTime(int millis)
        {
            Thread.Sleep(millis);
        }
    }
}