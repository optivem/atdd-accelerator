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
    Write-Host "Keeping: $keepItem"
    
    foreach ($item in $AllItems) {
        if ($item -ne $keepItem -and (Test-Path $item)) {
            Write-Host "Removing: $item"
            
            if ($IsFolder) {
                Remove-Item -Recurse -Force $item
                git rm -r $item | Out-Null
            } else {
                Remove-Item -Force $item
                git rm $item | Out-Null
            }
            
            $RemovedItems += $item
        }
    }
    
    return $RemovedItems
}

function Remove-ItemsByTemplate {
    param(
        [string]$PathTemplate,
        [string]$Language,
        [array]$RemovedItems,
        [switch]$IsFolder
    )
    
    $items = Get-LanguageItems -PathTemplate $PathTemplate
    
    if ($IsFolder) {
        return Remove-LanguageSpecificItems -Language $Language -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems -IsFolder
    } else {
        return Remove-LanguageSpecificItems -Language $Language -AllItems $items.AllItems -LanguageToItemMapping $items.LanguageToItemMapping -RemovedItems $RemovedItems
    }
}

function Remove-UnusedLanguageFolders {
    param(
        [string]$SystemLanguage,
        [string]$SystemTestLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Host "Removing unused language folders..."
    Write-Host "System Language: $SystemLanguage"
    Write-Host "System Test Language: $SystemTestLanguage"
    
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
        git commit -m $commitMessage | Out-Null
        return $true
    }
    
    return $false
}