@echo off
title NEXUS - Expedicion Eos
echo ===================================================
echo   NEXUS: EXPEDICION EOS - Compilando y ejecutando
echo ===================================================
cd /d "%~dp0NEXUS"
dotnet run --configuration Release
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Asegurate de tener instalado .NET 8 SDK Desktop.
    pause
)
