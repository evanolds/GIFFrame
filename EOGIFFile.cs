// Author:          Evan Olds
// Creation Date:   June 13, 2008

using System;
using System.IO;
using System.Text;

internal class EOGIFFile
{
    private const int EMPTY_PREFIX = 4095;

    public class ImageDescriptor
	{	
		public ushort xPos;		    // Image Left Position
		public ushort yPos;		    // Image Top Position
		public ushort Width;		// Image Width
		public ushort Height;		// Image Height
		private byte PackedFields;	// Packed Fields. Bits info:
							        //  0-2: Bits per pixel (-1)
							        //  3-4: (Reserved)
							        //    5: Sort Flag
							        //    6: Interlace Flag
							        //    7: Local Color Table Flag
		
		public ImageDescriptor()
		{
			xPos = 0;
			yPos = 0;
			Width = 0;
			Height = 0;
			PackedFields = 0;
		}

        public ImageDescriptor(int width, int height)
        {
            xPos = 0;
            yPos = 0;
            Width = (ushort) width;
            Height = (ushort) height;
            PackedFields = 0;
        }

        public ImageDescriptor(ImageDescriptor copyme)
        {
            xPos = copyme.xPos;
            yPos = copyme.yPos;
            Width = copyme.Width;
            Height = copyme.Height;
            PackedFields = copyme.PackedFields;
        }

        public int BitsPerPixel
        {
            get
            {
                return (int)(PackedFields & 0x07) + 1;
            }
            set
            {
                if (value < 1 || value > 8)
                {
                    throw new ArgumentException();
                }
                PackedFields &= 0xF8; // Clear lower 3 bits
                PackedFields |= (byte)(value - 1);
            }
        }

		public bool Interlaced
		{
			get
            {
                return ((PackedFields & 0x40) != 0);
            }
            set
            {
                if (value)
			    {
				    PackedFields |= 0x40;
			    }
			    else
			    {
				    PackedFields &= 0xBF;
			    }
            }
		}

		public bool LocalColorTable
		{
			get
            {
                return ((PackedFields & 0x80) != 0);
            }
            set
            {
                if (value)
                {
                    PackedFields |= 0x80;
                }
                else
                {
                    PackedFields &= 0x7F;
                }
            }
		}

		public void Read(BinaryReader br)
		{
			xPos = br.ReadUInt16();
            yPos = br.ReadUInt16();
            Width = br.ReadUInt16();
            Height = br.ReadUInt16();
            PackedFields = br.ReadByte();
		}

		public void Write(BinaryWriter bw)
		{
			bw.Write(xPos);
            bw.Write(yPos);
            bw.Write(Width);
            bw.Write(Height);
            bw.Write(PackedFields);
		}
	}

    public class GraphicControlExtension
	{
        // Packed Fields. Bits detail:
        //    0: Transparent Color Flag
        //    1: User Input Flag
        //  2-4: Disposal Method
        public byte PackedFields;

        public ushort Delay;		// Delay Time in hundredths of seconds
        public byte Transparent;	// Transparent Color Index

		public GraphicControlExtension()
		{
			this.PackedFields = 0;
			this.Delay = 0;
			this.Transparent = 0;
		}

        public GraphicControlExtension(GraphicControlExtension copyme)
        {
            this.PackedFields = copyme.PackedFields;
            this.Delay = copyme.Delay;
            this.Transparent = copyme.Transparent;
        }

        public int DisposalMethod
        {
            get
            {
                return (int)((PackedFields & 0x1C) >> 2);
            }
            set
            {
                PackedFields &= 0xE3;			// Clear out existing disposal method bits
                PackedFields |= (byte)((value & 0x07) << 2);
            }
        }
        
		public bool HasTransparency
		{
            get
            {
                return ((PackedFields & 0x01) != 0);
            }
            set
            {
                if (value)
                {
                    PackedFields |= 0x01;
                }
                else
                {
                    PackedFields &= 0xFE;
                }
            }
		}
		
        public void Read(BinaryReader br)
		{
			byte blockSize = br.ReadByte();
            if (blockSize != 4)
            {
                throw new InvalidDataException(
                    "The GIF graphic control extension did not have a block size of 4");
            }
            PackedFields = br.ReadByte();
            Delay = br.ReadUInt16();
            Transparent = br.ReadByte();
		}

        public void SetDelayInMilliseconds(uint delay)
        {
            this.Delay = (ushort)(delay / 10);
        }

		public void Write(BinaryWriter bw)
		{
            byte blockSize = 4;
            bw.Write(blockSize);
            bw.Write(PackedFields);
            bw.Write(Delay);
            bw.Write(Transparent);
		}
	}
    
    public class ScreenDescriptor
	{
		public ushort ScreenWidth;      // Logical Screen Width
        public ushort ScreenHeight;	    // Logical Screen Height
        private byte PackedFields;      // Packed Fields. Bits info:
									        //  0-2: Bits per pixel (-1)
									        //    3: Sort Flag
									        //  4-6: Color Resolution
									        //    7: Global Color Table Flag
        public byte Background;		    // Background Color Index
        public byte PixelAspectRatio;	// Pixel Aspect Ratio

        public ScreenDescriptor()
        {
	        // Initialize values for the screen descriptor
	        ScreenWidth			= 0;
	        ScreenHeight		= 0;
	        PackedFields		= 0;
	        Background			= 0;
	        PixelAspectRatio	= 0;
        }

        public ScreenDescriptor(int width, int height)
        {
            ScreenWidth = (ushort) width;
            ScreenHeight = (ushort) height;
            PackedFields = 0;
	        Background = 0;
	        PixelAspectRatio = 0;
        }

        public int BitsPerPixel
        {
	        get
            {
                // The first three bits of the "PackedFields" member is a value 
	            // representing bits per pixel. This value will range from 0 to 7
	            // since it is only 3 bits long, but it represents a value between
	            // 1 and 8, so we just add one.
	            return (PackedFields & 0x07) + 1;
            }
            set
            {
                // The paramater should range from 1 to 8. Since we are to store it
	            // in 3 bits, we will have to subtract 1 from the value.
	            value--;
            	
	            // Clear out first 3 bits:
	            PackedFields &= 0xF8;

	            // Set first 3 bits
	            PackedFields |= (byte)value;

	            // Set bits 4-6
	            value <<= 4;
	            PackedFields |= (byte)value;
            }
        }

        public bool HasColorTable
        {
	        get
            {
                return ((PackedFields & 0x80) != 0);
            }
            set
            {
                if (value)
                {
                    PackedFields |= 0x80;
                }
                else
                {
                    PackedFields &= 0x7F;
                }
            }
        }

        public void Read(BinaryReader br)
        {
	        ScreenWidth = br.ReadUInt16();
            ScreenHeight = br.ReadUInt16();
            PackedFields = br.ReadByte();
            Background = br.ReadByte();
            PixelAspectRatio = br.ReadByte();
        }

        public void Write(BinaryWriter bw)
        {
	        bw.Write(ScreenWidth);
            bw.Write(ScreenHeight);
            bw.Write(PackedFields);
            bw.Write(Background);
            bw.Write(PixelAspectRatio);
        }
	}

    protected class LZWCompressionTable
	{
		public short[] indices;
		public int ClearCode;
		public int EOICode;
		public short size;

        public static readonly int LZWTABLESIZE = 1048576;

        public LZWCompressionTable(int BitsPerPixel)
        {
	        indices = new short[LZWTABLESIZE];
	        Init(BitsPerPixel);
        }

        public int Add(int PrefixIndex, byte K)
        {
	        // This function adds an item to the table and returns the table size
	        // in number of elements.
        	
	        int Index = Hash(PrefixIndex, K);
	        indices[Index] = size;
        	
	        // Increase table size
	        size++;

	        // Return table size
	        return size;
        }

        public void Clear()
        {
	        // Clears all values to -1
            for (int i=0; i<indices.Length; i++)
            {
                indices[i] = -1;
            }

	        size = 0;
        }

        public int Hash(int PrefixIndex, byte K)
        {
	        int temp = K << 12;
	        temp = (temp | PrefixIndex) % LZWTABLESIZE;
	        return temp;
        }

        public void Init(int BitsPerPixel)
        {
	        // Initialize the table with entries depending on bits per pixel

	        Clear();
        	
	        int palsize = (1 << BitsPerPixel);		// # of entries in palette
	        ClearCode = (1 << BitsPerPixel);		// Clear code
	        EOICode = ClearCode + 1;				// End of information code
	        size = 0;								// Table size starts as 0

	        // Initialize the table with the pallete indices, clear code and end of 
	        // info code.
	        for (int x=0; x<palsize; x++)
	        {
		        Add(EMPTY_PREFIX, (byte) x);
	        }

	        // We don't need to add the clear code or the end of information code to
	        // the table because we want the search for these items to return false,
	        // but we do have increase the table size for their 2 entries.
	        size = (short)(palsize+2);
        }

        public bool IsItemInTable(int PrefixIndex, byte K, ref int OutIndex)
        {
	        // Note that OutIndex remains unchanged if the prefix is not found
        	
	        if (PrefixIndex == EMPTY_PREFIX)
	        {
		        // Prefix is empty, just fill OutIndex with K
		        OutIndex = (int) K;
		        return true;
	        }
        	
	        int Index = Hash(PrefixIndex, K);
	        if (indices[Index] != -1)
	        {
		        OutIndex = indices[Index];
		        return true;
	        }

	        return false;
        }
    }

    bool Animated;
    Stream m_s;
    string g_Comment;
    bool m_writingFile;
    private string errormsg;
    ScreenDescriptor m_sd;

    public delegate void GIFImageLoadedCallback(int w, int h, byte[] bits, uint[] palette,
        ushort xPos, ushort yPos, GraphicControlExtension GCE, string Comment);

    private struct TableItem
    {
        public byte[] Data;
        public int Length;
    }
    
    public EOGIFFile()
    {
        g_Comment = null;
        m_s = null;
	    m_writingFile = false;
	    Animated = false;
        m_sd = null;
        errormsg = "An unspecified error has occured";
    }

    /// <summary>
    /// Closes a file if one is currently open
    /// </summary>
    public void Close()
    {
	    if (m_writingFile)
	    {
		    // Write file terminator
            m_s.WriteByte(0x3B); // ';'
		    m_writingFile = false;
	    }
	    if (null != m_s)
	    {
            m_s.Close();
            m_s = null;
	    }
	    Animated = false;
    }

    private static unsafe byte[] Compress(byte[] bits, int w, int h)
    {
        // Variables
        int bpp = 8;					// Bits per pixel
        int ClearCode = (1 << bpp);		// Clear code
        int EOICode = ClearCode + 1;	// End of information code
        int CodeSize = bpp + 1;			// Code size (starts at bpp+1)
        int PrefixIndex = EMPTY_PREFIX;	// Index of prefix in table
        int TableSize = 0;				// # of used items in table
        int x;							// Loop variable
        
        // Initialize a bit vector for dealing with codes ranging from 3 to 12 bits.
        EOBitVector EBV = new EOBitVector();		
        
        // Number of clear codes that have been output (not required)
        int NumCCs = 0;

        // Initialize the compression table
        LZWCompressionTable table = new LZWCompressionTable(bpp);

        // Start output with clear code
        EBV.Push((uint)ClearCode, CodeSize);

        // Loop through image data and compress
        for (x=0; x<(w*h); x++)
        {
            // See if "prefix"+"bits[x]" is in table
            if (!table.IsItemInTable(PrefixIndex, bits[x], ref PrefixIndex))
            {
                // Add "prefix"+"bits[x]" to the string table
                TableSize = table.Add(PrefixIndex, bits[x]);

                // Output code for prefix (without "bits[x]") to the bit vector
                EBV.Push((uint)PrefixIndex, CodeSize);

                // If our next table entry is going to require more bits, increase
                // the code size.
                if ( (TableSize-1) > ((1 << CodeSize) - 1) )
                {
                    CodeSize++;
                }

                // Set prefix to "bits[x]"
                PrefixIndex = (int) bits[x];

                // If we've exceed the maximum code size of 12 bits, output the 
                // clear code, reset the code size to bpp+1, and reset the table.
                if (CodeSize > 12 || TableSize > 4095)
                {
                    NumCCs++;
                    EBV.Push((uint)ClearCode, (CodeSize>12)?12:CodeSize);
                    CodeSize = bpp+1;

                    // Initialize the table with the pallete indices, clear code 
                    // and end of info code.
                    table.Init(bpp);
                }
            }
        }

        EBV.Push((uint)PrefixIndex, CodeSize);
        EBV.Push((uint)EOICode, CodeSize);
        return EBV.DataArray;
    }

    public bool Decompress(byte[] CompressedBits, int CompBitsSize, byte[] OutBits,
        int rootsize, int imgsize)
    {
	    // Quick error check:
	    if (null == CompressedBits || null == OutBits || imgsize < 0 || rootsize < 1 || rootsize > 12)
	    {
		    return false;
	    }
    	
	    // Decompression pseudo-code:
	    /*
	     [1] Initialize string table;
         [2] get first code: <code>;
         [3] output the string for <code> to the charstream;
         [4] <old> = <code>;
         [5] <code> <- next code in codestream;
         [6] does <code> exist in the string table?
          (yes: output the string for <code> to the charstream;
                [...] <- translation for <old>;
                K <- first character of translation for <code>;
                add [...]K to the string table;        
			    <old> <- <code>;  
	      )
          (no: [...] <- translation for <old>;
               K <- first character of [...];
               output [...]K to charstream and add it to string table;
               <old> <- <code>
          )
         [7] go to [5];
	    */

        int i, x;						// Loop variables
	    int BitPos = 0;					// Position we're at (in # of bits) in 
									    // compressed data
	    int BytePos = 0;				// Position we're at in output data
	    int Code = 0;					// Temporary code from compressed data
	    int old = 0;
	    int CodeSize = rootsize + 1;	// Code size
	    int ClearCode = (1 << rootsize);// Clear code
	    int EOICode = ClearCode + 1;	// End of information code
	    int TableSize = 0;				// # of used items in table

	    // Init the string table
	    TableItem[] Table = new TableItem[4096];
        Array.Clear(Table, 0, Table.Length);
	    for (x=0; x<ClearCode; x++)
	    {
		    Table[x].Data = new byte[1];
		    Table[x].Length = 1;
		    Table[x].Data[0] = (byte)x;
	    }
	    TableSize = EOICode+1;

	    // Get the first code and if it's the clear code (which it should be)
	    // get the next one.
	    Code = GetNextCode(CompressedBits, ref BitPos, CodeSize);
	    if (Code == ClearCode)
	    {
		    Code = GetNextCode(CompressedBits, ref BitPos, CodeSize);
	    }

	    // Output string for code to output
	    OutBits[0] = (byte)Code;
	    BytePos++;

	    old = Code;

	    while (BytePos < imgsize)
	    {
		    // Avoid overflow:
		    /*
		    if (CodeSize + BitPos > CompBitsSize*8)
		    {
			    return true;
		    }
		    */

		    Code = GetNextCode(CompressedBits, ref BitPos, CodeSize);

		    if (Code == ClearCode)
		    {
			    // Re-init stuff
			    CodeSize = rootsize + 1;
			    for (x=EOICode+1; x<4096; x++)
			    {
				    Table[x].Data = null;
				    Table[x].Length = 0;
			    }
			    TableSize = EOICode+1;
			    Code = GetNextCode(CompressedBits, ref BitPos, CodeSize);
			    OutBits[BytePos] = (byte)Code;
			    BytePos++;
			    old = Code;
		    }
		    else if (Code == EOICode)
		    {
			    return true;
		    }
		    else if (null == Table[Code].Data) // Code does NOT exist in the string table
		    {
			    int len = Table[old].Length + 1;

			    // Add this string (old + first character of old) to the string table
			    Table[Code].Length = len;
			    Table[Code].Data = new byte[len];
                Array.Copy(Table[old].Data, Table[Code].Data, len - 1);
			    Table[Code].Data[len-1] = Table[old].Data[0];
			    TableSize++;
    			
			    // Output this string to the codestream:
                for (i = 0; i < len; i++)
                {
                    OutBits[BytePos + i] = Table[Code].Data[i];
                }
			    BytePos += len;

			    old = Code;

			    // See if we need to increase the code size:
			    if ( (TableSize-1) >= ((1 << CodeSize) - 1) )
			    {
				    CodeSize++;
				    if (CodeSize > 12)
				    {
					    CodeSize = 12;
				    }
			    }
		    }
		    else	// Code does exist in the string table
		    {
			    // Output the string for Code to the output bits
			    int len = Table[Code].Length;
                for (i = 0; i < len; i++)
                {
                    OutBits[BytePos + i] = Table[Code].Data[i];
                }
			    BytePos += len;

			    // Get string for old, add first character of the string for Code 
			    // to the end of it, and add it to the string table
			    len = Table[old].Length+1;
			    Table[TableSize].Length = len;
			    Table[TableSize].Data = new byte[len];
                Array.Copy(Table[old].Data, Table[TableSize].Data, len - 1);
			    Table[TableSize].Data[len-1] = Table[Code].Data[0];
			    TableSize++;

			    old = Code;

			    // See if we need to increase the code size:
			    if ( (TableSize-1) >= ((1 << CodeSize) - 1) )
			    {
				    CodeSize++;
				    if (CodeSize > 12)
				    {
					    CodeSize = 12;
				    }
			    }
		    }
	    }

	    // Free allocated items in string table
	    for (x=0; x<4096; x++)
	    {
		    Table[x].Data = null;
		    Table[x].Length = 0;
	    }

	    return true;
    }

    private void DeInterlace(byte[] imgbits, int w, int h)
    {
	    // Text taken from GIF document by Compuserve:
	    // The first pass writes every  8th  row, starting  with  the top row of 
	    // the image window.  The second pass writes every 8th row starting at 
	    // the fifth row from the top.   The  third  pass writes every 4th row 
	    // starting at the third row from the top.  The fourth pass completes the 
	    // image, writing  every  other  row,  starting  at  the second row from 
	    // the top.
    	
	    // We'll copy the rows over to a new image, then back to the original
	    byte[] tempbits = new byte[w*h];
	    if (null == tempbits)
	    {
            throw new OutOfMemoryException(
                "Could not allocate " + Convert.ToString(w * h) + " bytes of data.");
	    }
        Array.Clear(tempbits, 0, tempbits.Length);

	    // Determine the last row # for each pass (zero based)
	    int pass1 = (h-1) / 8;
	    int pass2 = pass1 + (h+3) / 8;
	    int pass3 = pass2 + (h+1) / 4;
    	
	    int y, RealRowNumber;
	    for (y=0; y<h; y++)
	    {
		    if (y <= pass1)
		    {
			    RealRowNumber = y * 8;
		    }
		    else if (y <= pass2)
		    {
			    RealRowNumber = (y-pass1-1) * 8 + 4;
		    }
		    else if (y <= pass3)
		    {
			    RealRowNumber = (y-pass2-1) * 4 + 2;
		    }
		    else
		    {
			    RealRowNumber = (y-pass3-1) * 2 + 1;
		    }
		    if (RealRowNumber < h)
		    {
                Array.Copy(imgbits, (y * w), tempbits, (RealRowNumber * w), w);
		    }
	    }

	    // Copy de-interlaced image back
        Array.Copy(tempbits, imgbits, w * h);
	    tempbits = null;
    }

    private unsafe int GetNextCode(byte[] compbits, ref int BitPosition, int CodeSize)
    {
	    // This function gets "CodeSize" bits from "compbits" starting at bit
	    // number "BitPosition". "BitPosition" is then increased by "CodeSize".
	    // "compbits" should have an extra 3 (or more) bytes allocated so that we 
	    // can cast to an integer for simplicity.
    	
	    // Get next code notes:
	    // - (BitPos / 8) will be the position of the first byte in the compressed 
	    //   data that contains one or more valid bit for our code.
	    // - The code will span at most over 3 bytes since the code size will not
	    //   exceed 12.
	    // - The first byte of the potential 3 will have 1 to 8 valid bits
	    // - The second byte of the potential 3 will have 0 to 8 valid bits
	    // - The third byte of the potential 3 will have 0 to 3 valid bits

	    int usedbits = (1 << CodeSize) - 1;
	    int bytepos = BitPosition/8;
        int Code = compbits[bytepos] + ((int)compbits[bytepos + 1]) * 256 +
            ((int)compbits[bytepos + 2]) * 65536 + ((int)compbits[bytepos + 3]) * (65536 << 8);
    	
	    // Get rid of unused bits and put used bits to the right.
	    Code >>= (BitPosition % 8);
	    Code &= usedbits;

	    // Increase bit position
	    BitPosition += CodeSize;

	    return Code;
    }

    private void Interlace(byte[] imgbits, int w, int h)
    {
	    // We'll copy the rows over to a new image, then back to the original
	    byte[] tempbits = new byte[w*h];
	    if (null == tempbits)
	    {
            throw new OutOfMemoryException(
                "Could not allocate " + Convert.ToString(w * h) + " bytes of data.");
	    }

	    int y=0;
	    int RealRowNumber = 0;

	    // Pass 1 : Every 8th. row, starting with row 0
	    for (y=0; y<h; y+=8)
	    {
		    // Copy from imgbits to tempbits
            Array.Copy(imgbits, y * w, tempbits, RealRowNumber * w, w);
		    RealRowNumber++;
	    }

	    // Pass 2 : Every 8th. row, starting with row 4
	    for (y=4; y<h; y+=8)
	    {
		    // Copy from imgbits to tempbits
            Array.Copy(imgbits, y * w, tempbits, RealRowNumber * w, w);
		    RealRowNumber++;
	    }

	    // Pass 3 : Every 4th. row, starting with row 2
	    for (y=2; y<h; y+=4)
	    {
		    // Copy from imgbits to tempbits
            Array.Copy(imgbits, y * w, tempbits, RealRowNumber * w, w);
		    RealRowNumber++;
	    }

	    // Pass 4 : Every 2nd. row, starting with row 1
	    for (y=1; y<h; y+=2)
	    {
		    // Copy from imgbits to tempbits
            Array.Copy(imgbits, y * w, tempbits, RealRowNumber * w, w);
		    RealRowNumber++;
	    }

	    // Copy the interlaced image back
        Array.Copy(tempbits, imgbits, tempbits.Length);
	    tempbits = null;
    }

    public bool LoadFromFile(string filename, out int out_NumImgsLoaded, bool CountImagesOnly,
        GIFImageLoadedCallback onImageLoaded)
    {
	    // If the onImagesLoaded callback function is not provided (null reference) and the 
        // user is not counting images, then throw an invalid argument exception.
        if (null == onImageLoaded && !CountImagesOnly)
        {
            throw new ArgumentException(
                "The \"onImageLoaded\" delegate must be non-null to load images. This parameter "+
                "can only be null when counting (but not actually decompressing) images.");
        }
        
        // Variables
	    int i=0, x=0;				        // For loops
	    int Globalbpp=0;		            // Bits per pixel as defined in screen descriptor
	    uint[] palette = new uint[256];     // Palette
	    uint[] g_palette = new uint[256];   // Global palette
	    int w=0, h=0;			            // Width and height as defined in screen descriptor
	    bool DidRetry = false;	            // For corrupt files
	    GraphicControlExtension gifgce = new GraphicControlExtension();
	    int GraphicExtensionFound = 0;

	    // Start with no images loaded
	    out_NumImgsLoaded = 0;

	    // Set all palette items to white by default
        for (x=0; x<256; x++)
        {
            palette[x] = 0xFFffFFff;
            g_palette[x] = 0xFFffFFff;
        }

	    // If a stream is already open from a previous save action, close it.
	    if (null != m_s)
	    {
		    m_s.Close();
	    }

	    // Open the GIF file
        try
        {
            m_s = new FileStream(filename, FileMode.Open, FileAccess.Read);
        }
        catch (Exception) { m_s = null; }
	    if (null == m_s)
	    {
		    errormsg = "Could not open the specified GIF file.\n"+
                "The file may in use, corrupt, or non-existent.";
		    return false;
	    }
        // Create a binary reader
        BinaryReader br = new BinaryReader(m_s);

	    // Read first 6 bytes (GIF87a or GIF89a)
	    byte[] filesig = new byte[6];
        br.Read(filesig, 0, 6);
	    if (!Encoding.ASCII.GetString(filesig).Equals("GIF87a") && 
            !Encoding.ASCII.GetString(filesig).Equals("GIF89a"))
	    {
		    errormsg = "File is not a valid GIF file.\n";
		    return false; 
	    }

	    // Read the screen descriptor. This structure defines the overall 
	    // parameters for all of the images in the GIF file. However, 
	    // individual images have their own descriptors that can take priority 
	    // over these values.
	    m_sd = new ScreenDescriptor();
        try
        {
            m_sd.Read(br);
        }
        catch (IOException)
	    {
		    errormsg = "Failed to read GIF header data.";
		    return false;
	    }
    	
	    // Get bits per pixel
	    Globalbpp = m_sd.BitsPerPixel;

	    // fill some animation data:
	    w = m_sd.ScreenWidth;
	    h = m_sd.ScreenHeight;
	    int ColorCount = 1<<Globalbpp; // 2 to the power of Globalbpp

	    // Read the global palette from the file or generate it
	    if (m_sd.HasColorTable)	// File has global palette?
	    {
		    // Read in the RGB palette items (3 bytes a piece)
		    for (x=0; x<ColorCount; x++)
		    {
			    byte[] tempbytes = new byte[3];
                br.Read(tempbytes, 0, 3);
			    palette[x] = 0xFF000000 | (((uint)tempbytes[2]) << 16) |
                    (((uint)tempbytes[1]) << 8) | ((uint)tempbytes[0]);
		    }		
	    }
	    else // GIF standard says to provide an internal default Palette:
	    {
		    for (x=0; x<256; x++)
		    {
			    palette[x] = 0xFF000000 | ((uint)x)<<16 | ((uint)x)<<8 | ((uint)x);
		    }
	    }
        Array.Copy(palette, g_palette, 256);

	    // The rest of the file data now consists of extension blocks and 
	    // images. We will read in extension blocks and separators until
	    // we encounter a ';' character to signify the end of the file.
	    do
	    {
		    byte charGot = br.ReadByte();

		    if (charGot == 0x21)		// Extension block
		    {
			    charGot = br.ReadByte();
			    switch (charGot)
			    {
                case 0xF9:			// Graphic Control Extension
				    gifgce.Read(br);
				    GraphicExtensionFound++;
				    charGot = br.ReadByte(); // Block Terminator (always 0)
			    break;

			    case 0xFE:			// Comment Extension:
				    ReadCommentExt();
			    break;

			    case 0x01:			// PlainText Extension: Ignored
			    case 0xFF:			// Application Extension: Ignored
			    default:			// Unknown Extension: Ignored
				    // read (and ignore) data sub-blocks
				    int nBlockLength = (int)br.ReadByte();
				    while (0 != nBlockLength)
				    {
					    br.BaseStream.Position += nBlockLength;
                        nBlockLength = (int)br.ReadByte();
				    }
			    break;
			    }
		    }
		    else if (charGot == 0x2c) // Image data (0x2c Image Separator)
		    {
                // Read Image Descriptor
			    ImageDescriptor gifid = new ImageDescriptor();
			    gifid.Read(br);

			    // See if there is anything that would hint at an error
			    if (gifid.xPos > m_sd.ScreenWidth || 
				    gifid.yPos > m_sd.ScreenHeight)
			    {
				    return false;
			    }

			    // Calculate size of image
			    int bpp = gifid.LocalColorTable ? gifid.BitsPerPixel : Globalbpp;
			    int imgsize = gifid.Width * gifid.Height;
    		
			    retry:
			    if (gifid.LocalColorTable)	// Read Color Map (if descriptor says so)
			    {
				    // Read in the RGB palette items
				    for (x=0; x<(1<<bpp); x++)
				    {
					    byte[] tempbytes = new byte[3];
                        br.Read(tempbytes, 0, 3);
			            palette[x] = 0xFF000000 | (((uint)tempbytes[2]) << 16) |
                            (((uint)tempbytes[1]) << 8) | ((uint)tempbytes[0]);
				    }
			    }
			    else				// Otherwise copy Global
			    {
				    Array.Copy(g_palette, palette, g_palette.Length);
			    }

			    // 1st byte of img block (root size)
			    int firstbyte = (int) br.ReadByte();
			    if (-1 == firstbyte)
			    {
                    errormsg = "Unexpected end of GIF found.\n"+
					    "This file may be incomplete or corrupt.";
				    return false;
			    }
			    else if (firstbyte < 1 || firstbyte > 12)
			    {
				    if (!gifid.LocalColorTable && !DidRetry)
				    {
					    // Maybe this means there IS a local palette
					    DidRetry = true;
					    goto retry;
				    }
				    else
				    {
                        errormsg = "GIF file is corrupt.";
					    return false;
				    }
			    }
			    DidRetry = false; // Reset so we can use it later (currently we don't but we may in the future)

			    // Calculate compressed image block size
			    long ImgStart, ImgEnd;				
			    ImgEnd = ImgStart = m_s.Position;
                try
                {
                    while ((x = (int)br.ReadByte()) != 0)
                    {
                        if (-1 == x)
                        {
                            errormsg = "Unexpected end of GIF found.\n" +
                               "This file may be incomplete or corrupt.";
                            return false;
                        }
                        m_s.Position = (ImgEnd += x + 1);
                    }
                }
                catch (EndOfStreamException)
                {
                    errormsg = "Unexpected end of GIF found.\n" +
                               "This file may be incomplete or corrupt.";
                    return false;
                }
                x = (int)br.ReadByte(); // This should be 0x21, 0x2C, or 0x3B
                m_s.Position = ImgStart;

			    // Allocate space for compressed image data (+4 bytes so we can
			    // cast to int when decompressing).
			    int compsize = (int)(ImgEnd-ImgStart+4);
			    byte[] pCompressedImage = new byte[compsize];
			    if (null == pCompressedImage)
			    {
                    errormsg = "Failed to allocate memory to load GIF image ("+
                        Convert.ToString(compsize) + " bytes).";
				    Close();
				    return false;
			    }
      
			    // Read and store Compressed Image
			    i = 0;
			    while ((x = (int)br.ReadByte()) != 0)
			    {
				    br.Read(pCompressedImage, i, x);
                    i += x;
			    }
    			
			    if (!CountImagesOnly)
			    {
				    // We only decompress if the user did not flag this call
				    // as a "count only" call. If a pointer to the "AddImage"
				    // callback is provided, it will be used, otherwise we
				    // buffer the images in the "Images" list.

				    // Allocate space for image
				    byte[] bits = new byte[imgsize];

				    // Call LZW/GIF decompressor
				    if (!Decompress(pCompressedImage, compsize, bits, firstbyte, 
                            gifid.Width*gifid.Height))
				    {
                        errormsg = "Failed to decompress GIF image data.\n"+
						    "The image data is corrupt.";
					    return false;
				    }
    				
				    // Deinterlace the image if needed
				    if (gifid.Interlaced)
				    {
					    DeInterlace(bits, gifid.Width, gifid.Height);
				    }

				    // Invoke the callback to give the image to the host application
                    onImageLoaded(gifid.Width, gifid.Height, bits, palette, gifid.xPos, gifid.yPos,
                        (GraphicExtensionFound>0) ? gifgce : null, g_Comment);
					
                    g_Comment = null;
                    bits = null;
			    }

			    out_NumImgsLoaded++;

			    // Some cleanup
			    pCompressedImage = null;
			    GraphicExtensionFound = 0;
		    }


		    else if (charGot == 0x3b)
		    {
			    // This marks the end of the GIF file; break the do-while loop.
			    break;
		    }

	    } while (m_s.Position < m_s.Length);

	    // Close the stream
        br.Close();
        br = null;
        m_s = null;
    	
	    return true;
    }

    public bool OpenForWriting(string filename, int w, int h, uint[] palette, bool GlobalPalette,
        bool Animate, bool LoopAnimation)
    {
	    // Open the output file
        m_s = new FileStream(filename, FileMode.Create, FileAccess.Write);
	    if (null == m_s)
	    {
            errormsg = "Could not open GIF file for writing.\n"+
			    "The destination disk may be full or inaccessible.";
		    return false;
	    }

	    // Write the 6 byte signature
	    byte[] sig = Encoding.ASCII.GetBytes("GIF89a");
	    try
        {
            m_s.Write(sig, 0, 6);
        }
        catch (IOException)
	    {
            errormsg = "Could not write file signature to file.\n"+
			    "The destination disk may be full or inaccessible.";
		    m_s.Close();
            m_s = null;
		    return false;
	    }

        // Create a binary writer for the stream
        BinaryWriter bw = new BinaryWriter(m_s);

	    // Create and write the screen descriptor
	    ScreenDescriptor sd = new ScreenDescriptor(w, h);
	    sd.BitsPerPixel = 8;
	    sd.HasColorTable = GlobalPalette; // Global palette follows?
	    sd.Background = 0;
	    try
        {
            sd.Write(bw);
        }
        catch (IOException)
	    {
            errormsg = "Could not write GIF image data to file.\n"+
			    "The destination disk may be full or inaccessible.";
		    m_s.Close();
            m_s = null;
		    return false;
	    }

	    // Write 256 RGB entries from our RGBA palette
	    if (GlobalPalette)
	    {
            WriteRGBPalette(palette, bw);
	    }

	    // Write the loop animation block if we want to loop animation
	    if (Animate && LoopAnimation)
	    {
		    m_s.WriteByte(0x21);			// Extension block identifier
            m_s.WriteByte(0xFF);			// Application extension
            m_s.WriteByte(0x0B);			// Length of application block (11)
		    byte[] temptext = Encoding.ASCII.GetBytes("NETSCAPE2.0");
            m_s.Write(temptext, 0, 11);

            m_s.WriteByte(0x03);			// 3  bytes follow
            m_s.WriteByte(0x01);
		    
            short loopcount = 0;
            bw.Write(loopcount);
            
            // End block
            m_s.WriteByte(0);
	    }

	    // Set global animation value
	    Animated = Animate;

	    m_writingFile = true;
	    return true;
    }

    private void ReadCommentExt()
    {
	    // This function reads in a comment extension block from the current 
	    // position in the file. The 0xFE comment extension identifier should have
	    // been the last byte read from the file and it should be at the position
	    // where the data blocks start.

	    // Backup the position where this block starts
        long startPos = m_s.Position;

        // First, determine the comment length and allocate necessary space
	    int iBlockLength = 0;
	    int CommentSize = 0;
        iBlockLength = m_s.ReadByte();
	    while (iBlockLength > 0)
	    {
		    // Seek past block data and add to comment size
            m_s.Position += iBlockLength;
            CommentSize += iBlockLength;
		    
            // Read in next block length
            iBlockLength = m_s.ReadByte();
	    }
	    if (CommentSize <= 0)
	    {
		    throw new IOException(
                "Unexpected end of file found. The file data seems to be truncated or invalid.");
	    }
        byte[] cmtBytes = new byte[CommentSize];

	    // Now seek back and read the data into the global string buffer
        m_s.Position = startPos;
	    iBlockLength = m_s.ReadByte();
	    CommentSize = 0;
	    while (iBlockLength > 0)
	    {
		    m_s.Read(cmtBytes, CommentSize, iBlockLength);
		    CommentSize += iBlockLength;
		    iBlockLength = m_s.ReadByte();
	    }

        // Copy bytes to g_Comment string
        g_Comment = Encoding.ASCII.GetString(cmtBytes);
    }

    /// <summary>
    /// Screen descriptor for the last GIF file loaded with a call to "LoadFromFile".
    /// May be null if no file has been successfully loaded.
    /// </summary>
    public ScreenDescriptor Descriptor
    {
        get
        {
            return m_sd;
        }
    }

    private void WriteDataBlocks(byte[] data)
    {
        int x;
        for (x = 0; x < data.Length; x += 255)
        {
            if (data.Length - x < 255)
            {
                // Write a block less than 255 bytes in size
                m_s.WriteByte((byte)(data.Length - x));
                m_s.Write(data, x, data.Length - x);
            }
            else
            {
                // Write a 255 byte block with 1 byte size before it
                m_s.WriteByte(0xFF);
                m_s.Write(data, x, 255);
            }
        }
    }

    public bool WriteComment(string comment)
    {
	    if (null == m_s)
        {
            throw new ApplicationException(
                "Could not write a comment block because there is no active stream. Use "+
                "\"OpenForWriting\" to open an output stream.");
        }
        if (string.IsNullOrEmpty(comment))
        {
            throw new ArgumentException(
                "Cannot write a null or empty string as a GIF comment.");
        }

	    m_s.WriteByte(0x21);	// Extension introducer
	    m_s.WriteByte(0xFE);	// Comment label
	    
        // Get the bytes for the comment string and write them in blocks
        byte[] cmt = Encoding.ASCII.GetBytes(comment);
        WriteDataBlocks(cmt);
        
        m_s.WriteByte(0x00);	// Block terminator
	    
        return true;
    }

    public bool WriteImage(byte[] image, int w, int h, uint[] palette, ushort xPos, ushort yPos,
        bool ilace, GraphicControlExtension gce)
    {
	    // Error check:
	    if (w < 1 || h < 1)
	    {
            throw new ArgumentException(
                "Invalid parameters passed to \"WriteImage\"");
	    }
	    if (null == m_s)
	    {
		    return false;
	    }

        // Create a binary writer for the stream
        BinaryWriter bw = new BinaryWriter(m_s);

	    // Write the extension block if the GCE was provided
	    if (null != gce)
	    {
		    m_s.WriteByte(0x21);	// Extension block identifier
		    m_s.WriteByte(0xF9);	// Graphic control label
		    gce.Write(bw);          // Write the graphic control extension data
		    m_s.WriteByte(0);		// Block terminator
	    }
    	
	    // Variables:
	    byte separator = 0x2C;
	    int x = 0;

	    // Write an image seperator
	    m_s.WriteByte(separator);

	    // Create and write image descriptor
	    ImageDescriptor id = new ImageDescriptor(w, h);
	    id.xPos = xPos;
	    id.yPos = yPos;
	    id.BitsPerPixel = 8;
	    id.LocalColorTable = (palette != null);
	    id.Interlaced = ilace;
        try
        {
            id.Write(bw);
        }
	    catch (IOException)
	    {
            errormsg = "Could not write GIF image data to file.\n"+
			    "The destination disk may be full or inaccessible.";
		    return false;	
	    }

	    // Write local color map if necessary (256 RGB entries)
	    if (null != palette)
	    {
            WriteRGBPalette(palette, bw);
	    }

	    // Write code size
        m_s.WriteByte(8);

	    // Interlace if requested
	    if (ilace)
	    {
		    Interlace(image, w, h);
	    }

	    // Compress the bits
	    byte[] output = Compress(image, w, h);
	    if (null == output)
	    {
            errormsg = "Failed to compress image data.";
		    return false;
	    }

	    // Write data in blocks of 255 bytes or less, with a 1 byte size value before each block.
	    for (x=0; x<output.Length; x+=255)
	    {
            if (output.Length - x < 255)
		    {
			    // Write a block less than 255 bytes in size
                m_s.WriteByte((byte)(output.Length - x));
                m_s.Write(output, x, output.Length - x);
		    }
		    else
		    {
			    // Write a 255 byte block with 1 byte size before it
                m_s.WriteByte(0xFF);
                m_s.Write(output, x, 255);
		    }
	    }

	    // "Free" memory (times like these I really miss C++)
        output = null;

	    // Write block terminator (a zero byte)
        m_s.WriteByte(0);

	    return true;
    }

    private void WriteRGBPalette(uint[] palette, BinaryWriter bw)
    {
        for (int x = 0; x < 256; x++)
        {
            // If there are less than 256 entries in the palette, then we'll fill the remainder
            // with grayscale entries.
            if (x >= palette.Length)
            {
                bw.Write((byte)x);
                bw.Write((byte)x);
                bw.Write((byte)x);
            }
            else
            {
                bw.Write((byte)palette[x]);
                bw.Write((byte)((palette[x] & 0x0000FF00) >> 8));
                bw.Write((byte)((palette[x] & 0x00FF0000) >> 16));
            }
        }
    }
}