
#
# Get External Dependancies
#

# FMTLIB
if(NOT FMTLIB_PRESENT)
  message("OpenField:CMake -> Requesting fmtlib from git repository...")

  # Fetch fmtlib
  set(EXTERNAL_INSTALL_LOCATION ${CMAKE_BINARY_DIR}/external)

  ExternalProject_Add(fmtlib
    GIT_REPOSITORY "https://github.com/fmtlib/fmt.git"
    CMAKE_ARGS -DCMAKE_INSTALL_PREFIX=${EXTERNAL_INSTALL_LOCATION}/fmtlib
  )

  include_directories(${EXTERNAL_INSTALL_LOCATION}/include)
  link_directories(${EXTERNAL_INSTALL_LOCATION}/lib)

  # Configure fmtlib
  set(FMT_DOC 0)
  set(FMT_TEST 0)
  set(FMT_FUZZ 0)

  # Finish

  set(FMTLIB_PRESENT 1)
else()
add_custom_target(fmtlib)
endif()

add_library(OpenField_Core)
add_dependencies(OpenField_Core fmtlib)
target_sources(OpenField_Core PRIVATE
  ./utility/logger.hpp
  ./utility/logger.cpp
  ./math/vector.hpp
)