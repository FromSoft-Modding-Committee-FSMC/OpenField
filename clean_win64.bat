@echo off
setlocal

echo "-|-- Open Field: Cleaning Project --|-"
cd %0\..\
echo "Deleting directory '%cd%\build\'"
rd /S %cd%\build\

endlocal
exit /B