//
// ZNE (Zone) Binary File Format
// File Version 1, Spec Revision 1
//

struct ZoneHeader
{
	ulong tag;    		//'ZONEbf[0D][0A]'
	uint version; 		//1
	uint filesize;		//reserved
}

//
// Tilemap
//

typedef struct
{
	u32 tag;		//'TMAP'
	u16 xorigin;	//X Origin of the tilemap
	u16 yorigin;	//Y Origin of the tilemap
	u16 hsize;		//Width of the tilemap
	u16 vsize;		//Height of the tilemap
	u16 numLayer;	//Number of layers in the tilemap. Max 8
	u16 numOverlay;	//Number of overlays in the tilemap.
} ZoneTilemap;

typedef struct
{
	u16 elevation.	//Where each 100 = 1 metre. Min = -32000, Max = 32000
	
} ZoneTilemapLayer;