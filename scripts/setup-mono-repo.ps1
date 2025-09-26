param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryName,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$SystemLanguage,

    [Parameter(Mandatory=$false)]
    [string]$SystemTestLanguage
)

# Import helper modules
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
. "$scriptDir\remove-unused-language-folders.ps1"

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

function Update-DockerComposeFiles {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Updating Docker Compose files..."
    
    # Find all docker-compose files in system-test folders
    $dockerComposeFiles = Get-ChildItem -Path "system-test-*" -Include "docker-compose.yml" -Recurse -ErrorAction SilentlyContinue
    
    $filesUpdated = $false
    
    foreach ($file in $dockerComposeFiles) {
        Write-Output "Updating Docker Compose file: $($file.FullName)"
        
        $content = Get-Content $file.FullName -Raw
        $originalContent = $content
        
        # First, update repository references
        $content = $content -replace "ghcr\.io/optivem/atdd-accelerator-template-mono-repo/", "ghcr.io/$RepositoryOwner/$RepositoryName/"
        
        # Define the target language service
        $targetService = "monolith-$($SystemLanguage.ToLower())"
        
        # Split content into lines for processing
        $lines = $content -split "`n"
        $newLines = @()
        
        $i = 0
        while ($i -lt $lines.Length) {
            $line = $lines[$i]
            
            # Check if this line starts a monolith service block
            if ($line -match "^\s*#?\s*monolith:\s*$") {
                # Look ahead to find the image line to determine the language
                $serviceLanguage = ""
                $serviceLines = @()
                $serviceLines += $line
                
                $j = $i + 1
                while ($j -lt $lines.Length -and $lines[$j] -match "^\s+(#\s*)?[^a-zA-Z]") {
                    $serviceLines += $lines[$j]
                    if ($lines[$j] -match "image:.*/(monolith-(java|dotnet|typescript)):latest") {
                        $serviceLanguage = $matches[2]
                    }
                    $j++
                }
                
                # If this is the target language, keep and uncomment the service
                if ($serviceLanguage -eq $SystemLanguage.ToLower()) {
                    foreach ($serviceLine in $serviceLines) {
                        # Uncomment and add to new lines
                        $cleanLine = $serviceLine -replace "^\s*#\s*", ""
                        if ($cleanLine -match "^monolith:\s*$") {
                            $newLines += "monolith:"
                        } elseif ($cleanLine -match "^\s*image:") {
                            $newLines += "  image: " + ($cleanLine -replace "^\s*image:\s*", "")
                        } elseif ($cleanLine -match "^\s*ports:") {
                            $newLines += "  ports:"
                        } elseif ($cleanLine -match "^\s*-\s*") {
                            $newLines += "    " + ($cleanLine -replace "^\s*-\s*", "- ")
                        } else {
                            $newLines += $cleanLine
                        }
                    }
                }
                # If not target language, skip this entire service block
                
                # Move index past this service block
                $i = $j - 1
            } else {
                # Not a monolith service line, keep as-is
                $newLines += $line
            }
            $i++
        }
        
        # Join lines back together
        $newContent = $newLines -join "`n"
        
        if ($newContent -ne $originalContent) {
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            git add $file.FullName
            $filesUpdated = $true
            Write-Output "Updated: $($file.FullName)"
        }
    }
    
    return $filesUpdated
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
    
    $hasChanges = Remove-UnusedLanguageFolders -SystemLanguage $SystemLanguage -SystemTestLanguage $SystemTestLanguage -RepositoryOwner $GitHubUsername -RepositoryName $RepositoryName
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