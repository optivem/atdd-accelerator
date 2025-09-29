function Update-DockerComposeFiles {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Updating Docker Compose files using templates..."
    
    # Find the single system-test folder
    $systemTestFolders = Get-ChildItem -Path "system-test-*" -Directory -ErrorAction SilentlyContinue
    
    if ($systemTestFolders.Count -eq 0) {
        Write-Error "No system-test folder found"
        return $false
    }
    
    if ($systemTestFolders.Count -gt 1) {
        Write-Error "Multiple system-test folders found: $($systemTestFolders.Name -join ', ')"
        Write-Error "Expected exactly one system-test folder"
        return $false
    }
    
    $systemTestFolder = $systemTestFolders[0]
    Write-Output "Found system-test folder: $($systemTestFolder.Name)"
    
    # Extract the system-test language from the folder name (e.g., "system-test-typescript" -> "typescript")
    if ($systemTestFolder.Name -match "^system-test-(.+)$") {
        $systemTestLanguage = $matches[1].ToLower()
        Write-Output "Detected system-test language: $systemTestLanguage"
    } else {
        Write-Error "Could not extract language from folder name: $($systemTestFolder.Name)"
        return $false
    }
    
    # Use the system-test language to find the correct template
    $templatePath = "temp\$systemTestLanguage\docker-compose.yml"
    
    # Check if template exists
    if (-not (Test-Path $templatePath)) {
        Write-Error "Template Docker Compose file not found: $templatePath"
        Write-Error "Available templates:"
        Get-ChildItem "temp" -Directory -ErrorAction SilentlyContinue | ForEach-Object {
            $dockerComposeFile = Join-Path $_.FullName "docker-compose.yml"
            if (Test-Path $dockerComposeFile) {
                Write-Error "  - $($_.Name): $dockerComposeFile"
            }
        }
        throw "Docker Compose template missing for language: $systemTestLanguage"
    }
    
    Write-Output "Using template: $templatePath"
    
    # Check if docker-compose.yml exists in the system-test folder
    $targetDockerCompose = Join-Path $systemTestFolder.FullName "docker-compose.yml"
    
    if (-not (Test-Path $targetDockerCompose)) {
        Write-Error "Docker Compose file not found in system-test folder: $targetDockerCompose"
        return $false
    }
    
    # Read template content
    $templateContent = Get-Content $templatePath -Raw
    
    # Update repository references in template content
    $updatedContent = $templateContent -replace "ghcr\.io/optivem/atdd-accelerator-template-mono-repo/", "ghcr.io/$RepositoryOwner/$RepositoryName/"
    
    # IMPORTANT: Replace the monolith image language with the SYSTEM language (not system-test language)
    # This ensures the monolith service uses the correct language (e.g., java) regardless of test language (e.g., typescript)
    $updatedContent = $updatedContent -replace "monolith-$systemTestLanguage", "monolith-$($SystemLanguage.ToLower())"
    
    Write-Output "Updated monolith image to use system language: $($SystemLanguage.ToLower())"
    
    # Check if target file would actually change
    $currentContent = Get-Content $targetDockerCompose -Raw
    if ($currentContent -eq $updatedContent) {
        Write-Output "No changes needed for: $targetDockerCompose"
        return $false
    }
    
    # Copy template and update it
    Set-Content -Path $targetDockerCompose -Value $updatedContent -NoNewline
    
    # Add to git
    git add $targetDockerCompose
    
    Write-Output "✅ Updated Docker Compose file: $targetDockerCompose"
    Write-Output "   Applied template for system-test language: $systemTestLanguage"
    Write-Output "   Updated monolith image to use system language: $($SystemLanguage.ToLower())"
    Write-Output "   Updated repository references to: ghcr.io/$RepositoryOwner/$RepositoryName/"
    
    return $true
}