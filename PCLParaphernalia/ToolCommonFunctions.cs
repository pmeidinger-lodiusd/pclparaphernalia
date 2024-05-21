﻿using System;
using System.IO;
using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides common Tool functions.
    /// 
    /// © Chris Hutchinson 2013
    /// 
    /// </summary>

    public static class ToolCommonFunctions
    {
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r e a t e O p e n F i l e D i a l o g                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Creates a OpenFileDialog.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//
        public static OpenFileDialog createOpenFileDialog(string initialPath)
        {
            string folderName = null;
            string fileName = null;

            ToolCommonFunctions.splitPathName(initialPath,
                                               ref folderName,
                                               ref fileName);

            OpenFileDialog openDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.Exists(folderName) ? folderName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = fileName,
                CheckFileExists = true,
                Filter = "All files|*.*"
            };

            return openDialog;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c r e a t e S a v e F i l e D i a l o g                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Creates a SaveFileDialog.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//
        public static SaveFileDialog createSaveFileDialog(string initialPath)
        {
            string folderName = null;
            string fileName = null;

            ToolCommonFunctions.splitPathName(initialPath,
                                               ref folderName,
                                               ref fileName);

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                InitialDirectory = Directory.Exists(folderName) ? folderName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = fileName,
                Filter = "Print Files | *.prn; *.PRN",
                DefaultExt = "prn",
                RestoreDirectory = true,
                OverwritePrompt = true,
                CheckFileExists = false,
                CheckPathExists = true
            };

            return saveDialog;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d e c o m p o s e P a t h N a m e                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Break specified name down into component parts.                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void decomposePathName (string pathName,
                                               ref string volName,
                                               ref string folderName,
                                               ref string lastName,
                                               ref string extension)
        {
            int indxA,
                  indxB,
                  lenA,
                  lenB,
                  lenC;

            if (pathName == null)
            {
                volName    = "";
                folderName = "";
                lastName   = "";
                extension  = "";
            }
            else
            {
                lenA = pathName.Length;

                //------------------------------------------------------------//
                //                                                            //
                // Obtain volume (disk) name.                                 //
                //                                                            //
                //------------------------------------------------------------//

                indxA = pathName.IndexOf (":");

                if (indxA == -1)
                {
                    volName = "";
                }
                else
                {
                    volName = pathName.Substring (0, indxA + 1);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain folder name (including volume (disk) name).         //
                //                                                            //
                //------------------------------------------------------------//

                indxA = pathName.LastIndexOf ("\\");

                if (indxA == -1)
                {
                    folderName = "";
                }
                else
                {
                    folderName = pathName.Substring (0, indxA);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain last (terminal) name and extension name.            //
                //                                                            //
                //------------------------------------------------------------//

                lenB = lenA - indxA - 1;

                indxB = pathName.LastIndexOf (".", (lenA - 1), lenB);

                lenC = lenA - indxB - 1;

                if (indxB == -1)
                {
                    lastName  = pathName.Substring ((indxA + 1), lenB);
                    extension = "";
                }
                else
                {
                    lastName  = pathName.Substring ((indxA + 1),
                                                    (lenB - lenC - 1));
                    extension = pathName.Substring ((indxB + 1), lenC);
                }
            }
        }
        
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t F o l d e r N a m e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return folder name from supplied path name.                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void getFolderName (string pathName,
                                          ref string folderName)
        {
            string tmpVol   = "", 
                   tmpTname = "",
                   tmpExt   = "";

            decomposePathName (pathName, ref tmpVol,
                               ref folderName, ref tmpTname, ref tmpExt);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T e r m i n a l N a m e                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return terminal name (including extension) from supplied path      //
        // name.                                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void getTerminalName (string pathName,
                                            ref string fileName)
        {
            string tmpVol    = null,
                   tmpFolder = null,
                   tmpTname  = null,
                   tmpExt    = null;

            decomposePathName (pathName, ref tmpVol,
                               ref tmpFolder, ref tmpTname, ref tmpExt);

            fileName = tmpTname + "." + tmpExt;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s p l i t P a t h N a m e                                   I      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return folder and file names from supplied path name.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void splitPathName (string pathName,
                                          ref string folderName,
                                          ref string fileName)
        {
            string tmpVol   = null,
                   tmpTname = null,
                   tmpExt   = null;

            decomposePathName (pathName, ref tmpVol,
                               ref folderName, ref tmpTname, ref tmpExt);

            fileName = tmpTname + "." + tmpExt;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // s p l i t P a t h N a m e                                  I I     //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return folder and file names from supplied path name.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void splitPathName (string pathName,
                                          ref string volName,
                                          ref string folderName,
                                          ref string fileName)
        {
            string tmpTname = null,
                   tmpExt = null;

            decomposePathName(pathName, ref volName,
                               ref folderName, ref tmpTname, ref tmpExt);

            fileName = tmpTname + "." + tmpExt;
        }
    }
}
