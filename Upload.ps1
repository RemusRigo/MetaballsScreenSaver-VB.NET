#---------------------------------------------------------------------------------------------------------------------------
# Sync to github
#    © 2025-2026 Remus Rigo
#       v1.2.20261001
#---------------------------------------------------------------------------------------------------------------------------

param([string]$msg)

if (-not $msg)
{
    $msg = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
}

# always work in the project folder (where this script is), never in a parent folder
Set-Location $PSScriptRoot

$proj, $lng = ($PSScriptRoot -split '\\')[-1,-2]
if ($lng -eq "C++") { $lng="CPlusPlus"}
if ($lng -eq "C#") { $lng="CSharp"}
$remoteUrl = "https://github.com/RemusRigo/" + $proj.Replace(' ', '') + "-" + $lng + ".git"

if (-not (Test-Path ".git"))
{
    # initialize project (first upload, or .git folder was deleted)
    git init
    git branch -M main
}

# set the right URL (if project name changed)
if (git remote) { git remote set-url origin $remoteUrl } else { git remote add origin $remoteUrl }

# upload only the current project: github is never pulled/merged into local (no <<<<<<< markers)
# continue github's history, but the files uploaded are exactly the local ones
if (git ls-remote --heads origin main)
{
    git fetch origin main
    git reset -q origin/main
}

git config user.email "remusrigo@hotmail.com"
git config user.name "Remus Rigo"

# upload to github
git add .

# never upload files with merge conflict markers
$changed = @(git diff --cached --name-only --diff-filter=AM | Where-Object { Test-Path -LiteralPath $_ })
$conflicts = $null
if ($changed.Count -gt 0)
{
    $conflicts = Select-String -LiteralPath $changed -Pattern '^(<<<<<<<|>>>>>>>)( |$)' -List
}
if ($conflicts)
{
    Write-Host "Merge conflict markers found - fix them first. Nothing was uploaded:" -ForegroundColor Red
    $conflicts | ForEach-Object { Write-Host ("   " + $_.Path + ":" + $_.LineNumber) -ForegroundColor Red }
    git reset -q
    exit 1
}

git commit -m "$msg"
git push -u origin main
