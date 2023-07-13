//
// APE (A PICTURE EXISTS) File Format
// File Version 1, Spec Version 1

//Format Definitions
#define APE_MAGIC 0x66455041	//APEf
#define APE_VERSION 1
#define APE_COMPVER 1

//Descriptor Flags
#define APE_DFTYPE_STRIP  0x00000001 //One dimensional   texture
#define APE_DFTYPE_PLANE  0x00000002 //Two dimensional   texture
#define APE_DFTYPE_VOLUME 0x00000004 //Three dimensional texture
#define APE_DFTYPE_CUBE	  0x00000008 //6 sided cube texture

//LinearMap
//PlanarMap
//VolumeMap


/**
 * RLE Encoding:
 *   Packet Types:
 *     0b01nnnnnn = Copy next '1 + n' colours
 *     0b10nnnnnn = Repeat next colour '1 + n' times
 *     0b00000000 = End RLE
 *	   0b11nnnnnn = Extended size mode. Take 'n' and read another RLE packet, bit shift in LE order.
 *       A max of 4 packets can be combined, for a total max value of 1 + 16777215
**/


typedef struct {
	uint32_t formatId;		//APEf - 'APE_MAGIC'
	uint16_t version;		//The version of this file.
	uint16_t compVersion;	//Last compatible loader version.
	uint32_t numDescriptor;	//A descriptor contains information about a plane, volume...
	uint32_t fileSize;		//Length of the file in bytes
} APEHeader;

typedef struct {
	uint32_t descriptorFlags;
} APEDescriptor;