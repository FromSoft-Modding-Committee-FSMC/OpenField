#include <stdio.h>
#include <stdint.h>
#include <memory>

#include "openfieldcore\math\vector.hpp"
#include "openfieldcore\utility\logger.hpp"

int main(int argc, char** argv)
{
    printf("%s", "Hello, you sexy GCC 12.2 installation\n");

    OFC::Logger::get()->Info("Hi there, smelly mother fucker! :3\0");
    OFC::Logger::get()->Warn("Warning m8\0");
    OFC::Logger::get()->Error("Error m8\0");

    auto OneVecXZ = OFC::Vector<float, 3>(1.f, 0.f, 1.f);
    auto OneVecY  = OFC::Vector<float, 3>(0.f, 1.f, 0.f);

    printf("%s", "Result of vector addition & casting: \n");
    OFC::Vector<int32_t, 3> vecInteger = OneVecXZ + OneVecY;
    OFC::Vector<float, 3> vecFloat = OneVecXZ + OneVecY; 

    printf("int vec: x:{%i}, y:{%i}, z:{%i}\n", vecInteger[VECX], vecInteger[VECY], vecInteger[VECZ]);
    printf("float vec: x:{%f}, y:{%f}, z:{%f}\n", vecFloat[VECX], vecFloat[VECY], vecFloat[VECZ]);

    printf("%s", "Result of direct vector set: \n");
    vecFloat[VECZ] = 20;

    printf("float vec z: {%f}\n", vecFloat[VECZ]);

    printf("%s", "Result of vector conversion (3 to 4): \n");

    OFC::Vector<float, 4> vec3to4 = OneVecXZ.To<4>();

    printf("vec3to2: (x:{%f}, y:{%f}, z:{%f}, w:{%f}\n", vec3to4[VECX], vec3to4[VECY], vec3to4[VECZ], vec3to4[VECW]);

    return 0;
}