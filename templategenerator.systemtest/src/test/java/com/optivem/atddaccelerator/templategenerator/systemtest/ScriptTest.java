package com.optivem.atddaccelerator.templategenerator.systemtest;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import static org.assertj.core.api.Assertions.assertThat;

class SetupScriptTest {

    private static final String SCRIPT_PATH = "../scripts/setup-mono-repo.ps1";

    @Test
    void setupScript_shouldReturnExitCode0() throws IOException, InterruptedException {
        var processBuilder = new ProcessBuilder("pwsh", SCRIPT_PATH);
        execute(processBuilder);
    }

    private static void execute(ProcessBuilder processBuilder) {
        int exitCode;
        StringBuilder output = new StringBuilder();
        StringBuilder errors = new StringBuilder();

        try {
            Process process = processBuilder.start();

            // Read stdout
            BufferedReader stdout = new BufferedReader(new InputStreamReader(process.getInputStream()));

            String line;
            while ((line = stdout.readLine()) != null) {
                output.append(line).append("\n");
            }

            // Read stderr
            BufferedReader stderr = new BufferedReader(new InputStreamReader(process.getErrorStream()));

            while ((line = stderr.readLine()) != null) {
                errors.append(line).append("\n");
            }

            exitCode = process.waitFor();

        }catch(Exception e) {
            throw new RuntimeException("Failed to execute script", e);
        }

        assertThat(exitCode)
                .withFailMessage("Exit code is not 0.\nErrors: %s\nOutput: %s", errors.toString(), output.toString())
                .isZero();

    }
}