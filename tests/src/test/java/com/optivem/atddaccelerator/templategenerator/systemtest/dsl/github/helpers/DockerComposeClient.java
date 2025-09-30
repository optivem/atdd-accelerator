package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Constants;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;

import static org.assertj.core.api.Assertions.assertThat;

public class DockerComposeClient {
    private final GithubClient client;
    private final String repositoryPath;

    public DockerComposeClient(GithubClient client) {
        this.client = client;
        this.repositoryPath = client.getRepositoryPath();
    }

    public void verifyDockerComposeImage(Language systemLanguage, Language systemTestLanguage) {
        var dockerComposePath = String.format("system-test-%s/docker-compose.yml", systemTestLanguage.getValue());

        for(var l : Language.getAll()) {
            var monolithDockerImageName = String.format(Constants.MONOLITH_DOCKER_IMAGE_NAME_FORMAT, repositoryPath, l.getValue());
            if(l.equals(systemLanguage)) {
                verifyDockerComposeContainsImage(dockerComposePath, monolithDockerImageName);
            } else {
                verifyDockerComposeDoesNotContainImage(dockerComposePath, monolithDockerImageName);
            }
        }
    }

    private void verifyDockerComposeContainsImage(String dockerComposePath, String image) {
        var dockerComposeContent = client.getFileContent(dockerComposePath);
        assertThat(dockerComposeContent)
                .as("Docker Compose should contain image: " + image)
                .contains(image);
    }

    private void verifyDockerComposeDoesNotContainImage(String dockerComposePath, String image) {
        var dockerComposeContent = client.getFileContent(dockerComposePath);
        assertThat(dockerComposeContent)
                .as("Docker Compose should not contain image: " + image)
                .doesNotContain(image);
    }
}
