@echo off

setlocal

echo "-|-- Open Field: Building Project --|-" 
cd %0\..\
cmake -G "Unix Makefiles" -S %cd%/ -B %cd%\build\

cd %0\..\build\
make -f "%cd%\Makefile"

endlocal
exit /B