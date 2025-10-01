namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util.Process
{
    public static class ProcessResultAssertions
    {
        public static void AssertSuccess(ProcessResult result)
        {
            Assert.True(result.IsSuccess, Stringify(result));
        }

        public static void AssertFailure(ProcessResult result)
        {
            Assert.True(result.IsError, Stringify(result));
        }

        public static void AssertSuccess(ProcessResult result, string customMessage)
        {
            Assert.True(result.IsSuccess, Stringify(result, customMessage));
        }

        public static void AssertFailure(ProcessResult result, string customMessage)
        {
            Assert.True(result.IsError, Stringify(result, customMessage));
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