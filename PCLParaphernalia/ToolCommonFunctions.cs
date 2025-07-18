﻿using Microsoft.Win32;
using System;
using System.IO;

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
        public static OpenFileDialog CreateOpenFileDialog(string initialPath)
        {
            string folderName = null;
            string fileName = null;

            ToolCommonFunctions.SplitPathName(initialPath,
                                               ref folderName,
                                               ref fileName);

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Directory.Exists(folderName) ? folderName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openDialog.FileName = fileName;
            openDialog.CheckFileExists = true;
            openDialog.Filter = "All files|*.*";

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
        public static SaveFileDialog CreateSaveFileDialog(string initialPath)
        {
            string folderName = null;
            string fileName = null;

            ToolCommonFunctions.SplitPathName(initialPath,
                                               ref folderName,
                                               ref fileName);

            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.InitialDirectory = Directory.Exists(folderName) ? folderName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.FileName = fileName;
            saveDialog.Filter = "Print Files | *.prn; *.PRN";
            saveDialog.DefaultExt = "prn";
            saveDialog.RestoreDirectory = true;
            saveDialog.OverwritePrompt = true;
            saveDialog.CheckFileExists = false;
            saveDialog.CheckPathExists = true;

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

        private static void DecomposePathName(string pathName,
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
                volName = string.Empty;
                folderName = string.Empty;
                lastName = string.Empty;
                extension = string.Empty;
            }
            else
            {
                lenA = pathName.Length;

                //------------------------------------------------------------//
                //                                                            //
                // Obtain volume (disk) name.                                 //
                //                                                            //
                //------------------------------------------------------------//

                indxA = pathName.IndexOf(":");

                if (indxA == -1)
                {
                    volName = string.Empty;
                }
                else
                {
                    volName = pathName.Substring(0, indxA + 1);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain folder name (including volume (disk) name).         //
                //                                                            //
                //------------------------------------------------------------//

                indxA = pathName.LastIndexOf("\\");

                if (indxA == -1)
                {
                    folderName = string.Empty;
                }
                else
                {
                    folderName = pathName.Substring(0, indxA);
                }

                //------------------------------------------------------------//
                //                                                            //
                // Obtain last (terminal) name and extension name.            //
                //                                                            //
                //------------------------------------------------------------//

                lenB = lenA - indxA - 1;

                indxB = pathName.LastIndexOf(".", (lenA - 1), lenB);

                lenC = lenA - indxB - 1;

                if (indxB == -1)
                {
                    lastName = pathName.Substring((indxA + 1), lenB);
                    extension = string.Empty;
                }
                else
                {
                    lastName = pathName.Substring((indxA + 1),
                                                    (lenB - lenC - 1));
                    extension = pathName.Substring((indxB + 1), lenC);
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

        public static void GetFolderName(string pathName,
                                          ref string folderName)
        {
            string tmpVol = string.Empty,
                   tmpTname = string.Empty,
                   tmpExt = string.Empty;

            DecomposePathName(pathName, ref tmpVol,
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

        public static void GetTerminalName(string pathName,
                                            ref string fileName)
        {
            string tmpVol = null,
                   tmpFolder = null,
                   tmpTname = null,
                   tmpExt = null;

            DecomposePathName(pathName, ref tmpVol,
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

        private static void SplitPathName(string pathName,
                                          ref string folderName,
                                          ref string fileName)
        {
            string tmpVol = null,
                   tmpTname = null,
                   tmpExt = null;

            DecomposePathName(pathName, ref tmpVol,
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

        public static void SplitPathName(string pathName,
                                          ref string volName,
                                          ref string folderName,
                                          ref string fileName)
        {
            string tmpTname = null,
                   tmpExt = null;

            DecomposePathName(pathName, ref volName,
                               ref folderName, ref tmpTname, ref tmpExt);

            fileName = tmpTname + "." + tmpExt;
        }
    }
}
