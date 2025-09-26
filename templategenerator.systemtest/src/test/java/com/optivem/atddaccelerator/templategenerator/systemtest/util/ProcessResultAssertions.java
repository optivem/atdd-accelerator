package com.optivem.atddaccelerator.templategenerator.systemtest.util;

import static org.assertj.core.api.Assertions.assertThat;

public class ProcessResultAssertions {

    public static void assertSuccess(ProcessResult result) {
        assertThat(result.isSuccess()).withFailMessage(() -> "Process finished with exit code " + result.getExitCode() + "\nErrors: " + result.getErrors()).isTrue();
    }

    public static void assertFailure(ProcessResult result) {
        assertThat(result.isError()).withFailMessage(() -> "Process finished with exit code " + result.getExitCode() + "\nErrors: " + result.getErrors()).isTrue();
    }

}
