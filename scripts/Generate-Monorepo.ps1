param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$SystemLanguage,

    [Parameter(Mandatory=$false)]
    [string]$SystemTestLanguage
)

# Import helper modules
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
. "$scriptDir\remove-unused-language-folders.ps1"
. "$scriptDir\update-readme-badges.ps1"
. "$scriptDir\setup-github-pages.ps1"
. "$scriptDir\update-docker-compose.ps1"
. "$scriptDir\invoke-system-test-release-workflows.ps1"
. "$scriptDir\invoke-build-workflows.ps1"

function Test-SystemLanguage {
    param([string]$SystemLanguage)
    
    $validLanguages = @("java", "dotnet", "typescript")
    
    if ([string]::IsNullOrWhiteSpace($SystemLanguage)) {
        throw "SystemLanguage parameter is required. Valid options: $($validLanguages -join ', ')"
    }
    
    if ($SystemLanguage.ToLower() -notin $validLanguages) {
        throw "Invalid SystemLanguage: '$SystemLanguage'. Valid options: $($validLanguages -join ', ')"
    }
    
    Write-Output "SystemLanguage '$SystemLanguage' is valid"
}

function Get-GitHubUsername {
    param([string]$ProvidedUsername)
    
    if ($ProvidedUsername) {
        return $ProvidedUsername
    }
    
    $authStatus = gh auth status 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "GitHub CLI not authenticated. Please run: gh auth login"
    }
    
    $username = ($authStatus | Select-String "Logged in to github\.com account (.+?) " | ForEach-Object { $_.Matches[0].Groups[1].Value })
    if (-not $username) {
        $username = ($authStatus | Select-String "Logged in to github\.com as (.+)" | ForEach-Object { $_.Matches[0].Groups[1].Value })
    }
    
    return $username
}

function New-RepositoryFromTemplate {
    param(
        [string]$RepositoryName,
        [string]$TemplateName = "optivem/atdd-accelerator-template-mono-repo"
    )
    
    Write-Output "Creating repository from template..."
    gh repo create $RepositoryName --template $TemplateName --public --clone
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create repository"
    }
}

function Push-RepositoryChanges {
    Write-Output "Pushing changes to remote repository..."
    git push origin main
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push changes"
    }
}

function Remove-TempFolder {
    Write-Output "Cleaning up temp folder..."
    
    if (Test-Path "temp") {
        try {
            Remove-Item -Recurse -Force "temp"
            Write-Output "✅ Temp folder removed successfully"
        } catch {
            Write-Warning "Could not remove temp folder: $_"
        }
    } else {
        Write-Output "ℹ️  Temp folder not found (already clean)"
    }
}

function Remove-LocalRepository {
    param([string]$RepositoryName)
    
    Write-Output "Cleaning up local repository clone..."
    Set-Location ..
    Remove-Item -Recurse -Force $RepositoryName
    Write-Output "Local repository cleanup completed"
}

function Invoke-ErrorCleanup {
    param([string]$RepositoryName)
    
    try {
        Set-Location ..
        if (Test-Path $RepositoryName) {
            Remove-Item -Recurse -Force $RepositoryName
            Write-Output "Cleaned up local repository after error"
        }
    } catch {
        Write-Warning "Could not clean up local repository: $_"
    }
}

# Main execution
try {
    Write-Output "ATDD Accelerator Setup Script"
    Write-Output "Repository Name: $RepositoryName"
    Write-Output "System Language: $SystemLanguage"
    Write-Output "System Test Language: $SystemTestLanguage"
    
    Test-SystemLanguage -SystemLanguage $SystemLanguage
    Test-SystemLanguage -SystemLanguage $SystemTestLanguage

    $GitHubUsername = Get-GitHubUsername -ProvidedUsername $GitHubUsername
    Write-Output "GitHub Username: $GitHubUsername"
    
    New-RepositoryFromTemplate -RepositoryName $RepositoryName
    
    # Change to the repository directory
    Set-Location $RepositoryName
    
    # First, complete all repository setup and content changes
    $hasChanges = Remove-UnusedLanguageFolders -SystemLanguage $SystemLanguage -SystemTestLanguage $SystemTestLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
    
    # Clean up temp folder before pushing
    Remove-TempFolder
    
    # Push all changes to remote
    if ($hasChanges) {
        Push-RepositoryChanges
    } else {
        Write-Output "No changes to push"
    }
    
    # Wait a moment for the push to complete
    Start-Sleep -Seconds 3
    
    # Trigger BUILD workflows first to create Docker images
    Write-Output ""
    
    # Wait for build workflows to complete (this may take several minutes)
    $buildCompleted = Wait-ForBuildWorkflows -SystemLanguage $SystemLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
    
    if ($buildCompleted) {
        Write-Output "✅ Build workflows completed - Docker images are now available"
    } else {
        Write-Warning "⚠️ Build workflows may still be running - test workflows might fail initially"
    }
    
    # NOW enable GitHub Pages (after all content is ready)
    Write-Output ""
    Write-Output "Enabling GitHub Pages..."
    $pagesEnabled = Enable-GitHubPages -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
    
    if ($pagesEnabled) {
        Write-Output "✅ GitHub Pages enabled successfully"
    } else {
        Write-Warning "⚠️ GitHub Pages setup failed, but continuing..."
    }

    # Trigger system test workflows (after builds complete)
    Write-Output ""
    Write-Output "Triggering system test workflows..."
    $workflowsTriggered = Invoke-SystemTestWorkflows -SystemTestLanguage $SystemTestLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
    
    if ($workflowsTriggered) {
        Write-Output "✅ System test workflows have been triggered"
        Write-Output "   You can monitor their progress at: https://github.com/$GitHubUsername/$RepositoryName/actions"
    } else {
        Write-Warning "⚠️ No workflows were triggered - you may need to trigger them manually"
    }

    Write-Output ""
    Write-Output "Repository created successfully: $GitHubUsername/$RepositoryName"
    Write-Output "Setup completed successfully"
    
    Remove-LocalRepository -RepositoryName $RepositoryName
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $_"
    Invoke-ErrorCleanup -RepositoryName $RepositoryName
    exit 1
}