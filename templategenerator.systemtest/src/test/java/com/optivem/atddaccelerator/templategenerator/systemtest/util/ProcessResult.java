package com.optivem.atddaccelerator.templategenerator.systemtest.util;

import lombok.Data;

@Data
public class ProcessResult {
    private final int exitCode;
    private final String output;
    private final String errors;

    public boolean isSuccess() {
        return exitCode == 0;
    }

    public boolean isError() {
        return !isSuccess();
    }
}
