cmake_minimum_required(VERSION 3.24.2)

project(OpenField)
include(ExternalProject)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

#
# External Dependancies from git submodules
#

# FMTLIB
add_subdirectory(external/fmt)
set(FMT_DOC OFF)
set(FMT_TEST OFF)

#
# Dependancies
#
add_subdirectory(src/openfieldcore)

# Test Project
add_executable(OpenField_Test)
target_sources(OpenField_Test PRIVATE src/main.cpp)
target_link_libraries(OpenField_Test OpenField_Core)