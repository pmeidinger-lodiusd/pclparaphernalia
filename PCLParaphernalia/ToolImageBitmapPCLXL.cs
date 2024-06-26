﻿using System;
using System.IO;
using System.Windows;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides PCL XL support for the ImageBitmap tool.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class ToolImageBitmapPCLXL
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
                                          int destScalePercentY)
        {
            const int sizeStd = 1024;

            byte[] bufStd = new byte[sizeStd];

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
                MessageBox.Show("Bitmaps: compressed formats not supported.",
                                "Bitmap File",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }
            else if ((srcBitsPerPixel != 1) &&
                     (srcBitsPerPixel != 4) &&
                     (srcBitsPerPixel != 24))
            {
                MessageBox.Show("Bitmaps: only 1-, 4- and 24-bit supported.",
                                "Bitmap File",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }
            else if (srcHeight < 0)
            {
                MessageBox.Show("Bitmaps: top-down DIBs not supported.",
                                "Bitmap File",
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
            const int maxImageBlock = 2048;
            const int sizeStd = 64;

            byte[] bufStd = new byte[sizeStd];

            int indStd = 0;
            bool firstBlock = true;

            int bytesPerRow,
                  padBytes;

            int imageCrntLine,
                  imageHeight,
                  imageRowMult,
                  imageBlockHeight,
                  imageBlockSize;
            bool indexed;
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

            padBytes = bytesPerRow % 4;

            if (padBytes != 0)
            {
                padBytes = 4 - padBytes;
                bytesPerRow += padBytes;
            }

            imageCrntLine = 0;
            imageHeight = srcHeight;
            imageRowMult = (int)Math.Floor(maxImageBlock /
                                             (double)bytesPerRow);

            if (imageRowMult == 0)
                imageRowMult = 1;

            byte[] bufSub = new byte[bytesPerRow];

            for (int i = 0; i < imageHeight; i += imageRowMult)
            {
                if ((imageCrntLine + imageRowMult) >= imageHeight)
                    imageBlockHeight = imageHeight - imageCrntLine;
                else
                    imageBlockHeight = imageRowMult;

                imageBlockSize = imageBlockHeight * bytesPerRow;

                PCLXLWriter.AddAttrUint16(ref bufStd,
                                    ref indStd,
                                    PCLXLAttributes.Tag.StartLine,
                                    (ushort)imageCrntLine);

                PCLXLWriter.AddAttrUint16(ref bufStd,
                                    ref indStd,
                                    PCLXLAttributes.Tag.BlockHeight,
                                    (ushort)imageBlockHeight);

                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.Tag.CompressMode,
                                   (byte)PCLXLAttrEnums.Val.eNoCompression);

                PCLXLWriter.AddOperator(ref bufStd,
                                  ref indStd,
                                  PCLXLOperators.Tag.ReadImage);

                PCLXLWriter.AddEmbedDataIntro(ref bufStd,
                                        ref indStd,
                                        imageBlockSize);

                prnWriter.Write(bufStd, 0, indStd);
                indStd = 0;

                for (int j = 0; j < imageRowMult; j++)
                {
                    if ((i + j) >= imageHeight)
                    {
                        j = imageRowMult;
                    }
                    else
                    {
                        ToolImageBitmapCore.GetNextImageBlock(ref bufSub,
                                                              bytesPerRow,
                                                              firstBlock);

                        //     if (srcBitsPerPixel == 24)
                        if (!indexed)
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

                        prnWriter.Write(bufSub, 0, bytesPerRow);
                    }
                }

                imageCrntLine += imageBlockHeight;
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
                                                uint srcPaletteEntries,
                                                bool srcBlackWhite)
        {
            const int sizeStd = 256;

            byte[] bufStd = new byte[sizeStd];

            int indStd = 0;
            uint paletteEntries = 0,
                   paletteSize = 0;

            byte colourDepth = 0,
                 colourMapping = 0,
                 colourSpace = 0;

            //----------------------------------------------------------------//
            //                                                                //
            // Calculate destination size.                                    //
            //                                                                //
            //----------------------------------------------------------------//

            if (srcResX == 0)
                srcResX = 96; // DefaultSourceBitmapResolution;
            else
                srcResX = (int)(srcResX / 39.37);

            if (srcResY == 0)
                srcResY = 96; // DefaultSourceBitmapResolution;
            else
                srcResY = (int)(srcResY / 39.37);

            int destWidth = srcWidth * PCLXLWriter._sessionUPI / srcResX *
              (destScalePercentX / 100);
            int destHeight = srcHeight * PCLXLWriter._sessionUPI / srcResY *
              (destScalePercentY / 100);

            //----------------------------------------------------------------//
            //                                                                //
            // Set position.                                                  //
            //                                                                //
            //----------------------------------------------------------------//

            PCLXLWriter.AddAttrSint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.Tag.Point,
                                  (short)(destPosX * PCLXLWriter._sessionUPI),
                                  (short)(destPosY * PCLXLWriter._sessionUPI));

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetCursor);

            //----------------------------------------------------------------//
            //                                                                //
            // Set colour space.                                              //
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
                colourSpace = (byte)PCLXLAttrEnums.Val.eGray;
                colourDepth = (byte)PCLXLAttrEnums.Val.e1Bit;
                colourMapping = (byte)PCLXLAttrEnums.Val.eIndexedPixel;
                paletteEntries = 2;
                paletteSize = 2;
            }
            else if (srcBitsPerPixel == 1)
            {
                colourSpace = (byte)PCLXLAttrEnums.Val.eRGB;
                colourDepth = (byte)PCLXLAttrEnums.Val.e1Bit;
                colourMapping = (byte)PCLXLAttrEnums.Val.eIndexedPixel;
                paletteEntries = 0x00000001 << 1;
                paletteSize = 3 * paletteEntries;    // one per plane
            }
            else if (srcBitsPerPixel == 4)
            {
                colourSpace = (byte)PCLXLAttrEnums.Val.eRGB;
                colourDepth = (byte)PCLXLAttrEnums.Val.e4Bit;
                colourMapping = (byte)PCLXLAttrEnums.Val.eIndexedPixel;
                paletteEntries = 0x00000001 << 4;
                paletteSize = 3 * paletteEntries;    // one per plane
            }
            else if (srcBitsPerPixel == 24)
            {
                colourSpace = (byte)PCLXLAttrEnums.Val.eRGB;
                colourDepth = (byte)PCLXLAttrEnums.Val.e8Bit;
                colourMapping = (byte)PCLXLAttrEnums.Val.eDirectPixel;
                paletteEntries = 0;
                paletteSize = 0;
            }

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.PushGS);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.ColorSpace,
                               colourSpace);

            if (paletteEntries != 0)
            {
                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.Tag.PaletteDepth,
                                   (byte)PCLXLAttrEnums.Val.e8Bit);

                if (srcBlackWhite)
                {
                    byte[] tempUByteArray = new byte[2];

                    tempUByteArray[0] = 0;
                    tempUByteArray[1] = 255;

                    PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                            ref indStd,
                                            PCLXLAttributes.Tag.PaletteData,
                                            2,
                                            tempUByteArray);
                }
                else
                {
                    int offset;

                    byte red = 0x00,
                         green = 0x00,
                         blue = 0x00;

                    byte[] tempUByteArray = new byte[paletteSize];

                    for (int i = 0; i < srcPaletteEntries; i++)
                    {
                        offset = i * 3;

                        ToolImageBitmapCore.GetBmpPaletteEntry(i,
                                                               ref red,
                                                               ref green,
                                                               ref blue);

                        tempUByteArray[offset] = red;
                        tempUByteArray[offset + 1] = green;
                        tempUByteArray[offset + 2] = blue;
                    }

                    PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                            ref indStd,
                                            PCLXLAttributes.Tag.PaletteData,
                                            (short)paletteSize,
                                            tempUByteArray);
                }
            }

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetColorSpace);

            //------------------------------------------------------------//
            //                                                            //
            // Generate BeginImage operator.                              //
            //                                                            //
            //------------------------------------------------------------//

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.ColorMapping,
                               colourMapping);

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.ColorDepth,
                               colourDepth);

            PCLXLWriter.AddAttrUint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.Tag.SourceWidth,
                                (ushort)srcWidth);

            PCLXLWriter.AddAttrUint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.Tag.SourceHeight,
                                (ushort)srcHeight);

            PCLXLWriter.AddAttrUint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.Tag.DestinationSize,
                                  (ushort)destWidth,
                                  (ushort)destHeight);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.BeginImage);

            prnWriter.Write(bufStd, 0, indStd);
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
            const int sizeStd = 16;

            byte[] bufStd = new byte[sizeStd];

            int indStd = 0;

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.EndImage);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.PopGS);

            prnWriter.Write(bufStd, 0, indStd);
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
                                       int destScalePercentY)
        {
            GenerateJobHeader(prnWriter,
                              paperSize,
                              paperType,
                              orientation);

            GenerateImage(prnWriter,
                          destPosX,
                          destPosY,
                          destScalePercentX,
                          destScalePercentY);

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
            const int sizeStd = 1024;

            byte[] bufStd = new byte[sizeStd];

            int indStd;

            PCLXLWriter.StdJobHeader(prnWriter, string.Empty);

            indStd = 0;

            if (orientation < PCLOrientations.GetCount())
            {
                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.Tag.Orientation,
                                   PCLOrientations.GetIdPCLXL(orientation));
            }

            if (paperSize < PCLPaperSizes.GetCount())
            {
                PCLXLWriter.AddAttrUbyte(ref bufStd,
                                   ref indStd,
                                   PCLXLAttributes.Tag.MediaSize,
                                   PCLPaperSizes.GetIdPCLXL(paperSize));
            }

            if ((paperType < PCLPaperTypes.GetCount()) &&
                (PCLPaperTypes.GetType(paperType) !=
                    PCLPaperTypes.EntryType.NotSet))
            {
                PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                        ref indStd,
                                        PCLXLAttributes.Tag.MediaType,
                                        PCLPaperTypes.GetName(paperType));
            }

            PCLXLWriter.AddAttrUbyte(ref bufStd,
                               ref indStd,
                               PCLXLAttributes.Tag.SimplexPageMode,
                               (byte)PCLXLAttrEnums.Val.eSimplexFrontSide);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.BeginPage);

            PCLXLWriter.AddAttrUint16XY(ref bufStd,
                                  ref indStd,
                                  PCLXLAttributes.Tag.PageOrigin,
                                  0, 0);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetPageOrigin);

            PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                    ref indStd,
                                    PCLXLAttributes.Tag.RGBColor,
                                    3, PCLXLWriter.rgbBlack);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetBrushSource);

            PCLXLWriter.AddAttrUbyteArray(ref bufStd,
                                    ref indStd,
                                    PCLXLAttributes.Tag.RGBColor,
                                    3, PCLXLWriter.rgbBlack);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.SetPenSource);

            prnWriter.Write(bufStd, 0, indStd);
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
            const int sizeStd = 32;

            byte[] bufStd = new byte[sizeStd];

            int indStd = 0;

            PCLXLWriter.AddAttrUint16(ref bufStd,
                                ref indStd,
                                PCLXLAttributes.Tag.PageCopies,
                                1);

            PCLXLWriter.AddOperator(ref bufStd,
                              ref indStd,
                              PCLXLOperators.Tag.EndPage);

            prnWriter.Write(bufStd, 0, indStd);

            PCLXLWriter.StdJobTrailer(prnWriter, false, string.Empty);
        }
    }
}