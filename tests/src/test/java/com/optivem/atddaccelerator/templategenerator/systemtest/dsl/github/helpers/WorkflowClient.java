package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Constants;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.WorkflowRunResult;
import dev.failsafe.Failsafe;
import dev.failsafe.RetryPolicy;

import java.time.Duration;
import java.util.List;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.process.ProcessResultAssertions.assertSuccess;
import static org.assertj.core.api.Assertions.assertThat;

public class WorkflowClient {

    private final GithubClient client;
    private final String repositoryPath;

    public WorkflowClient (GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
    }

    public void verifyWorkflowsPass(Language systemLanguage, Language systemTestLanguage) {
        verifyWorkflowPasses(Constants.PAGES_BUILD_DEPLOYMENT);
        verifyWorkflowPasses(Constants.COMMIT_STAGE_MONOLITH_FORMAT, systemLanguage);
        verifyWorkflowPasses(Constants.LOCAL_ACCEPTANCE_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyWorkflowPasses(Constants.ACCEPTANCE_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyWorkflowPasses(Constants.QA_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyWorkflowPasses(Constants.PROD_STAGE_TEST_FORMAT, systemTestLanguage);
    }

    private void verifyWorkflowPasses(String workflowFileName) {
        var workflowRun = waitUntilCompleted(workflowFileName);

        assertThat(workflowRun.getConclusion())
                .as("Workflow '" + workflowFileName + "' should pass, but concluded with: " + workflowRun.getConclusion())
                .isEqualTo("success");
    }

    private void verifyWorkflowPasses(String workflowFileNameFormat, Language language) {
        var workflowFileName = String.format(workflowFileNameFormat, language.getValue());
        verifyWorkflowPasses(workflowFileName);
    }

    private WorkflowRunResult waitUntilCompleted(String workflowFileName) {
        RetryPolicy<WorkflowRunResult> retryPolicy = RetryPolicy.<WorkflowRunResult>builder()
                .handleResultIf(result -> !"completed".equals(result.getStatus()))
                .withBackoff(Duration.ofSeconds(1), Duration.ofMinutes(5))  // 1s to 10s
                .withMaxRetries(10)
                .onFailedAttempt(event ->
                        System.out.println("Attempt " + event.getAttemptCount() +
                                ": Workflow status is '" + event.getLastResult().getStatus() +
                                "', retrying in " + event.getElapsedTime() + "..."))
                .build();

        return Failsafe.with(retryPolicy).get(() -> {
            var workflowRun = getWorklowRunResult(workflowFileName);
            if (!"completed".equals(workflowRun.getStatus())) {
                System.out.println("Status: " + workflowRun.getStatus() + ", will retry...");
            }
            return workflowRun;
        });
    }

    private WorkflowRunResult getWorklowRunResult(String workflowFileName) {
        var result = client.viewWorkflowRuns(workflowFileName);
        assertSuccess(result, "Failed to get workflow runs for '" + workflowFileName + "'.");

        var jsonOutput = result.getOutput();
        var workflowRunes = parseWorkflowRuns(jsonOutput);
        return workflowRunes.get(0);
    }

    private List<WorkflowRunResult> parseWorkflowRuns(String jsonOutput) {
        ObjectMapper mapper = new ObjectMapper();
        try {
            return mapper.readValue(jsonOutput, new TypeReference<>() {});
        } catch (JsonProcessingException e) {
            throw new RuntimeException(e);
        }
    }
}
