REM Modifications Copyright Rich Newman 2017
@echo off

if exist %~dp0..\src\WebLinter\Node\node_modules.7z goto:node_modules_done

pushd %~dp0..\src\WebLinter\Node

echo Installing packages...
call npm install ^
     tslint@5.4.2 ^
     typescript@2.3.4 ^
     --no-optional --quiet > nul


echo Deleting unneeded files and folders...
del /s /q *.html > nul
del /s /q *.markdown > nul
del /s /q *.md > nul
del /s /q *.npmignore > nul
del /s /q *.txt > nul
del /s /q *.yml > nul
del /s /q .gitattributes > nul
del /s /q CHANGELOG > nul
del /s /q CHANGES > nul
del /s /q CNAME > nul
del /s /q README > nul

for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (bench)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (docs)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tests)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d" > nul

echo Compressing artifacts and cleans up...
"%~dp07z.exe" a -r -mx9 node_modules.7z node_modules > nul
rmdir /S /Q node_modules > nul

:node_modules_done
if exist %~dp0..\src\WebLinter\Node\edge.7z goto:done

pushd %~dp0..\src\WebLinter\Node

"%~dp07z.exe"  a -r -mx9 edge.7z ..\..\..\packages\Edge.js.7.10.1\content\edge > nul

:done
echo Done
pushd "%~dp0"