// Author:          Evan Olds
// Creation Date:   June 13, 2008

using System;

internal class EOBitVector
{
    // Number of bits currently used in the bit vector
    private int m_bitSize;

    private byte[] m_data;
    
    public EOBitVector()
    {
        // Allocate a default size of 20 bytes (160 bits)
        m_data = new byte[20];

        // Zero the array data. The push algorithm depends on this.
        Array.Clear(m_data, 0, m_data.Length);
    }

    public void Clear()
    {
        // Zero the array data. The push algorithm depends on this.
        Array.Clear(m_data, 0, m_data.Length);
        
        m_bitSize = 0;
    }

    /// <summary>
    /// Gets the internal data array. Modifying the contents of the array or resizing it may cause
    /// undesired behavior. It is recommended that the data is treated as read-only data.
    /// </summary>
    public byte[] DataArray
    {
        get
        {
            return m_data;
        }
    }

    public void Push(string value)
    {
        // Ensure a large enough array size for the push
        Resize(m_bitSize + value.Length);

        byte bitset;
        int bytenum, bitnum;
        foreach (char c in value.ToCharArray())
        {
            if ('1' == c)
            {
                // Get the number of the byte we're on in the data pointer
                bytenum = m_bitSize / 8;

                // Get the number of the bit to set
                bitnum = m_bitSize % 8;

                // Set the bit
                bitset = (byte)(1 << bitnum);
                m_data[bytenum] |= bitset;
            }

            m_bitSize++;
        }
    }

    public void Push(uint value, int bitcount)
    {
	    // Error check parameters
        if (bitcount <= 0 || bitcount > 32)
        {
            throw new ArgumentException(
                "Cannot push " + Convert.ToString(bitcount) + " bits from a 32-bit value.\n"+
                "Value must be in the range [1,32].");
        }
        
        // Ensure a large enough array size for the push
        Resize(m_bitSize + bitcount);
        
        byte bitset;
		int bytenum, bitnum;
        for ( ; bitcount > 0; bitcount--)
        {
            if ((value & 0x01) != 0)
            {
                // Get the number of the byte we're on in the data pointer
                bytenum = m_bitSize / 8;

                // Get the number of the bit to set
                bitnum = m_bitSize % 8;

                // Set the bit
                bitset = (byte)(1 << bitnum);
                m_data[bytenum] |= bitset;
            }
            value >>= 1;
            m_bitSize++;
        }
    }

    private void Resize(int minBitCapacity)
    {
        // Keep doubling the data size until there is enough room
        while (minBitCapacity / 8 + 1 > m_data.Length)
        {
            int oldLen = m_data.Length;
            Array.Resize(ref m_data, oldLen * 2);
            // Make sure newly allocated bytes are zeroed
            Array.Clear(m_data, oldLen, oldLen);
        }
    }

    public int SizeInBits
    {
        get
        {
            return m_bitSize;
        }
    }
}