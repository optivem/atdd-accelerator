using FluentAssertions;
using Xunit;

namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process
{
    public static class ProcessResultAssertions
    {
        public static void ShouldSucceed(this ProcessResult result)
        {
            result.IsSuccess.Should().BeTrue(Stringify(result));
        }

        public static void ShouldSucceed(this ProcessResult result, string customMessage)
        {
            result.IsSuccess.Should().BeTrue(Stringify(result, customMessage));
        }

        public static void ShouldFail(this ProcessResult result)
        {
            result.IsError.Should().BeTrue(Stringify(result));
        }

        public static void ShouldFail(this ProcessResult result, string customMessage)
        {
            result.IsError.Should().BeTrue(Stringify(result, customMessage));
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