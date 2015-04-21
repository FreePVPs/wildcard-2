@echo off
call csc source/checker.cs /out:testing/checker.exe /nologo
call csc source/tester.cs /out:testing/tester.exe /nologo
call csc source/invoker.cs /out:testing/invoker.exe /nologo