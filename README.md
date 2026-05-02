# Beanstalk Survivors Unity Dev

Makefile helpers are provided for local Unity development. Use Unity `6000.3.5f2` for this project.

## Configuration

- Unity editor command defaults to `UNITY_EDITOR=unity`.
- Override it when needed: `make test-edit UNITY_EDITOR=/path/to/Unity`.
- Build/test platform defaults to `PLATFORM=StandaloneLinux64`.
- Override it when needed: `make build PLATFORM=StandaloneWindows64`.

## Quick Playtest

- `make run` or `make build-run` builds and launches a development player.
- In the playtest build, press `F1` for the overlay and `F5` to reload the scene.

## Builds

- `make build` creates a development player.
- Build outputs go under `Builds/Playtest/<platform>/`.
- Build logs go under `Artifacts/Logs/`.

## Tests

- `make test-edit` runs EditMode tests.
- `make test-play` runs PlayMode tests in the editor.
- `make test-player` runs PlayMode tests in a built player.
- Test results go under `Artifacts/TestResults/`.
- Test logs go under `Artifacts/Logs/`.

## All In One

- `make all` runs EditMode tests, PlayMode tests, then builds and launches the development player.
