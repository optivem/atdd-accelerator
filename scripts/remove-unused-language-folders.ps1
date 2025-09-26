function Remove-LanguageSpecificItems {
    param(
        [string]$ItemType,
        [string]$Language,
        [array]$AllItems,
        [hashtable]$LanguageToItemMapping,
        [array]$RemovedItems,
        [switch]$IsFolder
    )
    
    $keepItem = $LanguageToItemMapping[$Language.ToLower()]
    Write-Output "Keeping $ItemType`: $keepItem"
    
    foreach ($item in $AllItems) {
        if ($item -ne $keepItem -and (Test-Path $item)) {
            Write-Output "Removing $ItemType`: $item"
            
            if ($IsFolder) {
                Remove-Item -Recurse -Force $item
                git rm -r $item
            } else {
                Remove-Item -Force $item
                git rm $item
            }
            
            $RemovedItems += $item
        }
    }
    
    return $RemovedItems
}

function Remove-MonolithFolders {
    param(
        [string]$SystemLanguage,
        [array]$RemovedItems
    )
    
    $allFolders = @("monolith-java", "monolith-dotnet", "monolith-typescript")
    $languageToFolder = @{
        "java" = "monolith-java"
        "dotnet" = "monolith-dotnet" 
        "typescript" = "monolith-typescript"
    }
    
    return Remove-LanguageSpecificItems -ItemType "folder" -Language $SystemLanguage -AllItems $allFolders -LanguageToItemMapping $languageToFolder -RemovedItems $RemovedItems -IsFolder
}

function Remove-SystemTestFolders {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $allSystemTests = @("system-test-java", "system-test-dotnet", "system-test-typescript")
    $languageToSystemTest = @{
        "java" = "system-test-java"
        "dotnet" = "system-test-dotnet"
        "typescript" = "system-test-typescript"
    }
    
    return Remove-LanguageSpecificItems -ItemType "system test" -Language $SystemTestLanguage -AllItems $allSystemTests -LanguageToItemMapping $languageToSystemTest -RemovedItems $RemovedItems -IsFolder
}

function Remove-CommitWorkflows {
    param(
        [string]$SystemLanguage,
        [array]$RemovedItems
    )
    
    $allWorkflows = @(
        ".github/workflows/commit-stage-monolith-java.yml",
        ".github/workflows/commit-stage-monolith-dotnet.yml", 
        ".github/workflows/commit-stage-monolith-typescript.yml"
    )
    $languageToWorkflow = @{
        "java" = ".github/workflows/commit-stage-monolith-java.yml"
        "dotnet" = ".github/workflows/commit-stage-monolith-dotnet.yml"
        "typescript" = ".github/workflows/commit-stage-monolith-typescript.yml"
    }
    
    return Remove-LanguageSpecificItems -ItemType "commit workflow" -Language $SystemLanguage -AllItems $allWorkflows -LanguageToItemMapping $languageToWorkflow -RemovedItems $RemovedItems
}

function Remove-LocalAcceptanceWorkflows {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $allLocalAcceptanceWorkflows = @(
        ".github/workflows/local-acceptance-stage-test-java.yml",
        ".github/workflows/local-acceptance-stage-test-dotnet.yml",
        ".github/workflows/local-acceptance-stage-test-typescript.yml"
    )
    $languageToLocalAcceptanceWorkflow = @{
        "java" = ".github/workflows/local-acceptance-stage-test-java.yml"
        "dotnet" = ".github/workflows/local-acceptance-stage-test-dotnet.yml"
        "typescript" = ".github/workflows/local-acceptance-stage-test-typescript.yml"
    }
    
    return Remove-LanguageSpecificItems -ItemType "local acceptance workflow" -Language $SystemTestLanguage -AllItems $allLocalAcceptanceWorkflows -LanguageToItemMapping $languageToLocalAcceptanceWorkflow -RemovedItems $RemovedItems
}

function Remove-AcceptanceWorkflows {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $allAcceptanceWorkflows = @(
        ".github/workflows/acceptance-stage-test-java.yml",
        ".github/workflows/acceptance-stage-test-dotnet.yml",
        ".github/workflows/acceptance-stage-test-typescript.yml"
    )
    $languageToAcceptanceWorkflow = @{
        "java" = ".github/workflows/acceptance-stage-test-java.yml"
        "dotnet" = ".github/workflows/acceptance-stage-test-dotnet.yml"
        "typescript" = ".github/workflows/acceptance-stage-test-typescript.yml"
    }
    
    return Remove-LanguageSpecificItems -ItemType "acceptance workflow" -Language $SystemTestLanguage -AllItems $allAcceptanceWorkflows -LanguageToItemMapping $languageToAcceptanceWorkflow -RemovedItems $RemovedItems
}

function Remove-QAWorkflows {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $allQAWorkflows = @(
        ".github/workflows/qa-stage-test-java.yml",
        ".github/workflows/qa-stage-test-dotnet.yml",
        ".github/workflows/qa-stage-test-typescript.yml"
    )
    $languageToQAWorkflow = @{
        "java" = ".github/workflows/qa-stage-test-java.yml"
        "dotnet" = ".github/workflows/qa-stage-test-dotnet.yml"
        "typescript" = ".github/workflows/qa-stage-test-typescript.yml"
    }
    
    return Remove-LanguageSpecificItems -ItemType "QA workflow" -Language $SystemTestLanguage -AllItems $allQAWorkflows -LanguageToItemMapping $languageToQAWorkflow -RemovedItems $RemovedItems
}

function Remove-UnusedLanguageFolders {
    param(
        [string]$SystemLanguage,
        [string]$SystemTestLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Removing unused language folders..."
    Write-Output "System Language: $SystemLanguage"
    Write-Output "System Test Language: $SystemTestLanguage"
    
    # Remove unused items using helper methods
    $removedItems = @()
    
    # Remove monolith folders
    $removedItems = Remove-MonolithFolders -SystemLanguage $SystemLanguage -RemovedItems $removedItems
    
    # Remove system test folders
    $removedItems = Remove-SystemTestFolders -SystemTestLanguage $SystemTestLanguage -RemovedItems $removedItems
    
    # Remove commit stage workflow files
    $removedItems = Remove-CommitWorkflows -SystemLanguage $SystemLanguage -RemovedItems $removedItems
    
    # Remove local acceptance stage test workflow files
    $removedItems = Remove-LocalAcceptanceWorkflows -SystemTestLanguage $SystemTestLanguage -RemovedItems $removedItems
    
    # Remove acceptance stage test workflow files
    $removedItems = Remove-AcceptanceWorkflows -SystemTestLanguage $SystemTestLanguage -RemovedItems $removedItems
    
    # Remove QA workflows
    $removedItems = Remove-QAWorkflows -SystemTestLanguage $SystemTestLanguage -RemovedItems $removedItems
    
    # Update README badges
    $readmeUpdated = Update-ReadmeBadges -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
    
    # Update Docker Compose files
    $dockerComposeUpdated = Update-DockerComposeFiles -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
    
    # Commit and return whether changes were made
    $hasChanges = ($removedItems.Count -gt 0) -or $readmeUpdated -or $dockerComposeUpdated
    if ($hasChanges) {
        $changes = @()
        if ($removedItems.Count -gt 0) { $changes += "Remove unused folders/workflows" }
        if ($readmeUpdated) { $changes += "Update README badges" }
        if ($dockerComposeUpdated) { $changes += "Update Docker Compose files" }
        
        $commitMessage = $changes -join ", "
        git commit -m $commitMessage
        return $true
    }
    
    return $false
}