ARC Implementation:

General Packet Implementation:
	Some small details will apply to every type of packet in an RPC file. In order to implement a loader which conforms to the specification, these will need to be accounted for.
	
	Packet Header:
		Every packet in the RPC file contains a brief 8 byte header, with two very important properties:
			- A unique identifier ('META', 'DESC', 'SURF').
			- The size. (Size includes packet header length of 8 bytes)
	
		These allow for some very useful behaviour in RPC Implementations such as knowing if a packet is supported, and if not - 
		the possibility to skip it entirely. An RPC implementation **must** handle this gracefully, but _can_ allow a degree of user control in how it is handled.
		
		For example:
			An imaginary RPC file contains the packet "FOOB", and the packet size is '0x4152' - the loader doesn't have an implementation
			for this packet type. Depending on what the user wants - the loader could skip it by seeking forwards '0x4152 - 8' bytes, and continuing; or alternatively, the user could choose to throw an error if an unknown packet type is encountered.
	
	Alignment/Padding:
		RPC Packets can optionally be padded to any alignment. A loader implementation will have to take this into account when reading
		the packet stream. It is **NOT** acceptable for a loader to consume the known bytes of a packet, and assume it can move onto the
		next packet.
		
		In order to detect when a packet is aligned, the offset at which the packet header started should be retained - after the known
		packet fields have been read, this can be used to calculate a distance with between the current byte position, the retained position and the packet size stored in the header: 
			
			A = (headerStartPos - currentBytePos) 
			paddingSize = (packetSize - A)
		
		A loader implementation can then use 'paddingSize' to seek forwards, and be ready to read the next packet.
		
		Alignment is a useful feature for file formats to support if they are to be potentially used across many different CPU architectures; MIPSle for example favours a 4 byte alignment when reading values from RAM for 4 byte size values (words, ints).
		
		A writer implementation **MUST** implement user chosen alignment rules, and must write each packet in accordance with how it will be read as described above.					

Custom Packet Implementation:
	Custom packets _can_ be defined within an RPC file, but they **MUST** contain a generic header which tells an implementation what the source/itention of the custom packet is. Custom packets must also conform to the following when defining the packet header:
		- The packet ID/Tag **MUST** use uppercase letters.
		- The packet ID/Tag _can_ use numbers but they **MUST** encoded as UTF8/ASCII.
			- However, it **SHOULD NOT** only contain numbers.
		
		- The packet header size (8 bytes) **MUST** be included in the size.	
		- The size of the custom packet header (8 bytes) **MUST** be included in the size.
		- The size of any padding bytes **MUST** be included in the size.
	
	Inside the CustomPacketHeader is only a single 8 byte value which is to be used as an identifier for a company, software or even a special code. Should you make your own custom packet, you can optionally submit an issue to the RPC File Format github with the identifier (as a hex string, please) so it can be added to database of "owned" identifiers. You do not need to provide a specification
	of the data inside the custom packet, but you can do if you wish.
	
	The data of a custom packet can be anything you want; but it also has some restrictions that an implementation must obey:
		- The packet buffer size **MUST** obey the requested alignment.
	
	A custom packet which is considered 'open' (meaning you provided a specification of the data format) could be promoted into an extension if it can be considered generic enough and has a large enough userbase. This is to facilitate the growth of the RPC format.
	
	
The 'META' Packet.
Meta packets are an _optional_ packet type in RPC files which serve to provide useful context and information to an end user, or store bits of data (using the custom metatype) for format and program interchange (e.g: you can use a meta packet to store the user data area of a converted TGA file).

Meta packets have rules, which you must implement in order to conform to the specificiation:
	1) A program which imports a file in RPC format and then re-exports the file as RPC format must not strip any existing meta packets
	   which were already present in the file. Meta packet removal *MUST* be a user operation, although it can be program assisted
	   (E.G a 'strip meta packets' option on import/export)
	   
	2) The program **MUST** honour the flags set in each meta packet.
		- Expect any valid type.
		- Do not allow user edits of readonly packets. An expection is removal.
		- Do not allow program edits of retained packets. An exception is removal.
		
	3) The program **MUST** display each meta packet to the user, aside from type 'BUFF' packets - which may be hidden due to their general non human readable nature.
		
		
12.1 Buffer Compression Scheme:
The compression scheme for buffers in ARC uses a program defined; layered approach. 5 different layers of compression can be
applied to a buffer, from any of 64 possible compression types. Each compression layer can (if required) add data to a buffer, which will be used for decompression - E.G, huffman will store the code table in the buffer, ARC-RLE will store RLE configuration.

12.2 Buffer Compression Types:
	00 - 31: Lossless Compression Types:
		0 = No Compression
		1 = RLE
		2 = Huffman
		3 = Delta
	
	32 - 63: Lossy Compression Types:
		32 = Chroma Sub Sampling
		
	12.2.1 ARC RLE:
		RLE in ARC is a special operation.
	

