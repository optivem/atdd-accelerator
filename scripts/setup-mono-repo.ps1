param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$SystemLanguage
)

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

function Remove-UnusedLanguageFolders {
    param([string]$SystemLanguage)
    
    Write-Output "Removing unused language folders..."
    
    # Define all language folders
    $allFolders = @("monolith-java", "monolith-dotnet", "monolith-typescript")
    
    # Map language to folder to keep
    $languageToFolder = @{
        "java" = "monolith-java"
        "dotnet" = "monolith-dotnet" 
        "typescript" = "monolith-typescript"
    }
    
    # Default to Java if SystemLanguage not specified or unknown
    $keepFolder = $languageToFolder[$SystemLanguage.ToLower()]
    if (-not $keepFolder) {
        $keepFolder = "monolith-java"
    }
    
    Write-Output "Keeping folder: $keepFolder"
    
    # Remove all folders except the one to keep
    $removedFolders = @()
    foreach ($folder in $allFolders) {
        if ($folder -ne $keepFolder -and (Test-Path $folder)) {
            Write-Output "Removing folder: $folder"
            Remove-Item -Recurse -Force $folder
            git rm -r $folder
            $removedFolders += $folder
        }
    }
    
    # Commit changes if any folders were removed
    if ($removedFolders.Count -gt 0) {
        $removedList = $removedFolders -join ", "
        git commit -m "Remove unused language folders: $removedList"
    }
}

function Push-RepositoryChanges {
    Write-Output "Pushing changes to remote repository..."
    git push origin main
    
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to push changes"
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
    
    $GitHubUsername = Get-GitHubUsername -ProvidedUsername $GitHubUsername
    Write-Output "GitHub Username: $GitHubUsername"
    
    New-RepositoryFromTemplate -RepositoryName $RepositoryName
    
    # Change to the repository directory
    Set-Location $RepositoryName
    
    Remove-UnusedLanguageFolders -SystemLanguage $SystemLanguage
    Push-RepositoryChanges
    
    Write-Output "Repository created successfully: $GitHubUsername/$RepositoryName"
    Write-Output "Setup completed successfully"
    
    Remove-LocalRepository -RepositoryName $RepositoryName
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $_"
    Invoke-ErrorCleanup -RepositoryName $RepositoryName
    exit 1
}