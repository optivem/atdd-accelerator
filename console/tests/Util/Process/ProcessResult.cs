namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process
{
    public class ProcessResult
    {
        public int ExitCode { get; }
        public string Output { get; }
        public string Errors { get; }

        public ProcessResult(int exitCode, string output, string errors)
        {
            ExitCode = exitCode;
            Output = output;
            Errors = errors;
        }

        public bool IsSuccess => ExitCode == 0;
        public bool IsError => !IsSuccess;
    }
}