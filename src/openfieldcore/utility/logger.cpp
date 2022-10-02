#include "logger.hpp"

#include <iostream>
#include <ctime>

namespace OFC
{
    void Logger::Log(const char* tag, const char* colour, const char* message)
    {
        #ifdef _OFC_LOGGER_ENABLED

        std::time_t time = std::time(nullptr);
        tm* localtime = std::localtime(&time);

        char timeCBuffer[32];

        //TO-DO: Replace with std::format when avalible, or {fmt} library as suggested.
        snprintf(timeCBuffer, sizeof(timeCBuffer), "%i:%i:%i", localtime->tm_hour, localtime->tm_min, localtime->tm_sec);
        snprintf(messageBuffer, _OFC_LOGGER_BUFFSIZE, "[%s]-[%s]: %s\0", timeCBuffer, tag, message);

        std::cout << messageBuffer << "\n";

        #endif
    }

    void Logger::Info(std::string message)
    {
        Log("Info\0", "91m\0", message.c_str());
    }

    void Logger::Warn(std::string message)
    {
        Log("Warn\0", "93m\0", message.c_str());
    }

    void Logger::Error(std::string message)
    {
        #ifdef _OFC_LOGGER_PROFANITY
        Log("Shit\0", "91m\0", message.c_str());
        #else
        Log("Error\0", "91m\0", message.c_str());
        #endif
    }
}