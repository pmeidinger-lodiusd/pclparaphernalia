using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PJL 'status readback' Command objects.</para>
    /// <para>© Chris Hutchinson 2010</para>
    ///
    /// </summary>
    internal static class PJLCommands
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public enum RequestType
        {
            None,
            Variable,
            Category,
            FSBinSrc,
            FSDelete,
            FSDirList,
            FSInit,
            FSMkDir,
            FSQuery,
            FSUpload
        }

        public enum CmdFormat
        {
            None,
            Standard,
            Words
        }

        [System.Reflection.Obfuscation(Exclude = true)]
        public enum CmdIndex
        {
            Unknown,
            Null,
            COMMENT,
            DEFAULT,
            DINQUIRE,
            DMCMD,
            DMINFO,
            ECHO,
            ENTER,
            EOJ,
            FSAPPEND,
            FSDELETE,
            FSDIRLIST,
            FSDOWNLOAD,
            FSINIT,
            FSMKDIR,
            FSQUERY,
            FSUPLOAD,
            INFO,
            INITIALIZE,
            INQUIRE,
            JOB,
            OPMSG,
            RDYMSG,
            RESET,
            SET,
            STMSG,
            USAGE,
            USTATUS,
            USTATUSOFF
        }

        public static string nullCmdKey = "<null>";

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly SortedList<string, PJLCommand> _cmds = new SortedList<string, PJLCommand>();

        private static PJLCommand _cmdUnknown;

        private static int _cmdCount;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P J L C o m m a n d s                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PJLCommands()
        {
            PopulateTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // c h e c k C m d                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Searches the command name table for a matching entry.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool CheckCmd(string name, ref string description, int level)
        {
            PJLCommand cmd;

            bool seqKnown;
            if (_cmds.IndexOfKey(name) != -1)
            {
                seqKnown = true;
                cmd = _cmds[name];
            }
            else
            {
                seqKnown = false;
                cmd = _cmdUnknown;
            }

            description = cmd.Description;

            cmd.IncrementStatisticsCount(level);

            return seqKnown;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y C m d s                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Display list of commands.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int DisplayCmds(DataGrid grid)
        {
            int count = 0;

            foreach (KeyValuePair<string, PJLCommand> kvp in _cmds)
            {
                count++;
                grid.Items.Add(kvp.Value);
            }

            return count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S t a t s C o u n t s                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add counts of referenced commands to nominated data table.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DisplayStatsCounts(DataTable table, bool incUsedSeqsOnly)
        {
            bool displaySeq,
                    hddrWritten;

            DataRow row;

            hddrWritten = false;

            //----------------------------------------------------------------//

            displaySeq = true;

            int count = _cmdUnknown.StatsCtTotal;
            if (count == 0)
                displaySeq = false;

            if (displaySeq)
            {
                if (!hddrWritten)
                {
                    DisplayStatsCountsHddr(table);
                    hddrWritten = true;
                }

                row = table.NewRow();

                row[0] = _cmdUnknown.Name;
                row[1] = _cmdUnknown.Description;
                row[2] = _cmdUnknown.StatsCtParent;
                row[3] = _cmdUnknown.StatsCtChild;
                row[4] = _cmdUnknown.StatsCtTotal;

                table.Rows.Add(row);
            }

            //----------------------------------------------------------------//

            foreach (KeyValuePair<string, PJLCommand> kvp in _cmds)
            {
                displaySeq = true;

                count = kvp.Value.StatsCtTotal;

                if (count == 0)
                {
                    if (incUsedSeqsOnly)
                        displaySeq = false;
                }

                if (displaySeq)
                {
                    if (!hddrWritten)
                    {
                        DisplayStatsCountsHddr(table);
                        hddrWritten = true;
                    }

                    row = table.NewRow();

                    row[0] = kvp.Value.Name;
                    row[1] = kvp.Value.Description;
                    row[2] = kvp.Value.StatsCtParent;
                    row[3] = kvp.Value.StatsCtChild;
                    row[4] = kvp.Value.StatsCtTotal;

                    table.Rows.Add(row);
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d i s p l a y S t a t s C o u n t s H d d r                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Add statistics header lines to nominated data table.               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DisplayStatsCountsHddr(DataTable table)
        {
            //----------------------------------------------------------------//

            DataRow row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "_____________";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "PJL commands:";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);

            row = table.NewRow();

            row[0] = string.Empty;
            row[1] = "¯¯¯¯¯¯¯¯¯¯¯¯¯";
            row[2] = string.Empty;
            row[3] = string.Empty;
            row[4] = string.Empty;

            table.Rows.Add(row);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Command definitions.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount() => _cmdCount;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return description associated with specified command.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDesc(CmdIndex key) => _cmds[key.ToString()].Description;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t F o r m a t                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return format of command.                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static CmdFormat GetFormat(CmdIndex key) => _cmds[key.ToString()].Format;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t N a m e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return name associated with specified command.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetName(CmdIndex key) => _cmds[key.ToString()].Name;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t T y p e                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return type of command.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static RequestType GetType(CmdIndex key) => _cmds[key.ToString()].Type;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e T a b l e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate table of commands.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void PopulateTable()
        {
            CmdIndex indx = CmdIndex.Unknown;
            _cmdUnknown =
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "*** Unknown command ***");

            indx = CmdIndex.Null;
            _cmds.Add(nullCmdKey,
                new PJLCommand(indx,
                                CmdFormat.None,
                                RequestType.None,
                                "Null (no command)"));

            indx = CmdIndex.COMMENT;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Words,
                                RequestType.None,
                                "Comment"));

            indx = CmdIndex.DEFAULT;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Set environment variable default"));

            indx = CmdIndex.DINQUIRE;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.Variable,
                                "Request default value of environment variable"));

            indx = CmdIndex.DMCMD;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Process PML request"));

            indx = CmdIndex.DMINFO;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Process PML request & read response"));

            indx = CmdIndex.ECHO;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Words,
                                RequestType.None,
                                "Echo value to host"));

            indx = CmdIndex.ENTER;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Enter language"));

            indx = CmdIndex.EOJ;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Job end"));

            indx = CmdIndex.FSAPPEND;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSBinSrc,
                                "File System: file append"));

            indx = CmdIndex.FSDELETE;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSDelete,
                                "File System: file delete"));

            indx = CmdIndex.FSDIRLIST;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSDirList,
                                "File System: return directory list"));

            indx = CmdIndex.FSDOWNLOAD;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSBinSrc,
                                "File System: download file to printer"));

            indx = CmdIndex.FSINIT;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSInit,
                                "File System: initialise"));

            indx = CmdIndex.FSMKDIR;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSMkDir,
                                "File System: create directory"));

            indx = CmdIndex.FSQUERY;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSQuery,
                                "File System: query"));

            indx = CmdIndex.FSUPLOAD;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.FSUpload,
                                "File System: upload file to host"));

            indx = CmdIndex.INFO;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.Category,
                                "Request information category details"));

            indx = CmdIndex.INITIALIZE;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Reset environment variables to factory defaults"));

            indx = CmdIndex.INQUIRE;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.Variable,
                                "Request value of environment variable"));

            indx = CmdIndex.JOB;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Job start"));

            indx = CmdIndex.OPMSG;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Display Operator message"));

            indx = CmdIndex.RDYMSG;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Display Ready message"));

            indx = CmdIndex.RESET;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Reset environment variables to defaults"));

            indx = CmdIndex.SET;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Set environment variable"));

            indx = CmdIndex.STMSG;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Display Status message"));

            indx = CmdIndex.USAGE;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Usage (proprietary extension)"));

            indx = CmdIndex.USTATUS;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Allow printer to send unsolicited messages"));

            indx = CmdIndex.USTATUSOFF;
            _cmds.Add(indx.ToString(),
                new PJLCommand(indx,
                                CmdFormat.Standard,
                                RequestType.None,
                                "Stop printer sending unsolicited messages"));

            _cmdCount = _cmds.Count;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        //  r e s e t S t a t s C o u n t s                                   //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Reset counts of referenced commnads.                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void ResetStatsCounts()
        {
            PJLCommand cmd;

            _cmdUnknown.ResetStatistics();

            foreach (KeyValuePair<string, PJLCommand> kvp in _cmds)
            {
                cmd = kvp.Value;

                cmd.ResetStatistics();
            }
        }
    }
}