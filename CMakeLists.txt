cmake_minimum_required(VERSION 3.24.2)

project(OpenField)
include(ExternalProject)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_subdirectory(./src/openfieldcore/)

# Test Project
add_executable(OpenField_Test)
target_sources(OpenField_Test PRIVATE src/main.cpp)
target_link_libraries(OpenField_Test OpenField_Core)