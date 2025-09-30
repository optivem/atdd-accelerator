package com.optivem.atddaccelerator.templategenerator.systemtest.dsl;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers.FileClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers.ReadmeClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers.WorkflowClient;
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
    private final FileClient fileClient;
    private final WorkflowClient workflowClient;

    public GitHubDsl(GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
        this.readmeClient = new ReadmeClient(client);
        this.fileClient = new FileClient(client);
        this.workflowClient = new WorkflowClient(client);
    }

    public void verifyRepositoryExists() {
        var result = client.viewRepository();
        assertSuccess(result, "Repository '" + client.getRepositoryPath() + "' should exist.");
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

    public void verifyPathsExist(String systemLanguage, String systemTestLanguage) {
        fileClient.verifyPathsExist(systemLanguage, systemTestLanguage);
    }

    public void verifyWorkflowsPass(String systemLanguage, String systemTestLanguage) {
        workflowClient.verifyWorkflowsPass(systemLanguage, systemTestLanguage);
    }
}
