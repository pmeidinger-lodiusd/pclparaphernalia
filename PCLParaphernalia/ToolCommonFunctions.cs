using System;
using System.IO;
using Microsoft.Win32;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class provides common Tool functions.</para>
    /// <para>© Chris Hutchinson 2013</para>
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
            var folderName = Path.GetDirectoryName(initialPath);
            var fileName = Path.GetFileName(initialPath);

            return new OpenFileDialog
            {
                InitialDirectory = Directory.Exists(folderName) ? folderName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = fileName,
                CheckFileExists = true,
                Filter = "All Files|*.*"
            };
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
            var folderName = Path.GetDirectoryName(initialPath);
            var fileName = Path.GetFileName(initialPath);

            return new SaveFileDialog
            {
                InitialDirectory = Directory.Exists(folderName) ? folderName : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = fileName,
                Filter = "Print Files|*.prn",
                DefaultExt = "prn",
                RestoreDirectory = true,
                OverwritePrompt = true,
                CheckFileExists = false,
                CheckPathExists = true
            };
        }
    }
}
