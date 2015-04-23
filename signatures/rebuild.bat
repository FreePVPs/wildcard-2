@echo off
cd builder
call compile.bat
cd ..
call "builder\builder.exe" "builder\example.cs" < using-signatures.txt > output.cs
pause