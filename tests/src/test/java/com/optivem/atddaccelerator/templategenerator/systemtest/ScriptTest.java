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

        generator.generateNewRepository(repoName, Language.JAVA, Language.TYPESCRIPT);

        gitHub.verifyRepositoryExists();

        gitHub.verifyPathExists(RepositoryPaths.MONOLITH_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_TYPESCRIPT);

        gitHub.verifyPathExists(RepositoryPaths.COMMIT_STAGE_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_DOTNET);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);



        // System Test

        gitHub.verifyPathExists(RepositoryPaths.SYSTEM_TEST_TYPESCRIPT);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.SYSTEM_TEST_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.SYSTEM_TEST_DOTNET);

        gitHub.verifyPathExists(RepositoryPaths.SYSTEM_TEST_TYPESCRIPT_DOCKER_COMPOSE);

        var javaImageName = String.format("ghcr.io/%s/%s/monolith-java:latest", REPO_OWNER, repoName);
        var dotnetImageName = String.format("ghcr.io/%s/%s/monolith-dotnet:latest", REPO_OWNER, repoName);
        var typescriptImageName = String.format("ghcr.io/%s/%s/monolith-typescript:latest", REPO_OWNER, repoName);

        gitHub.verifyDockerComposeContainsImage(RepositoryPaths.SYSTEM_TEST_TYPESCRIPT_DOCKER_COMPOSE, javaImageName);
        gitHub.verifyDockerComposeDoesNotContainImage(RepositoryPaths.SYSTEM_TEST_TYPESCRIPT_DOCKER_COMPOSE, dotnetImageName);
        gitHub.verifyDockerComposeDoesNotContainImage(RepositoryPaths.SYSTEM_TEST_TYPESCRIPT_DOCKER_COMPOSE, typescriptImageName);

        gitHub.verifyPathExists(RepositoryPaths.LOCAL_ACCEPTANCE_STAGE_TEST_TYPESCRIPT);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.LOCAL_ACCEPTANCE_STAGE_TEST_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.LOCAL_ACCEPTANCE_STAGE_TEST_DOTNET);

        gitHub.verifyPathExists(RepositoryPaths.ACCEPTANCE_STAGE_TEST_TYPESCRIPT);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.ACCEPTANCE_STAGE_TEST_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.ACCEPTANCE_STAGE_TEST_DOTNET);

        gitHub.verifyPathExists(RepositoryPaths.QA_STAGE_TEST_TYPESCRIPT);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.QA_STAGE_TEST_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.QA_STAGE_TEST_DOTNET);

        gitHub.verifyPathExists(RepositoryPaths.PRODUCTION_STAGE_TEST_TYPESCRIPT);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.PRODUCTION_STAGE_TEST_JAVA);
        gitHub.verifyPathDoesNotExist(RepositoryPaths.PRODUCTION_STAGE_TEST_DOTNET);


        // Documentation

        gitHub.verifyPagesEnabled();
        gitHub.verifyPagesSourceIsMainDocs();

        // Verify readme has badges
        gitHub.verifyReadmeHasBadges(Language.JAVA, Language.TYPESCRIPT);

        // Verify Pipeline passes

        gitHub.verifyWorkflowPasses(Badges.PAGES_BUILD_DEPLOYMENT);
        gitHub.verifyWorkflowPasses(Badges.COMMIT_STAGE_MONOLITH_JAVA);
        gitHub.verifyWorkflowPasses(Badges.LOCAL_ACCEPTANCE_STAGE_TEST_TYPESCRIPT);
        gitHub.verifyWorkflowPasses(Badges.ACCEPTANCE_STAGE_TEST_TYPESCRIPT);
        gitHub.verifyWorkflowPasses(Badges.QA_STAGE_TEST_TYPESCRIPT);
        gitHub.verifyWorkflowPasses(Badges.PROD_STAGE_TEST_TYPESCRIPT);
    }

    @Test
    void shouldReturnErrorForInvalidLanguage() {
        generator.generateNewRepositoryExpectError(repoName, "invalidLang", Language.TYPESCRIPT);
    }



//    @Disabled
//    @Test
//    void shouldCreateDotNetRepositoryFull() {
//        generator.generateNewRepository(repoName, Language.DOTNET, Language.TYPESCRIPT);
//
//        gitHub.verifyRepositoryExists();
//
//        gitHub.verifyPathExists(RepositoryPaths.MONOLITH_DOTNET);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_JAVA);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_TYPESCRIPT);
//
//        gitHub.verifyPathExists(RepositoryPaths.COMMIT_STAGE_DOTNET);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_JAVA);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);
//
//        gitHub.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
//        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA);
//        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);
//
//        gitHub.verifyWorkflowPasses(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
//    }
//
//    @Disabled
//    @Test
//    void shouldCreateTypeScriptRepositoryFull() {
//        generator.generateNewRepository(repoName, Language.TYPESCRIPT, Language.TYPESCRIPT);
//
//        gitHub.verifyRepositoryExists();
//
//        gitHub.verifyPathExists(RepositoryPaths.MONOLITH_TYPESCRIPT);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_DOTNET);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.MONOLITH_JAVA);
//
//        gitHub.verifyPathExists(RepositoryPaths.COMMIT_STAGE_TYPESCRIPT);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_DOTNET);
//        gitHub.verifyPathDoesNotExist(RepositoryPaths.COMMIT_STAGE_JAVA);
//
//        gitHub.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);
//        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
//        gitHub.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA);
//
//        gitHub.verifyWorkflowPasses(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);
//    }




    private static String newName() {
        var repoName = "repo-" + System.currentTimeMillis();
        return repoName;
    }


}