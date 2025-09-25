package com.optivem.atddaccelerator.templategenerator.systemtest;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.TemplateGeneratorClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.io.IOException;

import static org.assertj.core.api.Assertions.assertThat;

class SetupScriptTest {


    private static final String REPO_OWNER = "valentinajemuovic";

    private TemplateGeneratorClient templateGeneratorClient = new TemplateGeneratorClient();
    private GithubClient githubClient;
    private String repoName;

    @BeforeEach
    void setup() {
        repoName = newName();
        githubClient = new GithubClient(REPO_OWNER, repoName);
    }

    @AfterEach
    void teardown() {
        if (githubClient != null) {
            githubClient.deleteRepository();
        }
    }

    @Test
    void githubRepository_fullTest() throws IOException, InterruptedException {
        templateGeneratorClient.generateNewRepository(repoName, Language.JAVA);
        githubClient.verifyRepositoryExists();
        Thread.sleep(5000);
        githubClient.verifyFolderExists("monolith-java");
        githubClient.verifyFolderDoesNotExist("monolith-dotnet");
        githubClient.verifyFolderDoesNotExist("monolith-typescript");
    }

    public static String newName() {
        var repoName = "repo-" + System.currentTimeMillis();
        return repoName;
    }


}