function Wait-ForBuildWorkflows {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName,
        [int]$TimeoutMinutes = 10
    )
    
    Write-Output "Waiting for build workflows to complete (timeout: $TimeoutMinutes minutes)..."
    
    $startTime = Get-Date
    $timeout = $startTime.AddMinutes($TimeoutMinutes)
    
    while ((Get-Date) -lt $timeout) {
        try {
            # Check if the Docker image exists
            $imageExists = Test-DockerImageExists -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
            
            if ($imageExists) {
                $elapsed = (Get-Date) - $startTime
                Write-Output "✅ Docker image is available after $([int]$elapsed.TotalMinutes) minutes"
                return $true
            }
            
            Write-Output "⏳ Still waiting for Docker image... (elapsed: $([int]((Get-Date) - $startTime).TotalMinutes) min)"
            Start-Sleep -Seconds 30
            
        } catch {
            Write-Warning "Error checking build status: $_"
            Start-Sleep -Seconds 30
        }
    }
    
    Write-Error "⚠️ Timeout waiting for build workflows to complete"
    return $false
}

function Test-DockerImageExists {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    $imageName = "ghcr.io/$RepositoryOwner/$RepositoryName/monolith-$($SystemLanguage.ToLower()):latest"
    
    try {
        # Try to get image metadata (this doesn't pull the image)
        docker manifest inspect $imageName > $null 2>&1
        return $LASTEXITCODE -eq 0
    } catch {
        return $false
    }
}