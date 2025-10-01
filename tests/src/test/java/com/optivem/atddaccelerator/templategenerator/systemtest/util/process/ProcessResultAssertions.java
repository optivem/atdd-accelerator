package com.optivem.atddaccelerator.templategenerator.systemtest.util.process;

import static org.assertj.core.api.Assertions.assertThat;

public class ProcessResultAssertions {

    public static void assertSuccess(ProcessResult result) {
        assertThat(result.isSuccess()).withFailMessage(() -> stringify(result)).isTrue();
    }

    public static void assertFailure(ProcessResult result) {
        assertThat(result.isError()).withFailMessage(() -> stringify(result)).isTrue();
    }

    public static void assertSuccess(ProcessResult result, String customMessage) {
        assertThat(result.isSuccess()).withFailMessage(() -> stringify(result, customMessage)).isTrue();
    }

    public static void assertFailure(ProcessResult result, String customMessage) {
        assertThat(result.isError()).withFailMessage(() -> stringify(result, customMessage)).isTrue();
    }

    private static String stringify(ProcessResult result) {
        return "Process finished with exit code " + result.getExitCode() + "\nErrors: " + result.getErrors() + "\nOutput: " + result.getOutput();
    }

    private static String stringify(ProcessResult result, String customMessage) {
        return customMessage + "\n" + stringify(result);
    }

}
