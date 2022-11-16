#ifndef _OFC_INPUTSTREAM_HPP_
#define _OFC_INPUTSTREAM_HPP_

#include <stdint.h>
#include <iostream>
#include <fstream>

#define _OFC_INPUTSTREAM_BUFFERSIZE 2048
#define _OFC_INPUTSTREAM_ENDIANBE (1 << 0)

namespace OFC
{
    class InputStream
    {
        private:
            //Input Buffering
            char byteBuffer[_OFC_INPUTSTREAM_BUFFERSIZE];
            uint16_t bufferedBytes = 0;
            uint8_t bitBuffer;
            uint8_t bufferedBits = 0;

            std::ifstream fstream;
            uint32_t fstreamSize;

            bool FillByteBuffer();
            void FillBitBuffer();
            uint8_t GetByte();
            uint8_t GetBit();
        public:
            InputStream(const char* filepath, uint32_t flags) { }

            uint8_t  ReadU8();
            uint16_t ReadU16();
            uint32_t ReadU32();
            int8_t   ReadS8();
            int16_t  ReadS16();
            int32_t  ReadS32();
            bool     ReadBit();
            uint32_t ReadUBits(uint8_t count);

    };
}

#endif