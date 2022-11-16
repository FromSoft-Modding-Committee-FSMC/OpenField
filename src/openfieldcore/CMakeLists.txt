add_library(OpenField_Core)

# Core Source
target_sources(OpenField_Core PRIVATE
  ./utility/logger.hpp
  ./utility/logger.cpp
  ./math/vector.hpp
)

# Core Include Locations
target_include_directories(OpenField_Core 
  PUBLIC /../../external/fmt/include
  PUBLIC /../../external/glad2/include
  PUBLIC /../../external/openalsoft/include
  PUBLIC /../../external/sdl2/include
)

# Core Library Locations
target_link_directories(OpenField_Core
  PRIVATE external/fmt
  PRIVATE external/openalsoft
  PRIVATE external/sdl2
)

# Core Libraries
target_link_libraries(OpenField_Core
  fmt
  OpenAL
  SDL2-static
)