
#include "..\openfieldcore\utility\logger.hpp"

std::shared_ptr<OFC::Logger> logger;

void ShowHelp()
{
    logger->Log("Help", "Usage: offorge.exe [args]");
    logger->Log("Help", "Arguments: ");
    logger->Log("Help", "  -i 'file'   : An input file to convert.");
    logger->Log("Help", "  -o 'file'   : Output format");
    logger->Log("Help", "  -h          : This menu");
    logger->Log("Help", "  -pn         : Enable 'pretty names'");
}

void ShowTitle()
{
    logger->Info("--|-> Open Field Forge <-|--");
}

int main(int argc, char** argv)
{
    logger = OFC::Logger::get();

    ShowTitle();

    if(argc <= 1)
    {
        ShowHelp();
        return 0;
    }

    char* inFile  = NULL;
    char* outFile = NULL;

    //Parse Arguments
    for(int i = 1; i < argc; ++i)
    {   
        if(strncmp(argv[i], "-h\0", 3) == 0)
        {
            logger->Info("Showing Help...");
            ShowHelp();
            return 0;
        }else
        if(strncmp(argv[i], "-f\0", 3) == 0)
        {
            if((i + 1) >= argc)
            {
                logger->Error("Invalid use of '-f' flag! Must be followed by a filepath!!!");
                ShowHelp();
                return 0;
            }

            inFile = argv[i + 1];
            i++;

            logger->Info("Set Input file to: ");
            logger->Info(inFile);

            continue;
        }else
        if(strncmp(argv[i], "-o\0", 3) == 0)
        {
            if((i + 1) >= argc)
            {
                logger->Error("Invalid use of '-o' flag! Must be followed by a filepath!!!");
                ShowHelp();
                return 0;
            }

            outFile = argv[i + 1];
            i++;

            logger->Info("Set output file to: ");
            logger->Info(outFile);

            continue;           
        }else
        {
            logger->Error("Invalid flag!");
            logger->Log("BadFlag", argv[i]);
            return 0;
        }
    }

    if(inFile == NULL)
    {
        logger->Error("No Input File!");
        ShowHelp();
        return 0;
    }

    //Set an output file if non was explicitly set
    if(outFile == NULL)
    {
        outFile = inFile;
        logger->Info("Set output file to: ");
        logger->Info(outFile);
    }
}