#include "inputstream.hpp"

#include "logger.hpp"

namespace OFC
{
    InputStream::InputStream(const char* filepath, uint32_t flags)
    {
        fstream.open(filepath, std::ios::in|std::ios::binary|std::ios::ate);
        fstreamSize = fstream.tellg();
        fstream.seekg(0, std::ios::beg);
    }

    bool InputStream::FillByteBuffer()
    {
        //Get distance between eof and current pos
        //if it is less than buffer size, read that amount and set bufferedBytes to the same value
        //otherwise read defined buffersize
        //
        uint32_t remainingBytes = fstreamSize - fstream.tellg();

        if(remainingBytes <= 0)
        {
            Logger::get()->Warn("File has reached end. Buffer cannot be filled!");
            return false;
        }

        if(remainingBytes < _OFC_INPUTSTREAM_BUFFERSIZE)
        {  
            fstream.read(byteBuffer,  remainingBytes);
            bufferedBytes =  remainingBytes;
            return true;
        }

        fstream.read(byteBuffer, _OFC_INPUTSTREAM_BUFFERSIZE);
        bufferedBytes = _OFC_INPUTSTREAM_BUFFERSIZE;
        return true;
    }

    uint8_t InputStream::GetByte()
    {
        if(bufferedBytes <= 0)
        {
            //Buffer has run dry, and must be repopulated
            FillByteBuffer();
        }

        bufferedBytes--;
        return byteBuffer[_OFC_INPUTSTREAM_BUFFERSIZE - (bufferedBytes + 1)];
    }

    void InputStream::FillBitBuffer()
    {
        bitBuffer = GetByte();
        bufferedBits = 8;
    }

    uint8_t InputStream::GetBit()
    {
        if(bufferedBits <= 0)
        {
            //Buffer has run dry, and must be repopulated
            FillBitBuffer();
        }

        uint8_t ret = bitBuffer & 1;
        bitBuffer = bitBuffer >> 1;
        bufferedBits--;
        return ret;
    }

    uint8_t InputStream::ReadU8()
    {
        return static_cast<uint8_t>(GetByte());
    }

    uint16_t InputStream::ReadU16()
    {
        return static_cast<uint16_t>(((GetByte() << 0) | (GetByte() << 8)) & 0xFFFF);
    }

    uint32_t InputStream::ReadU32()
    {
        return static_cast<uint32_t>(((GetByte() << 0) | (GetByte() << 8) | (GetByte() << 16) | (GetByte() << 24)) & 0xFFFFFFFF);
    }

    int8_t InputStream::ReadS8()
    {
        return static_cast<int8_t>(GetByte());
    }

    int16_t InputStream::ReadS16()
    {
        return static_cast<int16_t>(((GetByte() << 0) | (GetByte() << 8)) & 0xFFFF);
    }

    int32_t InputStream::ReadS32()
    {
        return static_cast<int32_t>(((GetByte() << 0) | (GetByte() << 8) | (GetByte() << 16) | (GetByte() << 24)) & 0xFFFFFFFF);
    }

    bool InputStream::ReadBit()
    {
        return GetBit() & 0x1;
    }

    uint32_t InputStream::ReadUBits(uint8_t count)
    {
        uint64_t temp = 0;

        for(uint8_t i = 0; i < (count & 0x1F); ++i)
        {
            temp |= ReadBit();
            temp = temp << 1;
        }
        temp = temp >> 1;

        return temp & 0xFFFFFFFF;
    }
}