//
// MPC (MULTIPLE PACKET CONTAINER) File Format
// File Version 3, Spec Version 5

//Format Definitions
#define MPC_VERSION 3
#define MPC_COMPVER 2

//Packet Definitions
#define MPC_PKT_VBNIFAST "VBFL"		//Fast loading Vertex Buffer, no indices or anything.
#define MPC_PKT_TEXTUREF "TEXR"		//Texture Reference. Stores exclusively a filename (+ extension)
#define MPC_PKT_TEXTURED "TEXD"		//Texture Data. Actual colour data.
#define MPC_PKT_COLLMESH "VBCL"		//Collision Mesh Buffer
#define MPC_PKT_VTXABASE "VBMB"		//Vertex Morph Base
#define MPC_PKT_VTXAPOSE "VBMP"		//Vertex Morph Frame/Pose
#define MPC_PKT_MATERIAL "MTRL"		//A Material packet. Deprecated.
#define MPC_PKT_USERDATA "USER"		//A User Packet. Stores non format defined data.
#define MPC_PKT_TEXTNOTE "TXTN"		//Simple text packet. Could be used for copyright notices.
#define MPC_PKT_TEXTTABL "TXTT"		//More advanced text packet, using a key/data type storage.
#define MPC_PKT_ENDFILEP "ENDF"		//End of File packet. dataCount and dataPerSize should be 0 when used.

/** MPC Header	(Length = 16 Bytes)
 * Stores basic information about the MPC file
**/
typedef struct
{
	uint32_t formatId;		//MPCf
	uint16_t version;		//Version of the MPC File.
	uint16_t compVersion;		//For loader compatibility. An older load will be able to mostly read the MPC, so long as 
	uint32_t numPacket;		//Number of packets inside the MPC
	uint32_t fileSize;		//Total size of the MPC.
} MPCHeader;


/** MPC Packet Header (Length = 12 Bytes)
 * One per MPC packet type in order to define what sort of data is about to come.
**/
typedef struct
{
	char     packetId[4];	//MPC packet id
	uint32_t packetLength;	//entire size of the following data.
	uint16_t dataCount;	//Number of data structures inside this packet. All types will be equal to that of the packetId. I.E. 7 Materials
	uint16_t dataPerSize;	//When not storing variable size packets, this is equal to the size of a single packet. Otherwise, it is 0.
}  MPCPacketHeader;


/** MPC Material (Length = 48 Bytes, Packet ID = 'MATERIAL') 
 * To be revised.
**/
typedef struct
{
	char      materialName[28];		//Name of the material
	uint32_t  textureId;			//Texture Id
	float     diffuseColour[3];		//RGB of the diffuse colour
	float     opacity;			//Material Opacity
} MPCMaterial;


/** MPC Collision Mesh (Length = N Bytes, Packet ID = MPC_PKT_COLLMESH) **/
typedef struct
{
	uint32_t numVertex;
	
	typedef struct
	{
		float Position[3];
	}  MPCCollisionVertex;
	
	MPCCollisionVertex collisionMesh[numVertex];
} MPCCollisionMesh;


/** MPC Texture File Reference (Length = 64 Bytes, PacketID = MPC_PKT_TEXTUREF)
 * stores only the file name of a texture
**/
typedef struct
{
	char TextureName[64];
}  MPCTextureReference;

  
/** MPC Texture Data (Length = N Bytes, PacketID = MPC_PKT_TEXTURED)
 * stores only the file name of a texture
**/
typedef struct
{
	/* MPCTextureData Types */
	typedef struct {	//A 1-Dimensional array type, containing colours with simple compression. Length = 8 + N bytes
		uint8_t componentCount;		//How many components are in each colour, RGBA = 4, R = 1.
		uint8_t bitsPerComponent;	//How many bits are used to store each component
		uint8_t componentFormat;	//How the components are to be decoded, Ex: 0 = byte, 1 = word ... 6 = float, etc
		uint8_t compressionMethod;	//How each colour is compressed, Ex: 0 = None, 1 = RLE, etc
		uint32_t bitCount;		//Total bit size of the palette. Divide by 8 and round up to get the actual size of the buffer.

		uint8_t paletteBuffer[ceil(bitCount >> 3)];		
	} MPCTexturePalette;

	typedef struct {	//A 2-Dimensional array type, containing either palette indices or colours.
		uint8_t componentCount;		//How many components are in each colour or index, RGBA = 4, R = 1.
		uint8_t bitsPerComponent;	//How many bits are used to store each component
		uint8_t componentFormat;	//How the components are to be decoded, Ex: 0 = byte, 1 = word ... 6 = float, etc
		uint8_t compressionMethod;	//How each colour/index is compressed, Ex: 0 = None, 1 = RLE, etc
		uint32_t bitCount;			//Total bit size of the plane. Divide by 8 and round up to get the actual size of the buffer.
		uint32_t flags;				//Flags controlling how the plane is to be handled. For ex, 0x00000001 would be an indexed image.
		
		uint8_t planeBuffer[ceil(bitCount >> 3)];		
	} MPCTexturePlane;

	/* MPCTextureData */
	char     TextureName[16];
	uint16_t numPalette;
	uint16_t numPlane;
	
	MPCTexturePalette palettes[numPalette];
	MPCTexturePlane planes[numPlane];
}  MPCTextureData;


/** MPC Fast Vertex Buffer (Length = 32 + N Bytes, PacketID = MPC_PKT_VBNIFAST)
 * The 'fast' vertex buffer is intended for direct loading into GPU memory, without repeated File/IO requirements.
**/
typedef struct
{
	char MeshName[16];
	uint numVertex;
	uint materialId;
	uint vertexSize;	//Size of vertex (always 48)
	uint vertexFormat;	//Type of vertex (always 0)
 	
	typedef struct
	{
		float Position[3];
		float Normal[3];
		float Texcoord[2];
		float Colour[4];
	}  MPCFastVertex;
	
	MPCFastVertex MeshData[numVertex];
} MPCFastVB;


/** MPC Vertex Morph Base (Length = 32 + N Bytes, PacketID = MPC_PKT_VTXABASE)
 * Colours and texture coordinates are not generally interpolated, so they don't need to be included in the poses. This stores those.
**/
typedef struct
{
	char BaseName[16];
	uint numVertex;
	uint materialId;
	uint vertexSize;	//Size of the vertex. Always 24.
	uint vertexFormat;	//Always 0.
	
	typedef struct
	{
		float Texcoord[2];
		float Colour[4];
	} MPCMorphBaseVertex;
	
	MPCMorphBaseVertex BaseData[numVertex];
}  MPCMorphBase;


/** MPC Vertex Morph Pose (Length = 8 + N Bytes, PacketID = MPC_PKT_VTXAPOSE)
 * Stores whole positions and normals for a vertex frame.
**/
typedef struct
{
	uint baseId;		//Index of the MPCMorphBase this pose belongs too
	uint numVertex;		//Number of vertices
	
	typedef struct
	{
		float Position[3];
		float Normal[3];
	} MPCMorphPoseVertex;
	
	MPCMorphPoseVertex PoseData[numVertex];
} MPCMorphPose;