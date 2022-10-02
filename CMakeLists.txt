cmake_minimum_required(VERSION 3.24.2)
project(OpenField)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_executable(Open-Field-Test)
target_sources(Open-Field-Test PRIVATE
    src/openfieldcore/utility/logger.hpp
    src/openfieldcore/utility/logger.cpp
    src/openfieldcore/math/vector.hpp
    src/main.cpp)

# Open Field Core Library
#add_library(OpenField-Core)
#target_sources(OpenField-Core PRIVATE
#    ./src/openfieldcore/utility/logger.hpp
#    ./src/openfieldcore/utility/logger.cpp
#    ./src/openfieldcore/math/vector.hpp)

# Open Field Forge Library
# add_library(Open-Field-Forge)

# Open Field Editor
# add_executable(Open-Field-Editor)

# Open Field Runner
# add_executable(Open-Field-Runner)