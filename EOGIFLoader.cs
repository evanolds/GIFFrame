// Author:          Evan Olds
// Creation Date:   June 22, 2008
// Description:     Class to load GIF images as they would be rendered. The class EOGIFFile is used
//                  by this class, but it only delivers uncompressed frames and doesn't take into
//                  account rendering parameters such as disposal methods and image offsets. This class
//                  produces GIF frames that appear as they would when rendered on screen. All images
//                  are loaded as 32-bit bitmaps.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

internal class EOGIFLoader
{
    public struct GIFImg32
    {
        public Bitmap BM;
        public EOGIFFile.GraphicControlExtension GCE;
        public EOGIFFile.ImageDescriptor ID;

        public GIFImg32(Bitmap bmRef, EOGIFFile.GraphicControlExtension gce,
            EOGIFFile.ImageDescriptor id)
        {
            // Copy the graphic control extension
            if (null != gce)
            {
                GCE = new EOGIFFile.GraphicControlExtension(gce);
            }
            else
            {
                GCE = new EOGIFFile.GraphicControlExtension();
            }

            // Copy the image descriptor
            ID = new EOGIFFile.ImageDescriptor(id);

            BM = bmRef;
        }
    }
    
    private int m_num;
    private List<GIFImg32> m_images;
    private EOGIFFile m_gif;
    
    public EOGIFLoader(string fileName)
    {
        // Initialize the image list
        m_images = new List<GIFImg32>();
        
        m_gif = new EOGIFFile();
        
        // First count the number of images
        if (!m_gif.LoadFromFile(fileName, out m_num, true, null))
        {
            m_gif.Close();
            throw new ArgumentException(
                "Could not load GIF from file");
        }

        // Now load images
        m_gif.LoadFromFile(fileName, out m_num, false, GIFImageLoadedCallback);
    }

    private unsafe void DrawOver(GIFImg32 bottom, GIFImg32 top)
    {
        // Make sure the images are the same size
        if (bottom.BM.Width != top.BM.Width || bottom.BM.Height != top.BM.Height)
        {
            throw new InvalidProgramException();
        }

        Rectangle r = new Rectangle(0, 0, bottom.BM.Width, bottom.BM.Height);

        // Lock both bitmaps
        BitmapData bdBottom = bottom.BM.LockBits(r, ImageLockMode.ReadOnly, 
            PixelFormat.Format32bppArgb);
        BitmapData bdTop = top.BM.LockBits(r, ImageLockMode.ReadWrite,
            PixelFormat.Format32bppArgb);
        uint* bot = (uint*) bdBottom.Scan0.ToPointer();
        uint* pTop = (uint*) bdTop.Scan0.ToPointer();

        // Loop through new image data, plotting pixels from the top if they are
        // non-transparent and pixels from the bottom otherwise.
        for (int y=0; y<bottom.BM.Height; y++)
        {
            for (int x = 0; x < bottom.BM.Width; x++)
            {
                if (((*pTop) & 0xFF000000) == 0)
                {
                    *pTop = *bot;
                }

                pTop++;
                bot++;
            }
        }

        // Unlock
        bottom.BM.UnlockBits(bdBottom);
        top.BM.UnlockBits(bdTop);
    }

    /// <summary>
    /// Generates a 32-bit bitmap for the image at the specified index.
    /// </summary>
    /// <param name="index">Index in list of images.</param>
    /// <returns>A 32-bit bitmap frame.</returns>
    public Bitmap GetBM32(int index)
    {
        return m_images[index].BM;
    }

    public EOGIFFile.GraphicControlExtension GetGCE(int imageIndex)
    {
        return m_images[imageIndex].GCE;
    }

    private void GIFImageLoadedCallback(int w, int h, byte[] bits, uint[] palette,
        ushort xPos, ushort yPos, EOGIFFile.GraphicControlExtension GCE, string Comment)
    {
        EOGIFFile.ImageDescriptor id = new EOGIFFile.ImageDescriptor(w, h);
        id.xPos = xPos;
        id.yPos = yPos;

        // If the GCE indicates transparency, set the alpha to 0 for 
        // the corresponding palette item
        if (null != GCE && GCE.HasTransparency)
        {
            palette[GCE.Transparent] &= 0x00FFffFF;
        }

        // Swap RGB palette entries to BGR
        for (int i = 0; i < palette.Length; i++)
        {
            palette[i] = (palette[i] & 0xFF00FF00) | ((palette[i] & 0x000000FF) << 16) |
                ((palette[i] & 0x00FF0000) >> 16);
        }

        int sw = m_gif.Descriptor.ScreenWidth;
        int sh = m_gif.Descriptor.ScreenHeight;

        // Make a 32-bit bitmap the size of the screen width (GIF screen descriptor
        // screen width that is) and fill it from the 8-bit data.
        Bitmap bm = new Bitmap(sw, sh, PixelFormat.Format32bppArgb);
        if (null == bm)
        {
            // Not much we can do but try to keep loading
            return;
        }
        Rectangle r = new Rectangle(0, 0, sw, sh);
        BitmapData bd = bm.LockBits(r, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        unsafe
        {
            uint* pxls = (uint*) bd.Scan0.ToPointer();
            for (int y = 0; y < sh; y++)
            {
                for (int x = 0; x < sw; x++)
                {
                    if (x < xPos || y < yPos || x >= xPos + w || y >= yPos + h)
                    {
                        // Outside of this image's rectangle => transparent pixel
                        *pxls = 0;
                        pxls++;
                        continue;
                    }

                    int pixelIndex = (y - yPos) * w + (x - xPos);
                    *pxls = palette[bits[pixelIndex]];
                    pxls++;
                }
            }
        }
        bm.UnlockBits(bd);

        GIFImg32 img = new GIFImg32(bm, GCE, id);

        // If this is the first image in the list, no custom rendering is needed
        if (m_images.Count == 0)
        {
            m_images.Add(img);
        }
        else if (m_images[m_images.Count - 1].GCE.DisposalMethod <= 1)
        {
            // If we have an unspecified disposal method (0) or a leave in place (1) then
            // we will render this image on top of the previous frame with appropriate
            // transparency and position.
            DrawOver(m_images[m_images.Count - 1], img);
            m_images.Add(img);
        }
        //else if (2 == m_images[m_images.Count - 1].GCE.DisposalMethod)
        //{
        //    // A disposal method of 2 indicates that we are to restore to background color
        //    uint clr = palette[m_gif.Descriptor.Background];
        //    TransToClr(img.BM, clr);
        //    m_images.Add(img);
        //}
        else
        {
            m_images.Add(img);
        }
    }

    public int ImageCount
    {
        get
        {
            return m_images.Count;
        }
    }

    public void RemoveImageAt(int index)
    {
        m_images.RemoveAt(index);
    }

    /// <summary>
    /// Sets all pixels in the bitmap with 0 alpha to clr.
    /// </summary>
    private unsafe void TransToClr(Bitmap bm, uint clr)
    {
        Rectangle r = new Rectangle(0, 0, bm.Width, bm.Height);
        BitmapData bd = bm.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        uint* pxls = (uint*)bd.Scan0.ToPointer();
        int pxlCount = bm.Width * bm.Height;
        while (pxlCount > 0)
        {
            if (((*pxls) & 0xFF000000) == 0)
            {
                *pxls = clr;
            }

            pxls++;
            pxlCount--;
        }
        bm.UnlockBits(bd);
    }
}