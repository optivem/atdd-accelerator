function Remove-QAWorkflows {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    # Define all QA stage test workflows
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

    # Remove QA stage test workflow files
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
    
    # Define all local acceptance stage test workflows
    $allLocalAcceptanceWorkflows = @(
        ".github/workflows/local-acceptance-stage-test-java.yml",
        ".github/workflows/local-acceptance-stage-test-dotnet.yml",
        ".github/workflows/local-acceptance-stage-test-typescript.yml"
    )
    
    # Define all acceptance stage test workflows
    $allAcceptanceWorkflows = @(
        ".github/workflows/acceptance-stage-test-java.yml",
        ".github/workflows/acceptance-stage-test-dotnet.yml",
        ".github/workflows/acceptance-stage-test-typescript.yml"
    )
    
    # Map languages to items to keep
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
    
    $languageToLocalAcceptanceWorkflow = @{
        "java" = ".github/workflows/local-acceptance-stage-test-java.yml"
        "dotnet" = ".github/workflows/local-acceptance-stage-test-dotnet.yml"
        "typescript" = ".github/workflows/local-acceptance-stage-test-typescript.yml"
    }
    
    $languageToAcceptanceWorkflow = @{
        "java" = ".github/workflows/acceptance-stage-test-java.yml"
        "dotnet" = ".github/workflows/acceptance-stage-test-dotnet.yml"
        "typescript" = ".github/workflows/acceptance-stage-test-typescript.yml"
    }
    
    # Get the values directly
    $keepFolder = $languageToFolder[$SystemLanguage.ToLower()]
    $keepSystemTest = $languageToSystemTest[$SystemTestLanguage.ToLower()]
    $keepWorkflow = $languageToWorkflow[$SystemLanguage.ToLower()]
    $keepLocalAcceptanceWorkflow = $languageToLocalAcceptanceWorkflow[$SystemTestLanguage.ToLower()]
    $keepAcceptanceWorkflow = $languageToAcceptanceWorkflow[$SystemTestLanguage.ToLower()]
    
    Write-Output "Keeping folder: $keepFolder"
    Write-Output "Keeping system test: $keepSystemTest"
    Write-Output "Keeping workflow: $keepWorkflow"
    Write-Output "Keeping local acceptance workflow: $keepLocalAcceptanceWorkflow"
    Write-Output "Keeping acceptance workflow: $keepAcceptanceWorkflow"
    
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
    
    # Remove commit stage workflow files
    foreach ($workflow in $allWorkflows) {
        if ($workflow -ne $keepWorkflow -and (Test-Path $workflow)) {
            Write-Output "Removing workflow: $workflow"
            Remove-Item -Force $workflow
            git rm $workflow
            $removedItems += $workflow
        }
    }
    
    # Remove local acceptance stage test workflow files
    foreach ($localAcceptanceWorkflow in $allLocalAcceptanceWorkflows) {
        if ($localAcceptanceWorkflow -ne $keepLocalAcceptanceWorkflow -and (Test-Path $localAcceptanceWorkflow)) {
            Write-Output "Removing local acceptance workflow: $localAcceptanceWorkflow"
            Remove-Item -Force $localAcceptanceWorkflow
            git rm $localAcceptanceWorkflow
            $removedItems += $localAcceptanceWorkflow
        }
    }
    
    # Remove acceptance stage test workflow files
    foreach ($acceptanceWorkflow in $allAcceptanceWorkflows) {
        if ($acceptanceWorkflow -ne $keepAcceptanceWorkflow -and (Test-Path $acceptanceWorkflow)) {
            Write-Output "Removing acceptance workflow: $acceptanceWorkflow"
            Remove-Item -Force $acceptanceWorkflow
            git rm $acceptanceWorkflow
            $removedItems += $acceptanceWorkflow
        }
    }
    
    # Remove QA workflows using helper method
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