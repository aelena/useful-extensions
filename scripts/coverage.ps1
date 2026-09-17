#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs the test suite on every target framework, merges the coverage
    results and fails when line, branch or method coverage drops below 100%.

.PARAMETER Minimum
    Minimum acceptable percentage for each metric. Defaults to 100.

.EXAMPLE
    pwsh scripts/coverage.ps1
#>
[CmdletBinding()]
param(
    [ValidateRange(0, 100)]
    [int] $Minimum = 100
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root

try {
    if (Test-Path TestResults) {
        Remove-Item -Recurse -Force TestResults
    }

    dotnet test --coverage --coverage-output-format cobertura
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    dotnet tool restore | Out-Null
    dotnet tool run reportgenerator `
        -reports:"TestResults/*.cobertura.xml" `
        -targetdir:TestResults/report `
        -reporttypes:"Html;TextSummary;JsonSummary;MarkdownSummaryGithub" `
        -assemblyfilters:+Aelena.Extensions `
        -verbosity:Warning
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    Get-Content TestResults/report/Summary.txt

    $summary = (Get-Content TestResults/report/Summary.json -Raw | ConvertFrom-Json).summary
    $failed = @()
    foreach ($metric in 'linecoverage', 'branchcoverage', 'methodcoverage') {
        if ($summary.$metric -lt $Minimum) {
            $failed += "$metric = $($summary.$metric)%"
        }
    }

    if ($failed.Count -gt 0) {
        Write-Host ""
        Write-Host "Coverage below $Minimum%: $($failed -join ', ')" -ForegroundColor Red
        Write-Host "Open TestResults/report/index.html to see what is missing."
        exit 1
    }

    Write-Host ""
    Write-Host "Coverage gate passed: line, branch and method coverage are all >= $Minimum%." -ForegroundColor Green
}
finally {
    Pop-Location
}
