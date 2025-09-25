package com.optivem.atddaccelerator.templategenerator.systemtest;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.TemplateGeneratorClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Badges;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.RepositoryPaths;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.assertj.core.api.Assertions.assertThat;

class ScriptTest {


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
        var created = templateGeneratorClient.isCreated(repoName);

        if(created && githubClient != null) {
            githubClient.deleteRepository();
        }
    }

    @Test
    void shouldCreateJavaRepository() {
        templateGeneratorClient.generateNewRepository(repoName, Language.JAVA);

        githubClient.verifyRepositoryExists();

        githubClient.verifyPathExists(RepositoryPaths.MONOLITH_JAVA);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_DOTNET);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_TYPESCRIPT);

        githubClient.verifyPathExists(RepositoryPaths.COMMIT_STAGE_JAVA);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_DOTNET);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);

        var badgeSvg = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-java.yml/badge.svg", REPO_OWNER, repoName);
        var badgeWorkflow = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-java.yml", REPO_OWNER, repoName);
        githubClient.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA, badgeSvg, badgeWorkflow);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);

    }

    @Test
    void shouldCreateDotNetRepository() {
        templateGeneratorClient.generateNewRepository(repoName, Language.DOTNET);

        githubClient.verifyRepositoryExists();

        githubClient.verifyPathExists(RepositoryPaths.MONOLITH_DOTNET);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_JAVA);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_TYPESCRIPT);

        githubClient.verifyPathExists(RepositoryPaths.COMMIT_STAGE_DOTNET);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_JAVA);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);

        var badgeSvg = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-dotnet.yml/badge.svg", REPO_OWNER, repoName);
        var badgeWorkflow = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-dotnet.yml", REPO_OWNER, repoName);
        githubClient.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET, badgeSvg, badgeWorkflow);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);
    }
    
    @Test
    void shouldCreateTypeScriptRepository() {
        templateGeneratorClient.generateNewRepository(repoName, Language.TYPESCRIPT);

        githubClient.verifyRepositoryExists();

        githubClient.verifyPathExists(RepositoryPaths.MONOLITH_TYPESCRIPT);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_DOTNET);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_JAVA);

        githubClient.verifyPathExists(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_DOTNET);
        githubClient.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_JAVA);

        var badgeSvg = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-typescript.yml/badge.svg", REPO_OWNER, repoName);
        var badgeWorkflow = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-typescript.yml", REPO_OWNER, repoName);
        githubClient.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT, badgeSvg, badgeWorkflow);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA);
    }

    @Test
    void shouldReturnErrorForInvalidLanguage() {
        templateGeneratorClient.generateNewRepositoryExpectError(repoName, "invalidLang");
    }



    private static String newName() {
        var repoName = "repo-" + System.currentTimeMillis();
        return repoName;
    }


}