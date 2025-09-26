package com.optivem.atddaccelerator.templategenerator.systemtest;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GeneratorClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.GeneratorDsl;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.GitHubDsl;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Badges;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.RepositoryPaths;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;

import static org.assertj.core.api.Assertions.assertThat;

class ScriptTest {


    private static final String REPO_OWNER = "valentinajemuovic";

    private GeneratorDsl generator;
    private GitHubDsl gitHub;
    private String repoName;

    @BeforeEach
    void setup() {
        repoName = newName();
        generator = new GeneratorDsl(new GeneratorClient());
        gitHub = new GitHubDsl(new GithubClient(REPO_OWNER, repoName));
    }

    @AfterEach
    void teardown() {
        var created = generator.isCreated(repoName);

        if(created && gitHub != null) {
            gitHub.deleteRepository();
        }
    }

    @Test
    void shouldCreateJavaRepositoryShort() {

        // System

        generator.generateNewRepository(repoName, Language.JAVA);

        gitHub.verifyRepositoryExists();

        gitHub.verifyPathExists(RepositoryPaths.MONOLITH_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_TYPESCRIPT);

        gitHub.verifyPathExists(RepositoryPaths.COMMIT_STAGE_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);

        var badgeSvg = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-java.yml/badge.svg", REPO_OWNER, repoName);
        var badgeWorkflow = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-java.yml", REPO_OWNER, repoName);
        gitHub.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA, badgeSvg, badgeWorkflow);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);

        // System Test

        gitHub.verifyPathExists(RepositoryPaths.SYSTEM_TEST_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.SYSTEM_TEST_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.SYSTEM_TEST_TYPESCRIPT);

        // var javaImageName = String.format("ghcr.io/%s/%s/monolith-java:latest", REPO_OWNER, repoName);
        // var dotnetImageName = String.format("ghcr.io/%s/%s/monolith-dotnet:latest", REPO_OWNER, repoName);
        // var typescriptImageName = String.format("ghcr.io/%s/%s/monolith-typescript:latest", REPO_OWNER, repoName);

        // var dockerCompose = "system-test-java/docker-compose.yml";

        // githubClient.verifyDockerComposeContainsImage(dockerCompose, javaImageName);
        // githubClient.verifyDockerComposeDoesNotContainImage(dockerCompose, dotnetImageName);
        // githubClient.verifyDockerComposeDoesNotContainImage(dockerCompose, typescriptImageName);

    }

    @Test
    void shouldCreateJavaRepositoryFull() {
        generator.generateNewRepository(repoName, Language.JAVA);

        gitHub.verifyRepositoryExists();

        gitHub.verifyPathExists(RepositoryPaths.MONOLITH_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_TYPESCRIPT);

        gitHub.verifyPathExists(RepositoryPaths.COMMIT_STAGE_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);

        var badgeSvg = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-java.yml/badge.svg", REPO_OWNER, repoName);
        var badgeWorkflow = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-java.yml", REPO_OWNER, repoName);
        gitHub.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA, badgeSvg, badgeWorkflow);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);

        // Slow running
        gitHub.verifyWorkflowPasses(Badges.COMMIT_STAGE_MONOLITH_JAVA);

        // TODO: Verify only one package remains, that rest are deleted

    }

    @Disabled
    @Test
    void shouldCreateDotNetRepositoryFull() {
        generator.generateNewRepository(repoName, Language.DOTNET);

        gitHub.verifyRepositoryExists();

        gitHub.verifyPathExists(RepositoryPaths.MONOLITH_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_TYPESCRIPT);

        gitHub.verifyPathExists(RepositoryPaths.COMMIT_STAGE_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);

        var badgeSvg = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-dotnet.yml/badge.svg", REPO_OWNER, repoName);
        var badgeWorkflow = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-dotnet.yml", REPO_OWNER, repoName);
        gitHub.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET, badgeSvg, badgeWorkflow);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);

        gitHub.verifyWorkflowPasses(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
    }
    
    @Disabled
    @Test
    void shouldCreateTypeScriptRepositoryFull() {
        generator.generateNewRepository(repoName, Language.TYPESCRIPT);

        gitHub.verifyRepositoryExists();

        gitHub.verifyPathExists(RepositoryPaths.MONOLITH_TYPESCRIPT);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_JAVA);

        gitHub.verifyPathExists(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_JAVA);

        var badgeSvg = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-typescript.yml/badge.svg", REPO_OWNER, repoName);
        var badgeWorkflow = String.format("https://github.com/%s/%s/actions/workflows/commit-stage-monolith-typescript.yml", REPO_OWNER, repoName);
        gitHub.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT, badgeSvg, badgeWorkflow);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA);

        gitHub.verifyWorkflowPasses(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);
    }

    @Test
    void shouldReturnErrorForInvalidLanguage() {
        generator.generateNewRepositoryExpectError(repoName, "invalidLang");
    }



    private static String newName() {
        var repoName = "repo-" + System.currentTimeMillis();
        return repoName;
    }


}