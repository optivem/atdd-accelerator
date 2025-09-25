package com.optivem.atddaccelerator.templategenerator.systemtest;

import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

import static org.assertj.core.api.Assertions.assertThat;

class SetupScriptTest {

    @Disabled
    @Test
    void setupScript_shouldReturnExitCode0() throws IOException, InterruptedException {
        // Run setup-mono-repo.ps1
        Process process = new ProcessBuilder("pwsh", "scripts/setup-mono-repo.ps1").start();
        
        // Read stdout
        BufferedReader stdout = new BufferedReader(new InputStreamReader(process.getInputStream()));
        StringBuilder output = new StringBuilder();
        String line;
        while ((line = stdout.readLine()) != null) {
            output.append(line).append("\n");
        }
        
        // Read stderr
        BufferedReader stderr = new BufferedReader(new InputStreamReader(process.getErrorStream()));
        StringBuilder errors = new StringBuilder();
        while ((line = stderr.readLine()) != null) {
            errors.append(line).append("\n");
        }
        
        // Wait for process to complete
        int exitCode = process.waitFor();
        
        // Print debug info
        System.out.println("Exit Code: " + exitCode);
        System.out.println("Stdout: " + output.toString());
        System.out.println("Stderr: " + errors.toString());
        
        // Assert that the response is 0
        assertThat(exitCode).isEqualTo(0);
    }
}