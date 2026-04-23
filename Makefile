UNITY_EDITOR ?= unity
PROJECT_PATH ?= $(CURDIR)
PLATFORM ?= StandaloneLinux64

ARTIFACTS_DIR := $(PROJECT_PATH)/Artifacts
LOG_DIR := $(ARTIFACTS_DIR)/Logs
RESULTS_DIR := $(ARTIFACTS_DIR)/TestResults

UNITY_COMMON := -batchmode -quit -projectPath "$(PROJECT_PATH)"

.PHONY: help test-edit test-play test-player build build-run run all

help:
	@printf '%s\n' \
		"Targets:" \
		"  test-edit    Run EditMode tests" \
		"  test-play    Run PlayMode tests in the editor" \
		"  test-player  Run PlayMode tests in a built player" \
		"  build        Build a development player" \
		"  build-run    Build and auto-launch a development player" \
		"  run          Alias for build-run" \
		"  all          Run edit tests, play tests, and build-run"

test-edit:
	mkdir -p "$(LOG_DIR)" "$(RESULTS_DIR)"
	"$(UNITY_EDITOR)" $(UNITY_COMMON) -runTests -testPlatform EditMode -testResults "$(RESULTS_DIR)/editmode-results.xml" -logFile "$(LOG_DIR)/editmode.log"

test-play:
	mkdir -p "$(LOG_DIR)" "$(RESULTS_DIR)"
	"$(UNITY_EDITOR)" $(UNITY_COMMON) -runTests -testPlatform PlayMode -testResults "$(RESULTS_DIR)/playmode-results.xml" -logFile "$(LOG_DIR)/playmode.log"

test-player:
	mkdir -p "$(LOG_DIR)" "$(RESULTS_DIR)"
	"$(UNITY_EDITOR)" $(UNITY_COMMON) -buildTarget $(PLATFORM) -runTests -testPlatform $(PLATFORM) -testResults "$(RESULTS_DIR)/player-$(PLATFORM)-results.xml" -logFile "$(LOG_DIR)/player-$(PLATFORM).log"

build:
	mkdir -p "$(LOG_DIR)"
	"$(UNITY_EDITOR)" $(UNITY_COMMON) -buildTarget $(PLATFORM) -executeMethod TestAutomation.BuildDevPlayer -logFile "$(LOG_DIR)/build-$(PLATFORM).log"

build-run:
	mkdir -p "$(LOG_DIR)"
	"$(UNITY_EDITOR)" $(UNITY_COMMON) -buildTarget $(PLATFORM) -executeMethod TestAutomation.BuildAndRunDevPlayer -logFile "$(LOG_DIR)/build-run-$(PLATFORM).log"

run: build-run

all: test-edit test-play build-run
