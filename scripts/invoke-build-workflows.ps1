function Wait-ForBuildWorkflows {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName,
        [int]$TimeoutMinutes = 10
    )
    
    Write-Output "Waiting for build workflows to complete..."
    
    # First check if the commit-stage workflow has completed
    $commitWorkflowCompleted = Wait-ForCommitStageWorkflow -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName -TimeoutMinutes $TimeoutMinutes
    
    if (-not $commitWorkflowCompleted) {
        Write-Error "❌ Commit stage workflow did not complete successfully"
        return $false
    }
    
    Write-Output "✅ Commit stage workflow completed successfully"
    
    # Now check if Docker image exists (should be available after commit-stage)
    Write-Output "Checking if Docker image exists..."
    $imageExists = Test-DockerImageExists -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
    
    if (-not $imageExists) {
        Write-Error "❌ Docker image does not exist even after commit stage completion"
        return $false
    }
    
    Write-Output "✅ Docker image exists"
    
    # Test if container can start and application is healthy
    $containerHealthy = Test-ContainerHealth -SystemLanguage $SystemLanguage -RepositoryOwner $RepositoryOwner -RepositoryName $RepositoryName
    
    return $containerHealthy
}

function Wait-ForCommitStageWorkflow {
    param(
        [string]$SystemLanguage,
        [string]$RepositoryOwner,
        [string]$RepositoryName,
        [int]$TimeoutMinutes = 10
    )
    
    $workflowName = "commit-stage-$($SystemLanguage.ToLower())"
    Write-Output "Waiting for workflow '$workflowName' to complete..."
    
    $startTime = Get-Date
    $timeout = $startTime.AddMinutes($TimeoutMinutes)
    
    while ((Get-Date) -lt $timeout) {
        try {
            # Get the latest workflow run for this workflow
            $workflowRuns = gh run list --repo "$RepositoryOwner/$RepositoryName" --workflow "$workflowName" --limit 1 --json status,conclusion,createdAt | ConvertFrom-Json
            
            if ($workflowRuns.Count -eq 0) {
                $elapsed = (Get-Date) - $startTime
                Write-Output "⏳ No workflow runs found yet... (elapsed: $([int]$elapsed.TotalMinutes) min)"
                Start-Sleep -Seconds 30
                continue
            }
            
            $latestRun = $workflowRuns[0]
            $status = $latestRun.status
            $conclusion = $latestRun.conclusion
            
            Write-Output "Workflow status: $status, conclusion: $conclusion"
            
            if ($status -eq "completed") {
                if ($conclusion -eq "success") {
                    $elapsed = (Get-Date) - $startTime
                    Write-Output "✅ Workflow '$workflowName' completed successfully (after $([int]$elapsed.TotalMinutes) minutes)"
                    return $true
                } else {
                    Write-Error "❌ Workflow '$workflowName' completed but failed with conclusion: $conclusion"
                    
                    # Show workflow URL for debugging
                    $workflowUrl = "https://github.com/$RepositoryOwner/$RepositoryName/actions/workflows/$workflowName.yml"
                    Write-Error "Check workflow details at: $workflowUrl"
                    return $false
                }
            } else {
                $elapsed = (Get-Date) - $startTime
                Write-Output "⏳ Workflow '$workflowName' is still running... (elapsed: $([int]$elapsed.TotalMinutes) min)"
                Start-Sleep -Seconds 30
            }
            
        } catch {
            Write-Warning "Error checking workflow status: $_"
            Start-Sleep -Seconds 30
        }
    }
    
    Write-Error "❌ Timeout waiting for workflow '$workflowName' to complete"
    return $false
}

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

function Test-ApplicationReady {
    param(
        [string]$ContainerName = "monolith",
        [int]$TimeoutSeconds = 120
    )
    
    Write-Output "Waiting for application to be ready (timeout: $TimeoutSeconds seconds)..."
    
    $startTime = Get-Date
    $timeout = $startTime.AddSeconds($TimeoutSeconds)
    
    while ((Get-Date) -lt $timeout) {
        try {
            # Check if container is still running
            $containerRunning = docker ps --filter "name=$ContainerName" --format "{{.Names}}" | Select-String $ContainerName
            
            if (-not $containerRunning) {
                Write-Error "❌ Container is not running"
                docker logs $ContainerName
                return $false
            }
            
            # Test different health endpoints
            $endpoints = @(
                "http://localhost:8080/",
                "http://localhost:8080/actuator/health",
                "http://localhost:8080/health"
            )
            
            foreach ($endpoint in $endpoints) {
                try {
                    $response = Invoke-WebRequest -Uri $endpoint -TimeoutSec 5 -ErrorAction Stop
                    if ($response.StatusCode -eq 200) {
                        $elapsed = (Get-Date) - $startTime
                        Write-Output "✅ Application is ready at $endpoint (after $([int]$elapsed.TotalSeconds) seconds)"
                        return $true
                    }
                } catch {
                    # Try next endpoint
                }
            }
            
            $elapsed = (Get-Date) - $startTime
            Write-Output "⏳ Still waiting for application... (elapsed: $([int]$elapsed.TotalSeconds)s)"
            Start-Sleep -Seconds 5
            
        } catch {
            Write-Warning "Error checking application: $_"
            Start-Sleep -Seconds 5
        }
    }
    
    Write-Error "❌ Application did not become ready within $TimeoutSeconds seconds"
    
    # Show container logs for debugging
    Write-Output "Container logs:"
    docker logs $ContainerName
    
    return $false
}