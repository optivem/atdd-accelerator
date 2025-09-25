package com.optivem.atddaccelerator.templategenerator.systemtest;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.TemplateGeneratorClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Folders;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Disabled;
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
    void githubRepositoryJava() {
        templateGeneratorClient.generateNewRepository(repoName, Language.JAVA);

        githubClient.verifyRepositoryExists();

        githubClient.verifyPathExists(Folders.MONOLITH_JAVA);
        githubClient.verifyPathDoesNotExist(Folders.MONOLITH_DOTNET);
        githubClient.verifyPathDoesNotExist(Folders.MONOLITH_TYPESCRIPT);

        githubClient.verifyPathExists(".github/workflows/commit-stage-monolith-java.yml");
        githubClient.verifyPathDoesNotExist(".github/workflows/commit-stage-monolith-dotnet.yml");
        githubClient.verifyPathDoesNotExist(".github/workflows/commit-stage-monolith-typescript.yml");
    }

    @Test
    void githubRepositoryDotNet() {
        templateGeneratorClient.generateNewRepository(repoName, Language.DOTNET);

        githubClient.verifyRepositoryExists();

        githubClient.verifyPathExists(Folders.MONOLITH_DOTNET);
        githubClient.verifyPathDoesNotExist(Folders.MONOLITH_JAVA);
        githubClient.verifyPathDoesNotExist(Folders.MONOLITH_TYPESCRIPT);
    }
    
    @Test
    void githubRepositoryTypeScript() {
        templateGeneratorClient.generateNewRepository(repoName, Language.TYPESCRIPT);

        githubClient.verifyRepositoryExists();
        
        githubClient.verifyPathExists(Folders.MONOLITH_TYPESCRIPT);
        githubClient.verifyPathDoesNotExist(Folders.MONOLITH_JAVA);
        githubClient.verifyPathDoesNotExist(Folders.MONOLITH_DOTNET);
    }



    private static String newName() {
        var repoName = "repo-" + System.currentTimeMillis();
        return repoName;
    }


}