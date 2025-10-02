using Shouldly;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process
{
    public static class ProcessResultAssertions
    {
        public static void ShouldSucceed(this ProcessResult result)
        {
            result.IsSuccess.ShouldBeTrue(Stringify(result));
        }

        public static void ShouldSucceed(this ProcessResult result, string customMessage)
        {
            result.IsSuccess.ShouldBeTrue(Stringify(result, customMessage));
        }

        public static void ShouldFail(this ProcessResult result)
        {
            result.IsError.ShouldBeTrue(Stringify(result));
        }

        public static void ShouldFail(this ProcessResult result, string customMessage)
        {
            result.IsError.ShouldBeTrue(Stringify(result, customMessage));
        }

        private static string Stringify(ProcessResult result)
        {
            return $"Process finished with exit code {result.ExitCode}\nErrors: {result.Errors}\nOutput: {result.Output}";
        }

        private static string Stringify(ProcessResult result, string customMessage)
        {
            return $"{customMessage}\n{Stringify(result)}";
        }
    }
}