#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Fails when any direct or transitive NuGet dependency has a known vulnerability,
    and reports deprecated packages. The .NET counterpart of an OWASP Dependency Check gate.

.DESCRIPTION
    Uses the NuGet vulnerability database that `dotnet list package --vulnerable` queries
    (GitHub Advisory Database via nuget.org). Restore already audits packages during the
    build (NuGetAuditMode=all); this script is the explicit, machine-readable gate.

.PARAMETER Solution
    The solution to audit. Defaults to the .slnx at the repository root.

.EXAMPLE
    pwsh scripts/audit.ps1
#>
[CmdletBinding()]
param(
    [string] $Solution = 'Aelena.CommonExtensions.slnx'
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root

try {
    dotnet restore $Solution | Out-Null
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    $report = dotnet list $Solution package --vulnerable --include-transitive --format json | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    $findings = @()
    foreach ($project in $report.projects) {
        foreach ($framework in $project.frameworks) {
            foreach ($package in @($framework.topLevelPackages) + @($framework.transitivePackages)) {
                foreach ($vulnerability in @($package.vulnerabilities)) {
                    $findings += [pscustomobject]@{
                        Project   = Split-Path -Leaf $project.path
                        Framework = $framework.framework
                        Package   = "$($package.id) $($package.resolvedVersion)"
                        Severity  = $vulnerability.severity
                        Advisory  = $vulnerability.advisoryurl
                    }
                }
            }
        }
    }

    $deprecated = dotnet list $Solution package --deprecated --include-transitive --format json | ConvertFrom-Json
    $deprecations = @()
    foreach ($project in $deprecated.projects) {
        foreach ($framework in $project.frameworks) {
            foreach ($package in @($framework.topLevelPackages) + @($framework.transitivePackages)) {
                if ($package.deprecationReasons) {
                    $deprecations += "$($package.id) $($package.resolvedVersion) ($($package.deprecationReasons -join ', '))"
                }
            }
        }
    }

    if ($deprecations.Count -gt 0) {
        Write-Host "Deprecated packages (not fatal):" -ForegroundColor Yellow
        $deprecations | Sort-Object -Unique | ForEach-Object { Write-Host "  $_" }
    }

    if ($findings.Count -gt 0) {
        Write-Host ""
        Write-Host "Vulnerable packages found:" -ForegroundColor Red
        $findings | Format-Table -AutoSize | Out-String | Write-Host
        exit 1
    }

    Write-Host "Dependency audit passed: no known vulnerabilities in direct or transitive packages." -ForegroundColor Green
}
finally {
    Pop-Location
}
