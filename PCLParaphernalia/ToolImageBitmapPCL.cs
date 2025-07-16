using System.IO;
using System.Windows;

namespace PCLParaphernalia;

/// <summary>
/// 
/// Class provides PCL support for the ImageBitmap tool.
/// 
/// © Chris Hutchinson 2010
/// 
/// </summary>

static class ToolImageBitmapPCL
{
    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e I m a g e                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate print equivalent of bitmap image.                         //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateImage(BinaryWriter prnWriter,
                                      float destPosX,
                                      float destPosY,
                                      int destScalePercentX,
                                      int destScalePercentY,
                                      int rasterResolution)
    {
        byte[] bufStd = new byte[1024];

        int srcWidth = 0,
              srcHeight = 0,
              srcResX = 0,
              srcResY = 0;

        uint srcCompression = 0,
               srcPaletteEntries = 0;

        ushort srcBitsPerPixel = 0;

        bool srcBlackWhite = false;

        ToolImageBitmapCore.GetBmpInfo(ref srcWidth,
                                       ref srcHeight,
                                       ref srcBitsPerPixel,
                                       ref srcCompression,
                                       ref srcResX,
                                       ref srcResY,
                                       ref srcPaletteEntries,
                                       ref srcBlackWhite);

        if (srcCompression != 0)
        {
            MessageBox.Show("Bitmaps: compressed formats not supported",
                            "Bitmap file",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
            return;
        }
        else if ((srcBitsPerPixel != 1) &&
                 (srcBitsPerPixel != 4) &&
                 (srcBitsPerPixel != 24))
        {
            MessageBox.Show("Bitmaps: only 1-, 4- and 24-bit supported",
                            "Bitmap file",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
            return;
        }
        else if (srcHeight < 0)
        {
            MessageBox.Show("Bitmaps: top-down DIBs not supported",
                            "Bitmap file",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
            return;
        }

        GenerateImageHeader(prnWriter,
                            srcBitsPerPixel,
                            srcWidth,
                            srcHeight,
                            srcResX,
                            srcResY,
                            destPosX,
                            destPosY,
                            destScalePercentX,
                            destScalePercentY,
                            rasterResolution,
                            srcPaletteEntries,
                            srcBlackWhite);

        GenerateImageData(prnWriter,
                          srcBitsPerPixel,
                          srcWidth,
                          srcHeight);

        GenerateImageTrailer(prnWriter);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e I m a g e D a t a                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate ReadImage operator(s) and associated embedded data.       //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateImageData(BinaryWriter prnWriter,
                                          ushort srcBitsPerPixel,
                                          int srcWidth,
                                          int srcHeight)
    {
        bool firstBlock = true,
                indexed = true;

        int bytesPerRow,
              bytesPerRowPadded,
              padBytes;

        //------------------------------------------------------------//

        if (srcBitsPerPixel == 1)
        {
            indexed = true;
            bytesPerRow = srcWidth / 8;
            if ((srcWidth % 8) != 0)
                bytesPerRow++;
        }
        else if (srcBitsPerPixel == 4)
        {
            indexed = true;
            bytesPerRow = srcWidth / 2;
            if ((srcWidth % 2) != 0)
                bytesPerRow++;
        }
        else // if (srcBitsPerPixel == 24)
        {
            indexed = false;
            bytesPerRow = srcWidth * 3;
        }

        bytesPerRowPadded = bytesPerRow;

        padBytes = bytesPerRow % 4;

        if (padBytes != 0)
        {
            padBytes = 4 - padBytes;
            bytesPerRowPadded += padBytes;
        }

        byte[] bufSub = new byte[bytesPerRowPadded];

        for (int i = 0; i < srcHeight; i++)
        {
            ToolImageBitmapCore.GetNextImageBlock(ref bufSub,
                                                  bytesPerRowPadded,
                                                  firstBlock);

            if (!indexed)      // if (srcBitsPerPixel == 24)
            {
                // change BGR components to RGB //

                byte temp;
                int endLine = bytesPerRow - 2;

                for (int k = 0; k <= endLine; k += 3)
                {
                    if (bufSub[k] != bufSub[k + 2])
                    {
                        temp = bufSub[k];
                        bufSub[k] = bufSub[k + 2];
                        bufSub[k + 2] = temp;
                    }
                }
            }

            firstBlock = false;

            PCLWriter.RasterTransferRow(prnWriter, bytesPerRow, bufSub);
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e I m a g e H e a d e r                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write image initialisation sequences to output file.               //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateImageHeader(BinaryWriter prnWriter,
                                            ushort srcBitsPerPixel,
                                            int srcWidth,
                                            int srcHeight,
                                            int srcResX,
                                            int srcResY,
                                            float destPosX,
                                            float destPosY,
                                            int destScalePercentX,
                                            int destScalePercentY,
                                            int rasterResolution,
                                            uint srcPaletteEntries,
                                            bool srcBlackWhite)
    {
        short coordX,
              coordY;

        uint paletteEntries = 0;

        byte bitsPerIndex = 0x00;

        bool indexed = true;

        //----------------------------------------------------------------//
        //                                                                //
        // Set position.                                                  //
        //                                                                //
        //----------------------------------------------------------------//

        coordX = (short)(destPosX * 600);
        coordY = (short)(destPosY * 600);

        PCLWriter.PalettePushPop(prnWriter, PCLWriter.ePushPop.Push);

        PCLWriter.CursorPosition(prnWriter, coordX, coordY);

        //----------------------------------------------------------------//
        //                                                                //
        // Set colour space, etc.                                         //
        //                                                                //
        // Note that we only support the following bitmap types:          //
        //                                                                //
        //   -  1-bit black and white:                                    //
        //      Colour space: Gray (1 plane)                              //
        //      Encoding:     indirect-pixel                              //
        //      Palette:      elements = 2 (= 2^1)                        //
        //                    planes   = 1                                //
        //                    length   = 2 (= 2 * 1) bytes.               //
        //      Image data:   Each image pixel is defined by 1 bit        //
        //                    which is used an an index into the          //
        //                    2-element palette.                          // 
        //                                                                //
        //   -  1-bit colour                                              //
        //      Colour space: RGB (3 plane)                               //
        //      Encoding:     indirect-pixel                              //
        //      Palette:      elements = 2 (= 2^1)                        //
        //                    planes   = 3                                //
        //                    length   = 6 (= 2 * 3) bytes.               //
        //      Image data:   Each image pixel is defined by 1 bit        //
        //                    which is used an an index into the          //
        //                    2-element palette.                          // 
        //                                                                //
        //   -  4-bit:                                                    //
        //      Colour space: RGB (3-plane)                               //
        //      Encoding:     indirect-pixel                              //
        //      Palette:      elements = 16 (= 2^4)                       //
        //                    planes   = 3                                //
        //                    length   = 48 (= 16 * 3) bytes.             //
        //      Image data:   Each group of 4 bits defines an image       //
        //                    pixel by use as an index into the           //
        //                    16-element palette.                         // 
        //                                                                //
        //   -  24-bit:                                                   //
        //      Colour space: RGB (3-plane)                               //
        //      Encoding:     direct-pixel                                //
        //      Palette:      none                                        //
        //      Image data:   Each group of 24 bits defines an image      //
        //                    pixel as three 8-bit values, directly       //
        //                    specifying the RGB values.                  //
        //                                                                //
        //----------------------------------------------------------------//

        if (srcBlackWhite)
        {
            indexed = true;
            bitsPerIndex = 0x01;
            paletteEntries = 0;
        }
        else if (srcBitsPerPixel == 1)
        {
            indexed = true;
            bitsPerIndex = 0x01;
            //  paletteEntries = 0x00000001 << 1;
            paletteEntries = srcPaletteEntries;
        }
        else if (srcBitsPerPixel == 4)
        {
            indexed = true;
            bitsPerIndex = 0x04;
            //  paletteEntries = 0x00000001 << 4;
            paletteEntries = srcPaletteEntries;
        }
        else if (srcBitsPerPixel == 24)
        {
            indexed = false;
            bitsPerIndex = 0x00;
            paletteEntries = 0;
        }

        if (srcBlackWhite)
        {
            PCLWriter.PaletteSimple(prnWriter, PCLWriter.eSimplePalette.K);
        }
        else
        {
            if (indexed)
            {
                PCLWriter.ConfigureImageData(prnWriter,
                                        0x02,   // ColourSpace = sRGB
                                        0x01,   // PEM = Indexed by Pixel
                                        bitsPerIndex,
                                        0x00,   // Not used
                                        0x00,   // Bits per component
                                        0x00);  // Bits per component
            }
            else
            {
                PCLWriter.ConfigureImageData(prnWriter,
                                        0x02,   // ColourSpace = sRGB
                                        0x03,   // PEM = Direct by Pixel
                                        0x00,   // Not used
                                        0x08,   // Bits per component
                                        0x08,   // Bits per component
                                        0x08);  // Bits per component
            }

            if (paletteEntries != 0)
            {
                byte red = 0x00,
                     green = 0x00,
                     blue = 0x00;

                for (short i = 0; i < paletteEntries; i++)
                {
                    ToolImageBitmapCore.GetBmpPaletteEntry(i,
                                                           ref red,
                                                           ref green,
                                                           ref blue);

                    PCLWriter.PaletteEntry(prnWriter, i, red, green, blue);
                }
            }
        }

        //----------------------------------------------------------------//
        //                                                                //
        // Generate raster definition and start sequences.                //
        //                                                                //
        //----------------------------------------------------------------//

        PCLWriter.RasterResolution(prnWriter,
                                    rasterResolution,
                                    true);

        PCLWriter.RasterBegin(prnWriter,
                              srcWidth,
                              srcHeight,
                              srcResX,
                              srcResY,
                              destScalePercentX,
                              destScalePercentY,
                              0);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e I m a g e T r a i l e r                            //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write image termination sequences to output file.                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateImageTrailer(BinaryWriter prnWriter)
    {
        PCLWriter.RasterEnd(prnWriter);

        PCLWriter.PalettePushPop(prnWriter, PCLWriter.ePushPop.Pop);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b                                              //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Generate test data.                                                //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void GenerateJob(BinaryWriter prnWriter,
                                   int paperSize,
                                   int paperType,
                                   int orientation,
                                   float destPosX,
                                   float destPosY,
                                   int destScalePercentX,
                                   int destScalePercentY,
                                   int rasterResolution)
    {
        GenerateJobHeader(prnWriter,
                          paperSize,
                          paperType,
                          orientation);

        GenerateImage(prnWriter,
                      destPosX,
                      destPosY,
                      destScalePercentX,
                      destScalePercentY,
                      rasterResolution);

        GenerateJobTrailer(prnWriter);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b H e a d e r                                  //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write stream initialisation sequences to output file.              //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobHeader(BinaryWriter prnWriter,
                                          int paperSize,
                                          int paperType,
                                          int orientation)
    {
        PCLWriter.StdJobHeader(prnWriter, string.Empty);

        PCLWriter.PageHeader(prnWriter,
                             paperSize,
                             paperType,
                             orientation,
                             PCLPlexModes.eSimplex);
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e n e r a t e J o b T r a i l e r                                //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Write termination sequences to output file.                        //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static void GenerateJobTrailer(BinaryWriter prnWriter)
    {
        PCLWriter.StdJobTrailer(prnWriter, false, 0);
    }
}
