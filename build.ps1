param (
	[switch]$Pack = $false
)

function ExitIfFailed() {
	if ($LASTEXITCODE -ne 0) {
		Write-Host "Failed with code $LASTEXITCODE. Exiting..." -ForegroundColor Red
		Exit 1
	}
}

function CreateStamp() {
	return Get-Date -UFormat '+%Y%m%dT%H%M%S'
}

function CleanArtifacts() {
	if (Test-Path 'artifacts') {
		Remove-Item -Recurse 'artifacts'
	}
}

function CompoundVersionSuffix($versionSuffixes) {
	return (@($versionSuffixes) | Where-Object { $_ }) -join '.'
}

# Resolve src/test projects

$srcProjects = @(Get-ChildItem -Path './src/*/*.csproj' -File)
$testProjects = @(Get-ChildItem -Path './test/*/*.csproj' -File)
$allProjects = $srcProjects + $testProjects

# git

$sha = git rev-parse HEAD
$tags = [string[]]@(git tag --points-at HEAD) | Where-Object { $_ -Match 'v[0-9]+\.[0-9]+\.[0-9]+.*' }
$tagged = !!$tags.Length

# ---

$ci = Test-Path env:GITHUB_ACTIONS
$dev = -not $ci -and -not $tagged
$stamp = CreateStamp

# ---

foreach ($project in $allProjects) {
	dotnet restore $project
}
ExitIfFailed

foreach ($srcProject in $srcProjects) {
	dotnet build $srcProject --no-restore -c Release
}
ExitIfFailed

$testLoggersArg = ''
if ($ci) {
	$testLoggersArg = '--logger "GitHubActions"'
}

foreach ($testProject in $testProjects) {
	Invoke-Expression "dotnet test $testProject --no-restore -c Release $testLoggersArg"
}
ExitIfFailed

if ($Pack) {
	CleanArtifacts

	$versionSuffix = Select-Xml -Path 'version.props' -XPath '/Project/PropertyGroup/VersionSuffix' | ForEach-Object { $_.Node.InnerXML }

	$versionSuffixArg = ''
	if ($ci -or $dev) {
		$versionSuffix = CompoundVersionSuffix ($versionSuffix, "ci.$stamp+sha.$sha")
		$versionSuffixArg = "--version-suffix $versionSuffix"
	}

	foreach ($srcProject in $srcProjects) {
		Invoke-Expression "dotnet pack $srcProject --no-restore -c Release -o artifacts/packages $versionSuffixArg"
	}
}
