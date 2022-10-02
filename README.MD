# Open Field
A multi project repository, creating a modern King's Field (Jp I, II, III) engine implementation.

# Build Requirements (Windows)
1. CMake (at least version 3.24.2)
2. A CPP compiler, that implements c++20

# Goals and Aim
The aim is to make a functional engine implementation for KF (and SOM, ST) using modern cpp (with minimal library use), which can be used to play these games on modern operating systems and hardware. For that to happen, the following sub projects must be completed...

###### Open Field Core
_Open Field Core_ will handle the bulk of complex work including graphics routines, math and audio.

###### Open Field Editor
_Open Field Editor_ will provide an editor (similar to the SOM Editor in scope) in order to be able to create a KF game.

###### Open Field Runtime
_Open Field Runtime_ will implement the components of Open Field Core in order to create an enviroment capable of playing games created and converted (see _Open Field Forge_).

###### Open Field Forge
_Open Field Forge_ will implement advanced conversion and transformation algorithms, in order to transfer as much of the original game data as possible into formats more suitable for the modern age. This will hopefully enable a user of the Open Field Editor open their copy of a targeted game or tool, and convert it into project files for the aformentioned editor.
This will require a lot of user input, which will be also provided by this subproject project - including scripts to drive the transformations, and data sets that be used to restore parts of files which are nessesarry for conversion (such as the sample rate of PSX VB file samples). Any data which cannot be converted (such as MIPS machine code) can be replaced with a counter part (such as a LUA script).
