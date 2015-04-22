@echo off
set file=%~dp0output.exe

call concat.bat
call csc /o+ /nologo output.cs
cd ../tester
call "run-tests.bat" %file%