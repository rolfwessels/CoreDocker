@echo off
rem Helper script for those who want to run psake from cmd.exe
rem Example run from cmd.exe:
rem psake "BuildHelloWord" "4.0" 

if '%1'=='/?' goto help
if '%1'=='-help' goto help
if '%1'=='-h' goto help

powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0\testall.ps1' %*;"
exit /B %errorlevel%


