param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$SystemLanguage
)

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

function Update-ReadmeBadges {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Updating README badges..."
    
    if (-not (Test-Path "README.md")) {
        Write-Warning "README.md not found, skipping badge update"
        return $false
    }
    
    $readmeContent = Get-Content "README.md" -Raw
    
    # Define badge patterns to remove based on language
    $badgesToRemove = @()
    $badgesToUpdate = @()
    
    switch ($SystemLanguage.ToLower()) {
        "java" {
            $badgesToRemove = @("commit-stage-monolith-dotnet", "commit-stage-monolith-typescript")
            $badgesToUpdate = @("commit-stage-monolith-java")
        }
        "dotnet" {
            $badgesToRemove = @("commit-stage-monolith-java", "commit-stage-monolith-typescript") 
            $badgesToUpdate = @("commit-stage-monolith-dotnet")
        }
        "typescript" {
            $badgesToRemove = @("commit-stage-monolith-java", "commit-stage-monolith-dotnet")
            $badgesToUpdate = @("commit-stage-monolith-typescript")
        }
    }
    
    $originalContent = $readmeContent
    
    # Remove unwanted badges (entire lines)
    foreach ($badge in $badgesToRemove) {
        $pattern = ".*\[!\[$badge\].*\n"
        $readmeContent = $readmeContent -replace $pattern, ""
    }
    
    # Update repository paths in remaining badges
    foreach ($badge in $badgesToUpdate) {
        $readmeContent = $readmeContent -replace "optivem/atdd-accelerator-template-mono-repo", "$RepositoryOwner/$RepositoryName"
    }
    
    # Check if content changed
    if ($readmeContent -ne $originalContent) {
        Set-Content "README.md" -Value $readmeContent -NoNewline
        git add "README.md"
        Write-Output "README badges updated"
        return $true
    }
    
    return $false
}

function Remove-UnusedLanguageFolders {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Removing unused language folders..."
    
    # Define all language folders
    $allFolders = @("monolith-java", "monolith-dotnet", "monolith-typescript")
    
    # Define all system test folders
    $allSystemTests = @("system-test-java", "system-test-dotnet", "system-test-typescript")
    
    # Define all workflow files
    $allWorkflows = @(
        ".github/workflows/commit-stage-monolith-java.yml",
        ".github/workflows/commit-stage-monolith-dotnet.yml", 
        ".github/workflows/commit-stage-monolith-typescript.yml"
    )
    
    # Map language to items to keep
    $languageToFolder = @{
        "java" = "monolith-java"
        "dotnet" = "monolith-dotnet" 
        "typescript" = "monolith-typescript"
    }
    
    $languageToSystemTest = @{
        "java" = "system-test-java"
        "dotnet" = "system-test-dotnet"
        "typescript" = "system-test-typescript"
    }
    
    $languageToWorkflow = @{
        "java" = ".github/workflows/commit-stage-monolith-java.yml"
        "dotnet" = ".github/workflows/commit-stage-monolith-dotnet.yml"
        "typescript" = ".github/workflows/commit-stage-monolith-typescript.yml"
    }
    
    # Get the values directly
    $keepFolder = $languageToFolder[$SystemLanguage.ToLower()]
    $keepSystemTest = $languageToSystemTest[$SystemLanguage.ToLower()]
    $keepWorkflow = $languageToWorkflow[$SystemLanguage.ToLower()]
    
    Write-Output "Keeping folder: $keepFolder"
    Write-Output "Keeping system test: $keepSystemTest"
    Write-Output "Keeping workflow: $keepWorkflow"
    
    # Remove unused items
    $removedItems = @()
    
    # Remove monolith folders
    foreach ($folder in $allFolders) {
        if ($folder -ne $keepFolder -and (Test-Path $folder)) {
            Write-Output "Removing folder: $folder"
            Remove-Item -Recurse -Force $folder
            git rm -r $folder
            $removedItems += $folder
        }
    }
    
    # Remove system test folders
    foreach ($systemTest in $allSystemTests) {
        if ($systemTest -ne $keepSystemTest -and (Test-Path $systemTest)) {
            Write-Output "Removing system test: $systemTest"
            Remove-Item -Recurse -Force $systemTest
            git rm -r $systemTest
            $removedItems += $systemTest
        }
    }
    
    # Remove workflow files
    foreach ($workflow in $allWorkflows) {
        if ($workflow -ne $keepWorkflow -and (Test-Path $workflow)) {
            Write-Output "Removing workflow: $workflow"
            Remove-Item -Force $workflow
            git rm $workflow
            $removedItems += $workflow
        }
    }
    
    # Update README badges
    $readmeUpdated = Update-ReadmeBadges -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
    
    # Commit and return whether changes were made
    $hasChanges = ($removedItems.Count -gt 0) -or $readmeUpdated
    if ($hasChanges) {
        if ($removedItems.Count -gt 0 -and $readmeUpdated) {
            $removedList = $removedItems -join ", "
            $commitMessage = "Remove unused language folders/system tests/workflows and update README: $removedList"
        } elseif ($removedItems.Count -gt 0) {
            $removedList = $removedItems -join ", "
            $commitMessage = "Remove unused language folders, system tests and workflows: $removedList"
        } else {
            $commitMessage = "Update README badges"
        }
        git commit -m $commitMessage
        return $true
    }
    
    return $false
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
    
    Test-SystemLanguage -SystemLanguage $SystemLanguage

    $GitHubUsername = Get-GitHubUsername -ProvidedUsername $GitHubUsername
    Write-Output "GitHub Username: $GitHubUsername"
    
    New-RepositoryFromTemplate -RepositoryName $RepositoryName
    
    # Change to the repository directory
    Set-Location $RepositoryName
    
    $hasChanges = Remove-UnusedLanguageFolders -SystemLanguage $SystemLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
    Push-RepositoryChanges -HasChanges $hasChanges
    
    Write-Output "Repository created successfully: $GitHubUsername/$RepositoryName"
    Write-Output "Setup completed successfully"
    
    Remove-LocalRepository -RepositoryName $RepositoryName
    
    exit 0
    
} catch {
    Write-Error "Setup failed: $_"
    Invoke-ErrorCleanup -RepositoryName $RepositoryName
    exit 1
}