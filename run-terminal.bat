@echo off
cd /d "%~dp0"
start cmd /k "dotnet run --project src/env0.terminal.console"
