package com.optivem.atddaccelerator.templategenerator.systemtest.util;

public class RepositoryPaths {
    public static final String MONOLITH_JAVA = "monolith-java";
    public static final String MONOLITH_DOTNET = "monolith-dotnet";
    public static final String MONOLITH_TYPESCRIPT = "monolith-typescript";

    public static final String COMMIT_STAGE_JAVA = ".github/workflows/commit-stage-monolith-java.yml";
    public static final String COMMIT_STAGE_DOTNET = ".github/workflows/commit-stage-monolith-dotnet.yml";
    public static final String COMMIT_STAGE_TYPESCRIPT = ".github/workflows/commit-stage-monolith-typescript.yml";

    public static final String SYSTEM_TEST_JAVA = "system-test-java";
    public static final String SYSTEM_TEST_DOTNET = "system-test-dotnet";
    public static final String SYSTEM_TEST_TYPESCRIPT = "system-test-typescript";

    public static final String SYSTEM_TEST_JAVA_DOCKER_COMPOSE = "system-test-java/docker-compose.yml";
    public static final String SYSTEM_TEST_DOTNET_DOCKER_COMPOSE = "system-test-dotnet/docker-compose.yml";
    public static final String SYSTEM_TEST_TYPESCRIPT_DOCKER_COMPOSE = "system-test-typescript/docker-compose.yml";


    public static final String LOCAL_ACCEPTANCE_STAGE_TEST_JAVA = ".github/workflows/local-acceptance-stage-test-java.yml";
    public static final String LOCAL_ACCEPTANCE_STAGE_TEST_DOTNET = ".github/workflows/local-acceptance-stage-test-dotnet.yml";
    public static final String LOCAL_ACCEPTANCE_STAGE_TEST_TYPESCRIPT = ".github/workflows/local-acceptance-stage-test-typescript.yml";
}
