package com.optivem.atddaccelerator.templategenerator.systemtest.dsl.github.helpers;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;

import static com.optivem.atddaccelerator.templategenerator.systemtest.util.ProcessResultAssertions.assertSuccess;
import static org.assertj.core.api.Assertions.assertThat;

public class PagesClient {
    private final GithubClient client;

    public PagesClient(GithubClient client) {
        this.client = client;
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
}
