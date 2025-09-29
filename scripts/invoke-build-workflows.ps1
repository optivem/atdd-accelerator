function Test-ContainerHealth {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName
    )
    
    Write-Output "Checking container health..."
    
    try {
        # Check if container is running
        $containerStatus = docker ps --filter "name=monolith" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
        Write-Output "Container status:"
        Write-Output $containerStatus
        
        # Check container logs for startup issues
        Write-Output ""
        Write-Output "Container logs (last 20 lines):"
        docker logs --tail 20 $(docker ps -q --filter "name=monolith") 2>&1
        
        # Test if the application is responding
        Write-Output ""
        Write-Output "Testing application health..."
        
        # Wait for application to start (Java apps need time)
        $maxAttempts = 12 # 2 minutes
        $attempt = 0
        
        while ($attempt -lt $maxAttempts) {
            try {
                $response = Invoke-WebRequest -Uri "http://localhost:8080/actuator/health" -TimeoutSec 5 -ErrorAction Stop
                if ($response.StatusCode -eq 200) {
                    Write-Output "✅ Application is healthy and responding"
                    return $true
                }
            } catch {
                $attempt++
                Write-Output "⏳ Attempt $attempt/$maxAttempts - Application not ready yet..."
                Start-Sleep -Seconds 10
            }
        }
        
        Write-Warning "❌ Application is not responding after 2 minutes"
        return $false
        
    } catch {
        Write-Error "Error checking container health: $_"
        return $false
    }
}

function Wait-ForBuildWorkflows {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName,
        [int]$TimeoutMinutes = 10
    )
    
    Write-Output "Checking if Docker image exists and container can start..."
    
    # First check if image exists
    $imageExists = Test-DockerImageExists -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
    
    if (-not $imageExists) {
        Write-Warning "❌ Docker image does not exist yet"
        return $false
    }
    
    Write-Output "✅ Docker image exists"
    
    # Test if container can start and application is healthy
    $containerHealthy = Test-ContainerHealth -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
    
    return $containerHealthy
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