#include "openfieldcore\math\vector.hpp"
#include "openfieldcore\utility\logger.hpp"

int main(int argc, char** argv)
{
    std::shared_ptr<OFC::Logger> log = OFC::Logger().get();

    OFC::Vector<float, 2> vec2D1 = OFC::Vector<float, 2>(1.f, 1.f);

    log->Warn("Test Vector Cast...");
    OFC::Vector<int32_t, 2> vecCastTest = vec2D1;
    assert(vecCastTest[VECX] == 1 && vecCastTest[VECY] == 1);
    OFC::Vector<float, 3> vecToTest = vec2D1.To<3>();

    log->Info("Test Success!");

    log->Warn("Test Vector Add...");
    OFC::Vector<float, 2> vecAddTest = vec2D1 + vec2D1;

    assert(vecAddTest[VECX] == 2.f && vecAddTest[VECY] == 2.f);

    log->Info("Test Success!");







}
