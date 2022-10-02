#ifndef _OFC_LOGGER_HPP_
#define _OFC_LOGGER_HPP_

#include <memory>
#include <string>
#include <fmt\format.h>

#define _OFC_LOGGER_ENABLED
#define _OFC_LOGGER_PROFANITY

namespace OFC 
{
    class Logger
    {
        public:
            Logger() {}

            //Stop CPP generating these.
            Logger(Logger const&) = delete;
            Logger& operator=(Logger const&) = delete;

            void Log(const char* tag, std::string message);

            /** @brief Log an error to both file and console.
             */
            void Error(std::string message);

            /** @brief Log a warning to both file and console.
             */
            void Warn(std::string message);

            /** @brief Log info to both file and console.
             */
            void Info(std::string message);

            /** @brief Get a singleton instance of the logger 
             */
            static std::shared_ptr<Logger> get()
            {
                static std::shared_ptr<Logger> logger(new Logger);
                return logger;
            }
    };
}

#endif