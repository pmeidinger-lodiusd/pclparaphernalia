﻿using System;
using System.Collections.Generic;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL Symbol Set Type objects.</para>
    /// <para>© Chris Hutchinson 2015</para>
    ///
    /// </summary>
    internal static class PCLSymSetTypes
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        // Note that the length of the index array must be the same as that   //
        // of the definition array; the entries must be in the same order.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly bool _flagBound = true;
        private static readonly bool _flagUnbound;

        public enum Index
        {
            Unknown = 0,
            Bound_7bit = 1,
            Bound_8bit = 2,
            Bound_PC8 = 3,
            Bound_16bit = 4,
            Unbound_MSL = 5,
            Unbound_Unicode = 6
        };

        private enum IdPCL : byte
        {
            Unknown = 0,
            Bound_7bit = 0,
            Bound_8bit = 1,
            Bound_PC8 = 2,
            Bound_16bit = 3,
            Unbound_MSL = 10,
            Unbound_Unicode = 11
        };

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly List<PCLSymSetType> _sets = new List<PCLSymSetType>();

        private static int _setsCountBound;
        private static int _setsCountTotal;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L S y m b o l S e t T y p e s                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        static PCLSymSetTypes()
        {
            PopulateSymbolSetTypeTable();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Symbol Set Type definitions.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount() => _setsCountTotal;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t B o u n d                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Symbol Set Types which are 'bound'.                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCountBound() => _setsCountBound;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c S h o r t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return short description associated with the specified Symbol Set  //
        // Type index.                                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDescShort(int selection) => _sets[selection].DescShort;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c S t d                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return standard description associated with the specified Symbol   //
        // Set Type index.                                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDescStd(int selection) => _sets[selection].DescStd;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I d P C L                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the PCL identifier associated with the specified Symbol Set //
        // Type index.                                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static byte GetIdPCL(int selection) => _sets[selection].IdPCL;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t I n d e x F o r I d P C L                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the index of the Symbol Set Type associated with the        //
        // specified PCL identifier.                                          //
        //                                                                    //
        // Note that the length of the index array must be the same as that   //
        // of the definition array; the entries must be in the same order,    //
        // and the count of entries in the set must be the same as the count  //
        // of entries in these two arrays.                                    //
        //                                                                    //
        // If no match is found, a default index is returned.                 //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static Index GetIndexForIdPCL(byte idPCL)
        {
            string entry = Enum.GetName(typeof(IdPCL), idPCL);

            if (entry == null)
                return Index.Bound_PC8;
            else
                return (Index)Enum.Parse(typeof(Index), entry);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // i s B o u n d                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return indication of whether or not the symbol set type associated //
        // with the specified index is bound or unbound.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool IsBound(int selection) => _sets[selection].IsBound;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // p o p u l a t e S y m b o l S e t T y p e T a b l e                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Populate the table of Symbol Set Types.                            //
        //                                                                    //
        // Note that the length of the definition array must be the same as   //
        // that of the index array; the entries must be in the same order.    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void PopulateSymbolSetTypeTable()
        {
            _sets.AddRange(new PCLSymSetType[]
            {
                new PCLSymSetType(
                                (byte)IdPCL.Unknown,
                                _flagBound,
                                "<unknown>",
                                "<unknown>"),

                new PCLSymSetType(
                                (byte)IdPCL.Bound_7bit,
                                _flagBound,
                                "0: Bound; 7-bit (96 charset: 0x20-7f printable)",
                                "0: 7-bit"),

                new PCLSymSetType(
                                (byte)IdPCL.Bound_8bit,
                                _flagBound,
                                "1: Bound; 8-bit (192 charset: 0x20-7f, 0xa0-ff printable)",
                                "1: 8 bit"),

                new PCLSymSetType(
                                (byte)IdPCL.Bound_PC8,
                                _flagBound,
                                "2: Bound; PC-8 (256 charset: 0x01-06, 0x10-1a, 0x1c-ff printable)",
                                "2: 8-bit PC8"),

                new PCLSymSetType(
                                (byte)IdPCL.Bound_16bit,
                                _flagBound,
                                "3: Bound; 16-bit (65535 charset: (0x01-06, 0x10-1a, 0x1c-fffd printable)",
                                "3: 16-bit"),

                new PCLSymSetType(
                                (byte)IdPCL.Unbound_MSL,
                                _flagUnbound,
                                "10: Unbound; uses HP MSL numbers",
                                "10: index MSL"),

                new PCLSymSetType(
                                (byte)IdPCL.Unbound_Unicode,
                                _flagUnbound,
                                "11: Unbound; uses Unicode code points",
                                "11: index U+")
                }
            );

            _setsCountTotal = _sets.Count;
            _setsCountBound = 0;

            for (int i = 0; i < _setsCountTotal; i++)
            {
                if (_sets[i].IsBound == _flagBound)
                    _setsCountBound++;
            }
        }
    }
}