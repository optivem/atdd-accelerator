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

        githubClient.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_JAVA);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
        githubClient.verifyReadmeDoesNotContainBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);
    }

//5. In the `System` section, update the status badge by providing your repository path, i.e. we'll be replacing `optivem/atdd-accelerator-template-mono-repo` by your concrete repository link, e.g. `valentinajemuovic/eshop`
//   - BEFORE: `[![commit-stage-monolith-java](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-java.yml/badge.svg)](https://github.com/optivem/atdd-accelerator-template-mono-repo/actions/workflows/commit-stage-monolith-java.yml)`
//   - AFTER: `[![commit-stage-monolith-java](https://github.com/valentinajemuovic/eshop/actions/workflows/commit-stage-monolith-java.yml/badge.svg)](https://github.com/valentinajemuovic/eshop/actions/workflows/commit-stage-monolith-java.yml)`



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

        githubClient.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_DOTNET);
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

        githubClient.verifyReadmeContainsBadge(Badges.COMMIT_STAGE_MONOLITH_TYPESCRIPT);
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