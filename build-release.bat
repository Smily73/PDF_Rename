@echo off
setlocal

echo ==========================
echo PDF Rename - Release Build
echo ==========================
echo.

REM Konfiguration
set "PROJECT_NAME=PDFRename"
set "PROJECT_DIR=%~dp0Source"
set "RELEASE_DIR=%~dp0Release"

REM Wechsle zum Projektverzeichnis
cd /d "%PROJECT_DIR%"

REM Prüfe ob Projektdatei existiert
if not exist "%PROJECT_NAME%.csproj" (
    echo FEHLER: Projektdatei %PROJECT_NAME%.csproj nicht gefunden!
    echo Stelle sicher, dass die Batch-Datei im Hauptverzeichnis liegt.
	echo Pfad: %PROJECT_DIR%
    pause
    exit /b 1
)

echo Erstelle Self-Contained Release Build...
echo.

REM Lösche vorherige Build-Artefakte
if exist "bin\Release" (
    echo Lösche vorherige Build-Artefakte...
    rmdir /s /q "bin\Release"
)

REM Erstelle Self-Contained Release für Windows x64
echo Baue Self-Contained Release (Windows x64)...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if %ERRORLEVEL% neq 0 (
    echo.
    echo FEHLER: Build fehlgeschlagen!
    pause
    exit /b 1
)

echo.
echo Build erfolgreich abgeschlossen!
echo.

REM Erstelle Release-Ordner falls nicht vorhanden
if not exist "%RELEASE_DIR%" (
    echo Erstelle Release-Ordner...
    mkdir "%RELEASE_DIR%"
)

REM Lösche alte Release-Dateien
if exist "%RELEASE_DIR%\%PROJECT_NAME%.exe" (
    echo Lösche alte Release-Dateien...
    del /q "%RELEASE_DIR%\*.*"
)

REM Kopiere die Self-Contained Dateien
echo Kopiere Release-Dateien nach %RELEASE_DIR%...
set "PUBLISH_DIR=bin\Release\net8.0-windows\win-x64\publish"

if exist "%PUBLISH_DIR%\%PROJECT_NAME%.exe" (
    copy "%PUBLISH_DIR%\*.*" "%RELEASE_DIR%\"
    echo.
    echo ====================================
    echo Release Build erfolgreich erstellt!
    echo ====================================
    echo.
    echo Dateien befinden sich in: %RELEASE_DIR%
    echo.
    dir "%RELEASE_DIR%"
) else (
    echo FEHLER: Publish-Verzeichnis nicht gefunden!
    echo Erwartet: %PUBLISH_DIR%
    pause
    exit /b 1
)

echo.
echo Moechten Sie die Release-Anwendung starten? (J/N)
set /p choice=
if /i "%choice%"=="J" (
    echo Starte %PROJECT_NAME%.exe...
    start "" "%RELEASE_DIR%\%PROJECT_NAME%.exe"
)

echo.
echo Fertig!
pause
