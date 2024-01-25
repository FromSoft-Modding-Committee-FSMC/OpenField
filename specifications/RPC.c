//
// RPC (RASTER PACKET CONTAINER) Format
// File Version 1, Spec Version 1
// Updated: 28/10/2023 22:46
//

//Format Definitions
#define RPC_FORMAT_TAG 0x66435052	//LE "RPCf"
#define RPC_FILE_VERSION 1
#define RPC_COMP_VERSION 1

//MetaPacketTag Definitions
#define RPC_PKTYPE_META 0x4154454D  //LE 'META'	- File Meta
#define RPC_PKTYPE_EXTN 0x4E545845	//LE 'EXTN'	- Format Extension
#define RPC_PKTYPE_DESC 0x43534544	//LE 'DESC'	- Image Descriptor
#define RPC_PKTYPE_BUFF 0x46465542  //LE 'BUFF'	- Image Buffer


//MetaType Definitions
//  The last bits of the METATYPE are used to set properties about the tag.
//  Bit 12 - 13: The meta value type (0 = Integer, 1 = Fractional, 2 = String, 3 = Buffer)
//  Bit 14 - 14: ReadOnly flag.
//  Bit 15 - 15: The meta must be retained if importing & re-exporting to RPC format.
#define RPC_METATYPE_CRUSER 0x0000	//The username of the RPC creator
#define RPC_METATYPE_EDUSER 0x0001	//The username of the last RPC editor
#define RPC_METATYPE_CRDATE 0x0002  //The date the RPC was created
#define RPC_METATYPE_EDDATE 0x0003  //The date the RPC was last edited
#define RPC_METATYPE_ORIGIN 0x0004	//The software that originally created the image
#define RPC_METATYPE_CUSTOM 0x0FFF	//Custom meta data.

//FormatFlags Definitions
#define RPC_FRMTFLAG_MULTIPLEXED 0x80000000
#define RPC_FRMTFLAG_INDEXED     0x40000000
#define RPC_CLRSPACE_RGB		 0x00000000

typedef struct 
{
	uint32_t formatTag;		//Format Tag (aka magic number)
	uint16_t fileVersion;	//The version of the format this file contains.
	uint16_t compVersion;	//The backwards compatibility version, for older loaders to try.
	uint32_t sizeOfFile;	//The total size of the file (in bytes)
	uint32_t packetCount;	//The number of packets contained in this file. Must not be zero.
} RPCHeader;

typedef struct 
{
	uint32_t tag;		//Packet Tag. 'EXTN', 'META', 'DESC', 'FRMT', 'BUFF', 'SURF', 'CLUT'
	uint32_t size;		//Size of the packet (bytes).
} RPCPacketHeader;

typedef struct 
{
	union 
	{
		struct //MSB to LSB
		{	
			uint16_t:1  retained;
			uint16_t:1  readonly;
			uint16_t:2  valueType;
			uint16_t:12 type;
		}
		uint16_t metaType;
	}
	uint16_t keyLength;		 //The length of the key field.
	char key[keyLength];	 //Variable size, depending on keyLength.
	uint16_t valueLength;	 //The length of the value field.
	byte value[valueLength]; //Variable size, depending on the type of meta.
} RPCPacketMeta;

typedef struct
{
	
	int32_t next;	//Index of the next surface (for cubemaps, volumes or mipmaps) 
} RPCPacketSurface;