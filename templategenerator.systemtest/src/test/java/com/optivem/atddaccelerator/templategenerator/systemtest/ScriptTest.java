package com.optivem.atddaccelerator.templategenerator.systemtest;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.execute;
import static org.assertj.core.api.Assertions.assertThat;

class SetupScriptTest {

    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    @Test
    void setupScript_shouldReturnExitCode0() throws IOException, InterruptedException {
        var processBuilder = new ProcessBuilder("pwsh", SCRIPT_PATH);
        execute(processBuilder);
    }
}