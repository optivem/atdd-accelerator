function Enable-GitHubPages {
    param(
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Host "Enabling GitHub Pages..."
    
    try {
        # Enable GitHub Pages with main branch and /docs folder
        $pagesConfig = @{
            source = @{
                branch = "main"
                path = "/docs"
            }
        } | ConvertTo-Json -Depth 3
        
        # Use GitHub CLI to enable pages
        $result = gh api -X POST "repos/$RepositoryOwner/$RepositoryName/pages" -f "source[branch]=main" -f "source[path]=/docs" 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "GitHub Pages enabled successfully"
            return $true
        } else {
            Write-Warning "Failed to enable GitHub Pages: $result"
            return $false
        }
    } catch {
        Write-Warning "Error enabling GitHub Pages: $_"
        return $false
    }
}