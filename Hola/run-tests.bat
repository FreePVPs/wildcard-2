@echo off
set file=%~dp0output.exe

cd "../signatures"
call "rebuild.bat"
cd "../Hola"

call concat.bat
call csc /o+ /nologo output.cs
cd ../tester
call "run-tests.bat" %file%