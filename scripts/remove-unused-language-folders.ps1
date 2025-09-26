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
    
    $keepFolder = $languageToFolder[$SystemLanguage.ToLower()]
    Write-Output "Keeping folder: $keepFolder"
    
    foreach ($folder in $allFolders) {
        if ($folder -ne $keepFolder -and (Test-Path $folder)) {
            Write-Output "Removing folder: $folder"
            Remove-Item -Recurse -Force $folder
            git rm -r $folder
            $RemovedItems += $folder
        }
    }
    
    return $RemovedItems
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
    
    $keepSystemTest = $languageToSystemTest[$SystemTestLanguage.ToLower()]
    Write-Output "Keeping system test: $keepSystemTest"
    
    foreach ($systemTest in $allSystemTests) {
        if ($systemTest -ne $keepSystemTest -and (Test-Path $systemTest)) {
            Write-Output "Removing system test: $systemTest"
            Remove-Item -Recurse -Force $systemTest
            git rm -r $systemTest
            $RemovedItems += $systemTest
        }
    }
    
    return $RemovedItems
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
    
    $keepWorkflow = $languageToWorkflow[$SystemLanguage.ToLower()]
    Write-Output "Keeping workflow: $keepWorkflow"
    
    foreach ($workflow in $allWorkflows) {
        if ($workflow -ne $keepWorkflow -and (Test-Path $workflow)) {
            Write-Output "Removing workflow: $workflow"
            Remove-Item -Force $workflow
            git rm $workflow
            $RemovedItems += $workflow
        }
    }
    
    return $RemovedItems
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
    
    $keepLocalAcceptanceWorkflow = $languageToLocalAcceptanceWorkflow[$SystemTestLanguage.ToLower()]
    Write-Output "Keeping local acceptance workflow: $keepLocalAcceptanceWorkflow"
    
    foreach ($localAcceptanceWorkflow in $allLocalAcceptanceWorkflows) {
        if ($localAcceptanceWorkflow -ne $keepLocalAcceptanceWorkflow -and (Test-Path $localAcceptanceWorkflow)) {
            Write-Output "Removing local acceptance workflow: $localAcceptanceWorkflow"
            Remove-Item -Force $localAcceptanceWorkflow
            git rm $localAcceptanceWorkflow
            $RemovedItems += $localAcceptanceWorkflow
        }
    }
    
    return $RemovedItems
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
    
    $keepAcceptanceWorkflow = $languageToAcceptanceWorkflow[$SystemTestLanguage.ToLower()]
    Write-Output "Keeping acceptance workflow: $keepAcceptanceWorkflow"
    
    foreach ($acceptanceWorkflow in $allAcceptanceWorkflows) {
        if ($acceptanceWorkflow -ne $keepAcceptanceWorkflow -and (Test-Path $acceptanceWorkflow)) {
            Write-Output "Removing acceptance workflow: $acceptanceWorkflow"
            Remove-Item -Force $acceptanceWorkflow
            git rm $acceptanceWorkflow
            $RemovedItems += $acceptanceWorkflow
        }
    }
    
    return $RemovedItems
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

    $keepQAWorkflow = $languageToQAWorkflow[$SystemTestLanguage.ToLower()]
    Write-Output "Keeping QA workflow: $keepQAWorkflow"

    foreach ($qaWorkflow in $allQAWorkflows) {
        if ($qaWorkflow -ne $keepQAWorkflow -and (Test-Path $qaWorkflow)) {
            Write-Output "Removing QA workflow: $qaWorkflow"
            Remove-Item -Force $qaWorkflow
            git rm $qaWorkflow
            $RemovedItems += $qaWorkflow
        }
    }
    
    return $RemovedItems
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