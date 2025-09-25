package com.optivem.atddaccelerator.templategenerator.systemtest.util;

import java.io.BufferedReader;
import java.io.InputStreamReader;

import static org.assertj.core.api.Assertions.assertThat;

public class ProcessExecutor {

    public static ProcessResult executeProcess(String... command)
    {
        var processBuilder = new ProcessBuilder(command);

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

        var outputStr = output.toString();
        var errorsStr = errors.toString();

        return new ProcessResult(exitCode, outputStr, errorsStr);
    }
}
