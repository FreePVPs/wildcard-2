@echo off
cd concatination
call compile.bat
cd ..
call concatination\concat-all < concatination\concat-list.txt > output.cs
echo Complete!
pause