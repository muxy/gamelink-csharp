mkdir build
cd build
cmake -Ax64 ../gamelink-cpp
cmake --build . --target cgamelink --config Release --parallel 8
copy "Release\cgamelink.dll" ..
cd ..