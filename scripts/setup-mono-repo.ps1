param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$SystemLanguage
)

try {
    Write-Output "ATDD Accelerator Setup Script"
    Write-Output "Repository Name: $RepositoryName"
    Write-Output "System Language: $SystemLanguage"
    
    # Get GitHub username if not provided
    if (-not $GitHubUsername) {
        $authStatus = gh auth status 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "GitHub CLI not authenticated. Please run: gh auth login"
        }
        
        $GitHubUsername = ($authStatus | Select-String "Logged in to github\.com account (.+?) " | ForEach-Object { $_.Matches[0].Groups[1].Value })
        if (-not $GitHubUsername) {
            $GitHubUsername = ($authStatus | Select-String "Logged in to github\.com as (.+)" | ForEach-Object { $_.Matches[0].Groups[1].Value })
        }
    }
    
    Write-Output "GitHub Username: $GitHubUsername"
    
    # Create repository from template
    Write-Output "Creating repository from template..."
    gh repo create $RepositoryName --template "optivem/atdd-accelerator-template-mono-repo" --public --clone
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create repository"
    }
    
    # Change to the repository directory
    Set-Location $RepositoryName
    
    # Remove unused language folders based on SystemLanguage
    Write-Output "Removing unused language folders..."
    
    if ($SystemLanguage -ieq "dotnet") {
        # Keep monolith-dotnet, remove others
        if (Test-Path "monolith-java") {
            Remove-Item -Recurse -Force "monolith-java"
            git rm -r "monolith-java"
        }
        if (Test-Path "monolith-typescript") {
            Remove-Item -Recurse -Force "monolith-typescript"
            git rm -r "monolith-typescript"
        }
        git commit -m "Remove unused language folders: monolith-java, monolith-typescript"
    } elseif ($SystemLanguage -ieq "typescript") {
        # Keep monolith-typescript, remove others
        if (Test-Path "monolith-java") {
            Remove-Item -Recurse -Force "monolith-java"
            git rm -r "monolith-java"
        }
        if (Test-Path "monolith-dotnet") {
            Remove-Item -Recurse -Force "monolith-dotnet"
            git rm -r "monolith-dotnet"
        }
        git commit -m "Remove unused language folders: monolith-java, monolith-dotnet"
    } else {
        # Default to Java - remove monolith-dotnet and monolith-typescript
        if (Test-Path "monolith-dotnet") {
            Remove-Item -Recurse -Force "monolith-dotnet"
            git rm -r "monolith-dotnet"
        }
        if (Test-Path "monolith-typescript") {
            Remove-Item -Recurse -Force "monolith-typescript"
            git rm -r "monolith-typescript"
        }
        git commit -m "Remove unused language folders: monolith-dotnet, monolith-typescript"
    }
    
    git push origin main
    
    Write-Output "Repository created successfully: $GitHubUsername/$RepositoryName"
    Write-Output "Setup completed successfully"
    
    # Clean up the local clone
    Write-Output "Cleaning up local repository clone..."
    Set-Location ..
    Remove-Item -Recurse -Force $RepositoryName
    Write-Output "Local repository cleanup completed"
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $_"
    # Clean up on error too
    try {
        Set-Location ..
        if (Test-Path $RepositoryName) {
            Remove-Item -Recurse -Force $RepositoryName
            Write-Output "Cleaned up local repository after error"
        }
    } catch {
        Write-Warning "Could not clean up local repository: $_"
    }
    exit 1
}