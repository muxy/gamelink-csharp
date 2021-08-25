@ECHO off
git submodule update --init --recursive

REM Clear out the build folder
rmdir /s /q build
mkdir build 
cd build
cmake -Ax64 ../gamelink-cpp 
cmake --build . --target cgamelink --config Release --parallel 8
copy /y "Release\cgamelink.dll" "..\cgamelink.dll"
cd ..
