param(
    [ValidateSet("test-edit", "test-play", "test-player", "build", "build-run", "run", "all")]
    [string]$Command = "all",

    [ValidateSet("StandaloneLinux64", "StandaloneWindows64", "StandaloneOSX")]
    [string]$Platform = "StandaloneLinux64",

    [string]$UnityEditorPath = $env:UNITY_EDITOR,
    [string]$ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot ".." )).Path
)

$ArtifactsDir = Join-Path $ProjectPath "Artifacts"
$LogDir = Join-Path $ArtifactsDir "Logs"
$ResultsDir = Join-Path $ArtifactsDir "TestResults"

if (-not $UnityEditorPath) {
    throw "Set UNITY_EDITOR or pass -UnityEditorPath to point at the Unity 6000.3.5f2 editor executable."
}

function Invoke-Unity {
    param([string[]]$Args)

    New-Item -ItemType Directory -Force -Path $LogDir, $ResultsDir | Out-Null
    & $UnityEditorPath @Args
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

$CommonArgs = @("-batchmode", "-quit", "-projectPath", $ProjectPath)

switch ($Command) {
    "test-edit" {
        Invoke-Unity (@CommonArgs + @("-runTests", "-testPlatform", "EditMode", "-testResults", (Join-Path $ResultsDir "editmode-results.xml"), "-logFile", (Join-Path $LogDir "editmode.log")))
    }
    "test-play" {
        Invoke-Unity (@CommonArgs + @("-runTests", "-testPlatform", "PlayMode", "-testResults", (Join-Path $ResultsDir "playmode-results.xml"), "-logFile", (Join-Path $LogDir "playmode.log")))
    }
    "test-player" {
        Invoke-Unity (@CommonArgs + @("-buildTarget", $Platform, "-runTests", "-testPlatform", $Platform, "-testResults", (Join-Path $ResultsDir ("player-{0}-results.xml" -f $Platform)), "-logFile", (Join-Path $LogDir ("player-{0}.log" -f $Platform))))
    }
    "build" {
        Invoke-Unity (@CommonArgs + @("-buildTarget", $Platform, "-executeMethod", "TestAutomation.BuildDevPlayer", "-logFile", (Join-Path $LogDir ("build-{0}.log" -f $Platform))))
    }
    "build-run" {
        Invoke-Unity (@CommonArgs + @("-buildTarget", $Platform, "-executeMethod", "TestAutomation.BuildAndRunDevPlayer", "-logFile", (Join-Path $LogDir ("build-run-{0}.log" -f $Platform))))
    }
    "run" {
        Invoke-Unity (@CommonArgs + @("-buildTarget", $Platform, "-executeMethod", "TestAutomation.BuildAndRunDevPlayer", "-logFile", (Join-Path $LogDir ("build-run-{0}.log" -f $Platform))))
    }
    "all" {
        & $PSCommandPath -Command test-edit -UnityEditorPath $UnityEditorPath -ProjectPath $ProjectPath
        & $PSCommandPath -Command test-play -UnityEditorPath $UnityEditorPath -ProjectPath $ProjectPath
        & $PSCommandPath -Command build-run -Platform $Platform -UnityEditorPath $UnityEditorPath -ProjectPath $ProjectPath
    }
}
