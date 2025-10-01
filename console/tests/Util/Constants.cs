namespace Optivem.AtddAccelerator.TemplateGenerator.SystemTests.Util
{
    public static class Constants
    {
        // Stages
        public const string PagesBuilderDeployment = "pages-build-deployment";
        public const string CommitStageMonolithFormat = "commit-stage-monolith-{0}";
        public const string LocalAcceptanceStageTestFormat = "local-acceptance-stage-test-{0}";
        public const string AcceptanceStageTestFormat = "acceptance-stage-test-{0}";
        public const string QaStageTestFormat = "qa-stage-test-{0}";
        public const string ProdStageTestFormat = "prod-stage-test-{0}";

        // Workflows
        public const string PagesBuilderDeploymentWorkflowFormat = "https://github.com/{0}/actions/workflows/pages/pages-build-deployment";
        public const string PagesBuilderDeploymentWorkflowImageFormat = "https://github.com/{0}/actions/workflows/pages/pages-build-deployment/badge.svg";
        public const string StageWorkflowFormat = "https://github.com/{0}/actions/workflows/{1}.yml";
        public const string StageWorkflowImageFormat = "https://github.com/{0}/actions/workflows/{1}.yml/badge.svg";

        // Docker Images
        public const string MonolithDockerImageNameFormat = "ghcr.io/{0}/monolith-{1}:latest";
    }
}