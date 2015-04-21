@echo off
set file=%~dp0output.exe

call concat.bat
call csc output.cs /optimize /nologo
cd ../tester
call "run-tests.bat" %file%