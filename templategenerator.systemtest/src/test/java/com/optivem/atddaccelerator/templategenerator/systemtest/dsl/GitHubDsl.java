package com.optivem.atddaccelerator.templategenerator.systemtest.dsl;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.WorkflowRunResult;
import dev.failsafe.Failsafe;
import dev.failsafe.RetryPolicy;

import java.time.Duration;
import java.util.List;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.executeProcess;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertFailure;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;
import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class GitHubDsl {

    private static final String README_PATH = "README.md";

    private final GithubClient client;

    public GitHubDsl(GithubClient client) {
        this.client = client;
    }

    public void verifyRepositoryExists() {
        var result = client.viewRepository();
        assertSuccess(result);
    }

    public void verifyPathExists(String path) {
        var result = client.viewPath(path);
        assertSuccess(result, "Path '" + path + "' should exist.");
    }

    public void verifyPathDoesNotExist(String path) {
        var result = client.viewPath(path);
        assertFailure(result, "Path '" + path + "' should not exist.");
    }

    public void deleteRepository() {
        var result = client.deleteRepository();
        assertSuccess(result);
    }

    public void verifyReadmeContainsBadge(String badge) {
        var readmeContent = getReadmeContent();
        assertTrue(readmeContent.contains(badge));
    }

    public void verifyReadmeDoesNotContainBadge(String badge) {
        var readmeContent = getReadmeContent();
        assertThat(readmeContent).doesNotContain(badge);
    }

    public void verifyReadmeContainsBadge(String badge, String badgeSvg, String badgeWorkflow) {
        var readmeContent = getReadmeContent();
        assertTrue(readmeContent.contains(badge));
        assertTrue(readmeContent.contains(badgeSvg));
        assertTrue(readmeContent.contains(badgeWorkflow));
    }

    private String getReadmeContent() {
        return client.getFileContent(README_PATH);
    }

    public void verifyDockerComposeContainsImage(String dockerComposePath, String image) {
        var dockerComposeContent = client.getFileContent(dockerComposePath);
        assertThat(dockerComposeContent).contains(image);
    }

    public void verifyDockerComposeDoesNotContainImage(String dockerComposePath, String image) {
        var dockerComposeContent = client.getFileContent(dockerComposePath);
        assertThat(dockerComposeContent).doesNotContain(image);
    }

    public void verifyWorkflowPasses(String workflowFileName) {
        var workflowRun = waitUntilCompleted(workflowFileName);

        assertThat(workflowRun.getConclusion()).isEqualTo("success");
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
        assertSuccess(result);

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
