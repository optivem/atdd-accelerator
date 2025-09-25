<#
.SYNOPSIS
Acceptance Test for Repository Creation

.DESCRIPTION
Tests that a repository can be created from the ATDD Accelerator template
given a repository name.

.NOTES
Run with: Invoke-Pester tests/Create-Repository.Tests.ps1
#>

Describe "Create Repository from Template" {
    BeforeAll {
        # Test configuration
        $script:TestRepositoryName = "atdd-test-$(Get-Random -Minimum 1000 -Maximum 9999)"
        $script:GitHubUsername = (gh auth status 2>&1 | Select-String "Logged in to github.com account (.+) " | ForEach-Object { $_.Matches[0].Groups[1].Value })
        if (-not $script:GitHubUsername) {
            $script:GitHubUsername = (gh auth status 2>&1 | Select-String "Logged in to github.com as (.+)" | ForEach-Object { $_.Matches[0].Groups[1].Value })
        }
        $script:CreatedRepository = "$script:GitHubUsername/$script:TestRepositoryName"
    }

    AfterAll {
        # Cleanup - delete test repository
        try {
            gh repo delete $script:CreatedRepository --yes 2>$null
        } catch {
            Write-Host "Test repository cleanup completed" -ForegroundColor Gray
        }
    }

    It "Given I provide a repository name, a new repository has been created" {
        # Given: I provide a repository name
        $repositoryName = $script:TestRepositoryName
        
        # # When: I create a repository from the template
        # gh repo create $repositoryName --template "optivem/atdd-accelerator-template-mono-repo" --public --yes
        Write-Output hello
        
        # Then: A new repository has been created
        $LASTEXITCODE | Should -Be 0
        
        # And: The repository exists and is accessible
        $repoInfo = gh repo view $script:CreatedRepository --json name,visibility 2>$null
        $LASTEXITCODE | Should -Be 0
        
        $repo = $repoInfo | ConvertFrom-Json
        $repo.name | Should -Be $repositoryName
        $repo.visibility | Should -Be "PUBLIC"
    }
}