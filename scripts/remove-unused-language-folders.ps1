function Get-LanguageItems {
    param(
        [string]$PathTemplate,
        [array]$Languages = @("java", "dotnet", "typescript")
    )
    
    $allItems = @()
    $languageToItemMapping = @{}
    
    foreach ($language in $Languages) {
        $item = $PathTemplate -replace '\{language\}', $language
        $allItems += $item
        $languageToItemMapping[$language] = $item
    }
    
    return @{
        AllItems = $allItems
        LanguageToItemMapping = $languageToItemMapping
    }
}

function Remove-LanguageSpecificItems {
    param(
        [string]$Language,
        [array]$AllItems,
        [hashtable]$LanguageToItemMapping,
        [array]$RemovedItems,
        [switch]$IsFolder
    )
    
    $keepItem = $LanguageToItemMapping[$Language.ToLower()]
    Write-Output "Keeping: $keepItem"
    
    foreach ($item in $AllItems) {
        if ($item -ne $keepItem -and (Test-Path $item)) {
            Write-Output "Removing: $item"
            
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
    
    $items = Get-LanguageItems -PathTemplate "monolith-{language}"
    return Remove-LanguageSpecificItems -Language $SystemLanguage -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems -IsFolder
}

function Remove-SystemTestFolders {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $items = Get-LanguageItems -PathTemplate "system-test-{language}"
    return Remove-LanguageSpecificItems -Language $SystemTestLanguage -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems -IsFolder
}

function Remove-CommitWorkflows {
    param(
        [string]$SystemLanguage,
        [array]$RemovedItems
    )
    
    $items = Get-LanguageItems -PathTemplate ".github/workflows/commit-stage-monolith-{language}.yml"
    return Remove-LanguageSpecificItems -Language $SystemLanguage -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems
}

function Remove-LocalAcceptanceWorkflows {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $items = Get-LanguageItems -PathTemplate ".github/workflows/local-acceptance-stage-test-{language}.yml"
    return Remove-LanguageSpecificItems -Language $SystemTestLanguage -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems
}

function Remove-AcceptanceWorkflows {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $items = Get-LanguageItems -PathTemplate ".github/workflows/acceptance-stage-test-{language}.yml"
    return Remove-LanguageSpecificItems -Language $SystemTestLanguage -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems
}

function Remove-QAWorkflows {
    param(
        [string]$SystemTestLanguage,
        [array]$RemovedItems
    )
    
    $items = Get-LanguageItems -PathTemplate ".github/workflows/qa-stage-test-{language}.yml"
    return Remove-LanguageSpecificItems -Language $SystemTestLanguage -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems
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