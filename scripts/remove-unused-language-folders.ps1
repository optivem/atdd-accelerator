function Remove-ItemsByTemplate {
    param(
        [string]$PathTemplate,
        [string]$Language,
        [array]$RemovedItems,
        [switch]$IsFolder
    )
    
    $items = Get-LanguageItems -PathTemplate $PathTemplate
    return Remove-LanguageSpecificItems -Language $Language -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems @(if ($IsFolder) { "-IsFolder" })
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
    
    # Remove unused items using template-based approach
    $removedItems = @()
    
    # Remove monolith folders
    $removedItems = Remove-ItemsByTemplate -PathTemplate "monolith-{language}" -Language $SystemLanguage -RemovedItems $removedItems -IsFolder
    
    # Remove system test folders
    $removedItems = Remove-ItemsByTemplate -PathTemplate "system-test-{language}" -Language $SystemTestLanguage -RemovedItems $removedItems -IsFolder
    
    # Remove commit stage workflow files
    $removedItems = Remove-ItemsByTemplate -PathTemplate ".github/workflows/commit-stage-monolith-{language}.yml" -Language $SystemLanguage -RemovedItems $removedItems
    
    # Remove local acceptance stage test workflow files
    $removedItems = Remove-ItemsByTemplate -PathTemplate ".github/workflows/local-acceptance-stage-test-{language}.yml" -Language $SystemTestLanguage -RemovedItems $removedItems
    
    # Remove acceptance stage test workflow files
    $removedItems = Remove-ItemsByTemplate -PathTemplate ".github/workflows/acceptance-stage-test-{language}.yml" -Language $SystemTestLanguage -RemovedItems $removedItems
    
    # Remove QA workflows
    $removedItems = Remove-ItemsByTemplate -PathTemplate ".github/workflows/qa-stage-test-{language}.yml" -Language $SystemTestLanguage -RemovedItems $removedItems
    
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