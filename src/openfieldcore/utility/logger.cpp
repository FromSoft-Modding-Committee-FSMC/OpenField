#include "logger.hpp"

#include <ctime>

namespace OFC
{
    void Logger::Log(const char* tag, std::string message)
    {
        #ifdef _OFC_LOGGER_ENABLED

        std::time_t time = std::time(nullptr);
        tm* localtime = std::localtime(&time);

        std::string fmtMessage = fmt::format("[{:02}:{:02}:{:02}]-[{}]: {}\0", localtime->tm_hour, localtime->tm_min, localtime->tm_sec, tag, message);

        fmt::print("{}\n", fmtMessage);

        #endif
    }

    void Logger::Info(std::string message)
    {
        Log("Info\0", message);
    }

    void Logger::Warn(std::string message)
    {
        Log("Warn\0", message);
    }

    void Logger::Error(std::string message)
    {
        #ifdef _OFC_LOGGER_PROFANITY
        Log("Shit\0", message);
        #else
        Log("Error\0", message);
        #endif
    }
}