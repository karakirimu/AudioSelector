@echo off

cd /d %~dp0

call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvarsx86_amd64.bat"

set /p version="Enter the version (e.g., 1.0.0): "

dotnet publish .\AudioSelector\AudioSelector.csproj -c Release --self-contained -r win-x86 -o ./AudioSelector_%version%_x86 -p:Platform=x86;PublishSingleFile=true;Version=%version%
dotnet publish .\AudioSelector\AudioSelector.csproj -c Release --self-contained -r win-x64 -o ./AudioSelector_%version%_x64 -p:Platform=x64;PublishSingleFile=true;Version=%version%
dotnet publish .\AudioSelector\AudioSelector.csproj -c Release --self-contained -r win-arm64 -o ./AudioSelector_%version%_ARM64 -p:Platform=arm64;PublishSingleFile=true;Version=%version%

echo Deleting all .pdb files ...
del /s /q "./AudioSelector_%version%_x86\*.pdb"
del /s /q "./AudioSelector_%version%_x64\*.pdb"
del /s /q "./AudioSelector_%version%_ARM64\*.pdb"

echo Copy misc files ...
copy ".\README.md" "./AudioSelector_%version%_x86\README.md"
copy ".\README.md" "./AudioSelector_%version%_x64\README.md"
copy ".\README.md" "./AudioSelector_%version%_ARM64\README.md"
copy ".\LICENSE" "./AudioSelector_%version%_x86\LICENSE"
copy ".\LICENSE" "./AudioSelector_%version%_x64\LICENSE"
copy ".\LICENSE" "./AudioSelector_%version%_ARM64\LICENSE"

echo Compressing ...
powershell Compress-Archive -Path "./AudioSelector_%version%_x86" -DestinationPath "./AudioSelector_%version%_x86.zip"
powershell Compress-Archive -Path "./AudioSelector_%version%_x64" -DestinationPath "./AudioSelector_%version%_x64.zip"
powershell Compress-Archive -Path "./AudioSelector_%version%_ARM64" -DestinationPath "./AudioSelector_%version%_ARM64.zip"

echo Publish complete.

pause .