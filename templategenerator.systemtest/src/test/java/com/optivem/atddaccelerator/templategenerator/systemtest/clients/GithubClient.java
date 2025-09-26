package com.optivem.atddaccelerator.templategenerator.systemtest.clients;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResult;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.WorkflowRunResult;
import dev.failsafe.Failsafe;
import dev.failsafe.RetryPolicy;

import java.time.Duration;
import java.util.List;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessExecutor.*;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertFailure;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;
import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class GithubClient {

    private String repositoryPath;

    public GithubClient(String owner, String repoName) {
        this.repositoryPath = owner + "/" + repoName;
    }

    public static GithubClient createRandom(String owner) {
        var repoName = "repo-" + System.currentTimeMillis();
        return new GithubClient(owner, repoName);
    }

    public String getRepositoryPath() {
        return repositoryPath;
    }

    public void verifyRepositoryExists() {
        var result = viewRepository();
        assertSuccess(result);
    }

    public void verifyPathExists(String path) {
        var result = viewPath(path);
        assertSuccess(result);
    }

    public void verifyPathDoesNotExist(String path) {
        var result = viewPath(path);
        assertFailure(result);
    }

    public void deleteRepository() {
        var result = executeProcess("gh", "repo", "delete", repositoryPath, "--yes");
        assertSuccess(result);
    }

    private ProcessResult viewRepository() {
        return executeProcess("gh", "repo", "view", repositoryPath);
    }

    private ProcessResult viewPath(String path) {
        return executeProcess("gh", "api", "/repos/" + repositoryPath + "/contents/" + path);
    }

    public void verifyReadmeContainsBadge(String badge) {
        var readmeContent = getReadmeContent();
        assertTrue(readmeContent.contains(badge));
    }

    public void verifyReadmeDoesNotContainBadge(String badge) {
        var readmeContent = getReadmeContent();
        assertThat(readmeContent).doesNotContain(badge);
    }

    private String getReadmeContent() {
        // TODO: VJ: Make consistent
        var result = executeProcess("gh", "api", "/repos/" + repositoryPath + "/readme", "--jq", ".content");
        assertSuccess(result);

        var content = result.getOutput().replaceAll("\\s+", "");
        var decodedBytes = java.util.Base64.getDecoder().decode(content);
        return new String(decodedBytes);
    }

    public void verifyReadmeContainsBadge(String badge, String badgeSvg, String badgeWorkflow) {
        var readmeContent = getReadmeContent();
        assertTrue(readmeContent.contains(badge));
        assertTrue(readmeContent.contains(badgeSvg));
        assertTrue(readmeContent.contains(badgeWorkflow));
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
        var result = executeProcess("gh", "run", "list",
                "--workflow", workflowFileName,
                "--limit", "1",
                "--json", "status,conclusion",
                "--repo", repositoryPath);
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

    public void verifyDockerComposeContainsImage(String dockerComposePath, String image) {
        // var result = executeProcessExpectSuccess("gh", "api", "/repos/" + repositoryPath + "/" + dockerComposePath, "--jq", ".content");
    }

    public void verifyDockerComposeDoesNotContainImage(String dockerComposePath, String image) {
    }
}
