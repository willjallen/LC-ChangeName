@echo off
SETLOCAL

:: Define the destination directory
SET "DEST_DIR=dist"

:: Create the destination directory if it does not exist
IF NOT EXIST "%DEST_DIR%" (
    mkdir "%DEST_DIR%"
)

:: Copy each file
COPY "icon.png" "%DEST_DIR%"
COPY "manifest.json" "%DEST_DIR%"
COPY "README.md" "%DEST_DIR%"
COPY "CHANGELOG.md" "%DEST_DIR%"
COPY "ChangeName\bin\Debug\netstandard2.1\ChangeName.dll" "%DEST_DIR%"

echo Done.
ENDLOCAL
