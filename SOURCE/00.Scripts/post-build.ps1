Write-LogInfo "Running Project Post-Build PS1 Script: $MSBuildProjectName"
. "../../../../SCRIPTS/post-build-variables.ps1"
Write-LogInfo "Copying source (./bin) files to target: $relativeTargetDir"
Copy-Item -Path "./bin/*" -Destination $relativeTargetDir -Recurse
