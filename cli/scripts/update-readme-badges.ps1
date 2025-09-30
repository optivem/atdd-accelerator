function Update-ReadmeBadges {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName,
        [string]$SystemTestLanguage
    )
    
    Write-Host "Updating README badges..."
    Write-Host "SystemLanguage: $SystemLanguage"
    Write-Host "SystemTestLanguage: $SystemTestLanguage"
    
    if (-not (Test-Path "README.md")) {
        Write-Warning "README.md not found, skipping badge update"
        return $false
    }
    
    $readmeContent = Get-Content "README.md" -Raw
    $originalContent = $readmeContent
    
    # Define all badge patterns to remove based on what should be kept
    $badgesToRemove = @()
    
    # For commit stage, keep only SystemLanguage
    switch ($SystemLanguage.ToLower()) {
        "java" {
            $badgesToRemove += "commit-stage-monolith-dotnet"
            $badgesToRemove += "commit-stage-monolith-typescript"
        }
        "dotnet" {
            $badgesToRemove += "commit-stage-monolith-java"
            $badgesToRemove += "commit-stage-monolith-typescript"
        }
        "typescript" {
            $badgesToRemove += "commit-stage-monolith-java"
            $badgesToRemove += "commit-stage-monolith-dotnet"
        }
    }
    
    # For test stages, keep ONLY SystemTestLanguage badges, remove ALL others including SystemLanguage
    $allTestLanguages = @("java", "dotnet", "typescript")
    
    foreach ($lang in $allTestLanguages) {
        if ($lang -ne $SystemTestLanguage.ToLower()) {
            $badgesToRemove += "local-acceptance-stage-test-$lang"
            $badgesToRemove += "acceptance-stage-test-$lang"
            $badgesToRemove += "qa-stage-test-$lang"
            $badgesToRemove += "prod-stage-test-$lang"
        }
    }
    
    Write-Host "Badges to remove: $($badgesToRemove -join ', ')"
    
    # Remove unwanted badges (entire lines)
    foreach ($badge in $badgesToRemove) {
        $beforeCount = ($readmeContent | Select-String $badge -AllMatches).Matches.Count
        
        # Pattern to match the entire line containing the badge
        $pattern = ".*\[!\[$badge\].*(?:\r?\n)?"
        $readmeContent = $readmeContent -replace $pattern, ""
        
        $afterCount = ($readmeContent | Select-String $badge -AllMatches).Matches.Count
        Write-Host "Removed badge '$badge': Before=$beforeCount, After=$afterCount"
    }
    
    # Update repository paths in remaining badges
    $readmeContent = $readmeContent -replace "optivem/atdd-accelerator-template-mono-repo", "$RepositoryOwner/$RepositoryName"
    Write-Host "Updated repository paths to: $RepositoryOwner/$RepositoryName"
    
    # Check if content changed
    if ($readmeContent -ne $originalContent) {
        Set-Content "README.md" -Value $readmeContent -NoNewline
        git add "README.md" | Out-Null
        Write-Host "README badges updated successfully"
        return $true
    }
    
    Write-Host "No changes needed to README badges"
    return $false
}