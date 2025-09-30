package com.optivem.atddaccelerator.templategenerator.systemtest;

import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GithubClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.clients.GeneratorClient;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.GeneratorDsl;
import com.optivem.atddaccelerator.templategenerator.systemtest.dsl.GitHubDsl;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Constants;
import com.optivem.atddaccelerator.templategenerator.systemtest.util.Language;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.Arguments;
import org.junit.jupiter.params.provider.MethodSource;

import java.util.stream.Stream;

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

    static Stream<Arguments> languageProvider() {
        return Stream.of(
                Arguments.of(Language.DOTNET, Language.DOTNET),
                Arguments.of(Language.DOTNET, Language.JAVA),
                Arguments.of(Language.DOTNET, Language.TYPESCRIPT),

                Arguments.of(Language.JAVA, Language.DOTNET),
                Arguments.of(Language.JAVA, Language.JAVA),
                Arguments.of(Language.JAVA, Language.TYPESCRIPT),

                Arguments.of(Language.TYPESCRIPT, Language.DOTNET),
                Arguments.of(Language.TYPESCRIPT, Language.JAVA),
                Arguments.of(Language.TYPESCRIPT, Language.TYPESCRIPT)
        );
    }

    @Disabled
    @Test
    void shouldCreateRepositoryWithJava() {
        // Act
        generator.generateNewRepository(repoName, Language.JAVA, Language.TYPESCRIPT);

        // Assert
        gitHub.verifyRepositoryExists();
        gitHub.verifyPathsExist(Language.JAVA, Language.TYPESCRIPT);
        gitHub.verifyDockerComposeImage(Language.JAVA, Language.TYPESCRIPT);
        gitHub.verifyReadmeHasBadges(Language.JAVA, Language.TYPESCRIPT);
        gitHub.verifyPagesEnabled();

        gitHub.verifyWorkflowsPass(Language.JAVA, Language.TYPESCRIPT);
    }

    @ParameterizedTest
    @MethodSource("languageProvider")
    void shouldCreateRepositoryWithLanguages(Language systemLanguage, Language systemTestLanguage) {
        generator.generateNewRepository(repoName, systemLanguage, systemTestLanguage);

        gitHub.verifyRepositoryExists();
        gitHub.verifyPathsExist(systemLanguage, systemTestLanguage);
        gitHub.verifyDockerComposeImage(systemLanguage, systemTestLanguage);
        gitHub.verifyReadmeHasBadges(systemLanguage, systemTestLanguage);
        gitHub.verifyPagesEnabled();

        gitHub.verifyWorkflowsPass(systemLanguage, systemTestLanguage);
    }

    @Test
    void shouldReturnErrorForInvalidLanguage() {
        generator.generateNewRepositoryExpectError(repoName, Language.NONE, Language.TYPESCRIPT);
    }

    private static String newName() {
        var repoName = "repo-" + System.currentTimeMillis();
        return repoName;
    }


}