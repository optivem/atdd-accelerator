package com.optivem.atddaccelerator.templategenerator.systemtest.dsl;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers.ReadmeClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Constants;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.WorkflowRunResult;
import dev.failsafe.Failsafe;
import dev.failsafe.RetryPolicy;

import java.time.Duration;
import java.util.List;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertFailure;
import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;
import static org.assertj.core.api.Assertions.assertThat;

public class GitHubDsl {

    private final GithubClient client;
    private final String repositoryPath;

    private final ReadmeClient readmeClient;

    public GitHubDsl(GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
        this.readmeClient = new ReadmeClient(client);
    }

    public void verifyRepositoryExists() {
        var result = client.viewRepository();
        assertSuccess(result, "Repository '" + client.getRepositoryPath() + "' should exist.");
    }

    private void verifyPathExists(String path) {
        var result = client.viewPath(path);
        assertSuccess(result, "Path '" + path + "' should exist.");
    }

    private void verifyPathDoesNotExist(String path) {
        var result = client.viewPath(path);
        assertFailure(result, "Path '" + path + "' should not exist.");
    }

    public void verifyPathLanguageExists(String pathFormat, String language) {
        for(String l : Language.ALL) {
            var path = String.format(pathFormat, l);
            if(l.equals(language)) {
                verifyPathExists(path);
            } else {
                verifyPathDoesNotExist(path);
            }
        }
    }


    public void deleteRepository() {
        var result = client.deleteRepository();
        assertSuccess(result, "Failed to delete repository '" + client.getRepositoryPath() + "'.");
    }



    public void verifyDockerComposeContainsImage(String dockerComposePath, String image) {
        var dockerComposeContent = client.getFileContent(dockerComposePath);
        assertThat(dockerComposeContent)
                .as("Docker Compose should contain image: " + image)
                .contains(image);
    }

    public void verifyDockerComposeDoesNotContainImage(String dockerComposePath, String image) {
        var dockerComposeContent = client.getFileContent(dockerComposePath);
        assertThat(dockerComposeContent)
                .as("Docker Compose should not contain image: " + image)
                .doesNotContain(image);
    }

    private void verifyWorkflowPasses(String workflowFileName) {
        var workflowRun = waitUntilCompleted(workflowFileName);

        assertThat(workflowRun.getConclusion())
                .as("Workflow '" + workflowFileName + "' should pass, but concluded with: " + workflowRun.getConclusion())
                .isEqualTo("success");
    }

    private void verifyWorkflowPasses(String workflowFileNameFormat, String language) {
        var workflowFileName = String.format(workflowFileNameFormat, language);
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

    public void verifyPagesEnabled() {
        var result = client.viewPages();
        assertSuccess(result, "GitHub Pages should be enabled.");
        verifyPagesSourceIsMainDocs();
    }

    private void verifyPagesSourceIsMainDocs() {
        var result = client.viewPages();
        assertSuccess(result, "Failed to get GitHub Pages info.");

        var json = result.getOutput();
        ObjectMapper mapper = new ObjectMapper();
        try {
            var root = mapper.readTree(json);
            var source = root.path("source");
            String branch = source.path("branch").asText();
            String path = source.path("path").asText();

            assertThat(branch)
                    .as("GitHub Pages source branch should be 'main'")
                    .isEqualTo("main");
            assertThat(path)
                    .as("GitHub Pages source path should be '/docs'")
                    .isEqualTo("/docs");
        } catch (JsonProcessingException e) {
            throw new RuntimeException("Failed to parse GitHub Pages JSON", e);
        }
    }



    public void verifyPathsExist(String systemLanguage, String systemTestLanguage) {
        verifyPathLanguageExists("monolith-%s", systemLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.COMMIT_STAGE_MONOLITH_FORMAT), systemLanguage);

        verifyPathLanguageExists("system-test-%s/docker-compose.yml", systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.LOCAL_ACCEPTANCE_STAGE_TEST_FORMAT), systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.ACCEPTANCE_STAGE_TEST_FORMAT), systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.QA_STAGE_TEST_FORMAT), systemTestLanguage);
        verifyPathLanguageExists( getWorkflowPath(Constants.PROD_STAGE_TEST_FORMAT), systemTestLanguage);
    }

    private String getWorkflowPath(String stageFormat) {
        return ".github/workflows/" + stageFormat + ".yml";
    }

    public void verifyWorkflowsPass(String systemLanguage, String systemTestLanguage) {
        verifyWorkflowPasses(Constants.PAGES_BUILD_DEPLOYMENT);
        verifyWorkflowPasses(Constants.COMMIT_STAGE_MONOLITH_FORMAT, systemLanguage);
        verifyWorkflowPasses(Constants.LOCAL_ACCEPTANCE_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyWorkflowPasses(Constants.ACCEPTANCE_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyWorkflowPasses(Constants.QA_STAGE_TEST_FORMAT, systemTestLanguage);
        verifyWorkflowPasses(Constants.PROD_STAGE_TEST_FORMAT, systemTestLanguage);
    }

    public void verifyDockerComposeImage(String systemLanguage, String systemTestLanguage) {
        var dockerComposePath = String.format("system-test-%s/docker-compose.yml", systemTestLanguage);

        for(String l : Language.ALL) {
            var monolithDockerImageName = String.format(Constants.MONOLITH_DOCKER_IMAGE_NAME_FORMAT, repositoryPath, l);
            if(l.equals(systemLanguage)) {
                verifyDockerComposeContainsImage(dockerComposePath, monolithDockerImageName);
            } else {
                verifyDockerComposeDoesNotContainImage(dockerComposePath, monolithDockerImageName);
            }
        }
    }

    public void verifyReadmeHasBadges(String systemLanguage, String systemTestLanguage) {
        readmeClient.verifyReadmeHasBadges(systemLanguage, systemTestLanguage);
    }
}
