﻿using System;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>Class defines a set of PCL Logical Operation objects.</para>
    /// <para>© Chris Hutchinson 2014</para>
    ///
    /// </summary>
    static class PCLLogicalOperations
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Class variables.                                                   //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static readonly PCLLogicalOperation[] _rops =
        {
            new PCLLogicalOperation (  0, 0x0042, "0"          , "Black"),
            new PCLLogicalOperation (  1, 0x0289, "DTSoon"     , "~(D | (T | S))"),
            new PCLLogicalOperation (  2, 0x0c89, "DTSona"     , "D & ~(T | S)"),
            new PCLLogicalOperation (  3, 0x00aa, "TSon"       , "~(T | S)"),
            new PCLLogicalOperation (  4, 0x0c88, "SDTona"     , "S & ~(D | T)"),
            new PCLLogicalOperation (  5, 0x00a9, "DTon"       , "~(D | T)"),
            new PCLLogicalOperation (  6, 0x0865, "TDSxnon"    , "~(T | ~(D ^ S))"),
            new PCLLogicalOperation (  7, 0x02c5, "TDSaon"     , "~(T | (D & S))"),
            new PCLLogicalOperation (  8, 0x0f08, "SDTnaa"     , "S & (D & ~T)"),
            new PCLLogicalOperation (  9, 0x0245, "TDSxon"     , "~(T | (D ^ S))"),
            new PCLLogicalOperation ( 10, 0x0329, "DTna"       , "D & ~T"),
            new PCLLogicalOperation ( 11, 0x0b2a, "TSDnaon"    , "~(T | (S & ~D))"),
            new PCLLogicalOperation ( 12, 0x0324, "STna"       , "S & ~T"),
            new PCLLogicalOperation ( 13, 0x0b25, "TDSnaon"    , "~(T | (D & ~S))"),
            new PCLLogicalOperation ( 14, 0x08a5, "TDSonon"    , "~(T | ~(D | S))"),
            new PCLLogicalOperation ( 15, 0x0001, "Tn"         , "~T"),
            new PCLLogicalOperation ( 16, 0x0c85, "TDSona"     , "T & ~(D | S)"),
            new PCLLogicalOperation ( 17, 0x00a6, "DSon"       , "~(D | S)"),
            new PCLLogicalOperation ( 18, 0x0868, "SDTxnon"    , "~(S | ~(D ^ T))"),
            new PCLLogicalOperation ( 19, 0x02c8, "SDTaon"     , "~(S | (D & T))"),
            new PCLLogicalOperation ( 20, 0x0869, "DTSxnon"    , "~(D | ~(T ^ S))"),
            new PCLLogicalOperation ( 21, 0x02c9, "DTSaon"     , "~(D | (T & S))"),
            new PCLLogicalOperation ( 22, 0x5cca, "TSDTSanaxx" , "T ^ (S ^ (D & ~(T & S)))"),
            new PCLLogicalOperation ( 23, 0x1d54, "SSTxDSxaxn" , "~(S ^ ((S ^ T) & (D ^ S)))"),
            new PCLLogicalOperation ( 24, 0x0d59, "STxTDxa"    , "(S ^ T) & (T ^ D)"),
            new PCLLogicalOperation ( 25, 0x1cc8, "SDTSanaxn"  , "~(S ^ (D & ~(T & S)))"),
            new PCLLogicalOperation ( 26, 0x06c5, "TDSTaox"    , "T ^ (D | (S & T))"),
            new PCLLogicalOperation ( 27, 0x0768, "SDTSxaxn"   , "~(S ^ (D & (T ^ S)))"),
            new PCLLogicalOperation ( 28, 0x06ca, "TSDTaox"    , "T ^ (S | (D & T))"),
            new PCLLogicalOperation ( 29, 0x0766, "DSTDxaxn"   , "~(D ^ (S & (T ^ D)))"),
            new PCLLogicalOperation ( 30, 0x01a5, "TDSox"      , "T ^ (D | S)"),
            new PCLLogicalOperation ( 31, 0x0385, "TDSoan"     , "~(T & (D | S))"),
            new PCLLogicalOperation ( 32, 0x0f09, "DTSnaa"     , "D & (T & ~S)"),
            new PCLLogicalOperation ( 33, 0x0248, "SDTxon"     , "~(S | (D ^ T))"),
            new PCLLogicalOperation ( 34, 0x0326, "DSna"       , "D & ~S"),
            new PCLLogicalOperation ( 35, 0x0b24, "STDnaon"    , "~(S | (T & ~D))"),
            new PCLLogicalOperation ( 36, 0x0d55, "STxDSxa"    , "(S ^ T) & (D ^ S)"),
            new PCLLogicalOperation ( 37, 0x1cc5, "TDSTanaxn"  , "~(T ^ (D & ~(S & T)))"),
            new PCLLogicalOperation ( 38, 0x06c8, "SDTSaox"    , "S ^ (D | (T & S))"),
            new PCLLogicalOperation ( 39, 0x1868, "SDTSxnox"   , "S ^ (D | ~(T ^ S))"),
            new PCLLogicalOperation ( 40, 0x0369, "DTSxa"      , "D & (T ^ S)"),
            new PCLLogicalOperation ( 41, 0x16ca, "TSDTSaoxxn" , "~(T ^ (S ^ (D | (T & S))))"),
            new PCLLogicalOperation ( 42, 0x0cc9, "DTSana"     , "D & ~(T & S)"),
            new PCLLogicalOperation ( 43, 0x1d58, "SSTxTDxaxn" , "~(S ^ ((S ^ T) & (T ^ D)))"),
            new PCLLogicalOperation ( 44, 0x0784, "STDSoax"    , "S ^ (T & (D | S))"),
            new PCLLogicalOperation ( 45, 0x060a, "TSDnox"     , "T ^ (S | ~D)"),
            new PCLLogicalOperation ( 46, 0x064a, "TSDTxox"    , "T ^ (S | (D ^ T))"),
            new PCLLogicalOperation ( 47, 0x0e2a, "TSDnoan"    , "~(T & (S | ~D))"),
            new PCLLogicalOperation ( 48, 0x032a, "TSna"       , "T & ~S"),
            new PCLLogicalOperation ( 49, 0x0b28, "SDTnaon"    , "~(S | (D & ~T))"),
            new PCLLogicalOperation ( 50, 0x0688, "SDTSoox"    , "S ^ (D | (T | S))"),
            new PCLLogicalOperation ( 51, 0x0008, "Sn"         , "~S"),
            new PCLLogicalOperation ( 52, 0x06c4, "STDSaox"    , "S ^ (T | (D & S))"),
            new PCLLogicalOperation ( 53, 0x1864, "STDSxnox"   , "S ^ (T | ~(D ^ S))"),
            new PCLLogicalOperation ( 54, 0x01a8, "SDTox"      , "S ^ (D | T)"),
            new PCLLogicalOperation ( 55, 0x0388, "SDToan"     , "~(S & (D | T))"),
            new PCLLogicalOperation ( 56, 0x078a, "TSDToax"    , "T ^ (S & (D | T))"),
            new PCLLogicalOperation ( 57, 0x0604, "STDnox"     , "S ^ (T | ~D)"),
            new PCLLogicalOperation ( 58, 0x0644, "STDSxox"    , "S ^ (T | (D ^ S))"),
            new PCLLogicalOperation ( 59, 0x0e24, "STDnoan"    , "~(S & (T | ~D))"),
            new PCLLogicalOperation ( 60, 0x004a, "TSx"        , "T ^ S"),
            new PCLLogicalOperation ( 61, 0x18a4, "STDSonox"   , "S ^ (T | ~(D | S))"),
            new PCLLogicalOperation ( 62, 0x1b24, "STDSnaox"   , "S ^ (T | (D & ~S))"),
            new PCLLogicalOperation ( 63, 0x00ea, "TSan"       , "~(T & S)"),
            new PCLLogicalOperation ( 64, 0x0f0a, "TSDnaa"     , "T & (S & ~D)"),
            new PCLLogicalOperation ( 65, 0x0249, "DTSxon"     , "~(D | (T ^ S))"),
            new PCLLogicalOperation ( 66, 0x0d5d, "SDxTDxa"    , "(S ^ D) & (T ^ D)"),
            new PCLLogicalOperation ( 67, 0x1cc4, "STDSanaxn"  , "~(S ^ (T & ~(D & S)))"),
            new PCLLogicalOperation ( 68, 0x0328, "SDna"       , "S & ~D"),
            new PCLLogicalOperation ( 69, 0x0b29, "DTSnaon"    , "~(D | (T & ~S))"),
            new PCLLogicalOperation ( 70, 0x06c6, "DSTDaox"    , "D ^ (S | (T & D))"),
            new PCLLogicalOperation ( 71, 0x076a, "TSDTxaxn"   , "~(T ^ (S & (D ^ T)))"),
            new PCLLogicalOperation ( 72, 0x0368, "SDTxa"      , "S & (D ^ T)"),
            new PCLLogicalOperation ( 73, 0x16c5, "TDSTDaoxxn" , "~(T ^ (D ^ (S | (T & D))))"),
            new PCLLogicalOperation ( 74, 0x0789, "DTSDoax"    , "D ^ (T & (S | D))"),
            new PCLLogicalOperation ( 75, 0x0605, "TDSnox"     , "T ^ (D | ~S)"),
            new PCLLogicalOperation ( 76, 0x0cc8, "SDTana"     , "S & ~(D & T)"),
            new PCLLogicalOperation ( 77, 0x1954, "SSTxDSxoxn" , "~(S ^ ((S ^ T) | (D ^ S)))"),
            new PCLLogicalOperation ( 78, 0x0645, "TDSTxox"    , "T ^ (D | (S ^ T))"),
            new PCLLogicalOperation ( 79, 0x0e25, "TDSnoan"    , "~(T & (D | ~S))"),
            new PCLLogicalOperation ( 80, 0x0325, "TDna"       , "T & ~D"),
            new PCLLogicalOperation ( 81, 0x0b26, "DSTnaon"    , "~(D | (S & ~T))"),
            new PCLLogicalOperation ( 82, 0x06c9, "DTSDaox"    , "D ^ (T | (S & D))"),
            new PCLLogicalOperation ( 83, 0x0764, "STDSxaxn"   , "~(S ^ (T & (D ^ S)))"),
            new PCLLogicalOperation ( 84, 0x08a9, "DTSonon"    , "~(D | ~(T | S))"),
            new PCLLogicalOperation ( 85, 0x0009, "Dn"         , "~D"),
            new PCLLogicalOperation ( 86, 0x01a9, "DTSox"      , "D ^ (T | S)"),
            new PCLLogicalOperation ( 87, 0x0389, "DTSoan"     , "~(D & (T | S))"),
            new PCLLogicalOperation ( 88, 0x0785, "TDSToax"    , "T ^ (D & (S | T))"),
            new PCLLogicalOperation ( 89, 0x0609, "DTSnox"     , "D ^ (T | ~S)"),
            new PCLLogicalOperation ( 90, 0x0049, "DTx"        , "D ^ T"),
            new PCLLogicalOperation ( 91, 0x18a9, "DTSDonox"   , "D ^ (T | ~(S | D))"),
            new PCLLogicalOperation ( 92, 0x0649, "DTSDxox"    , "D ^ (T | (S ^ D))"),
            new PCLLogicalOperation ( 93, 0x0e29, "DTSnoan"    , "~(D & (T | ~S))"),
            new PCLLogicalOperation ( 94, 0x1b29, "DTSDnaox"   , "D ^ (T | (S & ~D))"),
            new PCLLogicalOperation ( 95, 0x00e9, "DTan"       , "~(D & T)"),
            new PCLLogicalOperation ( 96, 0x0365, "TDSxa"      , "T & (D ^ S)"),
            new PCLLogicalOperation ( 97, 0x16c6, "DSTDSaoxxn" , "~(D ^ (S ^ (T | (D & S))))"),
            new PCLLogicalOperation ( 98, 0x0786, "DSTDoax"    , "D ^ (S & (T | D))"),
            new PCLLogicalOperation ( 99, 0x0608, "SDTnox"     , "S ^ (D | ~T)"),
            new PCLLogicalOperation (100, 0x0788, "SDTSoax"    , "S ^ (D & (T | S))"),
            new PCLLogicalOperation (101, 0x0606, "DSTnox"     , "D ^ (S | ~T)"),
            new PCLLogicalOperation (102, 0x0046, "DSx"        , "D ^ S"),
            new PCLLogicalOperation (103, 0x18a8, "SDTSonox"   , "S ^ (D | ~(T | S))"),
            new PCLLogicalOperation (104, 0x58a6, "DSTDSonoxxn", "~(D ^ (S ^ (T | ~(D | S))))"),
            new PCLLogicalOperation (105, 0x0145, "TDSxxn"     , "~(T ^ (D ^ S))"),
            new PCLLogicalOperation (106, 0x01e9, "DTSax"      , "D ^ (T & S)"),
            new PCLLogicalOperation (107, 0x178a, "TSDTSoaxxn" , "~(T ^ (S ^ (D & (T | S))))"),
            new PCLLogicalOperation (108, 0x01e8, "SDTax"      , "S ^ (D & T)"),
            new PCLLogicalOperation (109, 0x1785, "TDSTDoaxxn" , "~(T ^ (D ^ (S & (T | D))))"),
            new PCLLogicalOperation (110, 0x1e28, "SDTSnoax"   , "S ^ (D & (T | ~S))"),
            new PCLLogicalOperation (111, 0x0c65, "TDSxnan"    , "~(T & ~(D ^ S))"),
            new PCLLogicalOperation (112, 0x0cc5, "TDSana"     , "T & ~(D & S)"),
            new PCLLogicalOperation (113, 0x1d5c, "SSDxTDxaxn" , "~(S ^ ((S ^ D) & (T ^ D)))"),
            new PCLLogicalOperation (114, 0x0648, "SDTSxox"    , "S ^ (D | (T ^ S))"),
            new PCLLogicalOperation (115, 0x0e28, "SDTnoan"    , "~(S & (D | ~T))"),
            new PCLLogicalOperation (116, 0x0646, "DSTDxox"    , "D ^ (S | (T ^ D))"),
            new PCLLogicalOperation (117, 0x0e26, "DSTnoan"    , "~(D & (S | ~T))"),
            new PCLLogicalOperation (118, 0x1b28, "SDTSnaox"   , "S ^ (D | (T & ~S))"),
            new PCLLogicalOperation (119, 0x00e6, "DSan"       , "~(D & S)"),
            new PCLLogicalOperation (120, 0x01e5, "TDSax"      , "T ^ (D & S)"),
            new PCLLogicalOperation (121, 0x1786, "DSTDSoaxxn" , "~(D ^ (S ^ (T & (D | S))))"),
            new PCLLogicalOperation (122, 0x1e29, "DTSDnoax"   , "D ^ (T & (S | ~D))"),
            new PCLLogicalOperation (123, 0x0c68, "SDTxnan"    , "~(S & ~(D ^ T))"),
            new PCLLogicalOperation (124, 0x1e24, "STDSnoax"   , "S ^ (T & (D | ~S))"),
            new PCLLogicalOperation (125, 0x0c69, "DTSxnan"    , "~(D & ~(T ^ S))"),
            new PCLLogicalOperation (126, 0x0955, "STxDSxo"    , "(S ^ T) | (D ^ S)"),
            new PCLLogicalOperation (127, 0x03c9, "DTSaan"     , "~(D & (T & S))"),
            new PCLLogicalOperation (128, 0x03e9, "DTSaa"      , "D & (T & S)"),
            new PCLLogicalOperation (129, 0x0975, "STxDSxon"   , "~((S ^ T) | (D ^ S))"),
            new PCLLogicalOperation (130, 0x0c49, "DTSxna"     , "D & ~(T ^ S)"),
            new PCLLogicalOperation (131, 0x1e04, "STDSnoaxn"  , "~(S ^ (T & (D | ~S)))"),
            new PCLLogicalOperation (132, 0x0c48, "SDTxna"     , "S & ~(D ^ T)"),
            new PCLLogicalOperation (133, 0x1e05, "TDSTnoaxn"  , "~(T ^ (D & (S | ~T)))"),
            new PCLLogicalOperation (134, 0x17a6, "DSTDSoaxx"  , "D ^ (S ^ (T & (D | S)))"),
            new PCLLogicalOperation (135, 0x01c5, "TDSaxn"     , "~(T ^ (D & S))"),
            new PCLLogicalOperation (136, 0x00c6, "DSa"        , "D & S"),
            new PCLLogicalOperation (137, 0x1b08, "SDTSnaoxn"  , "~(S ^ (D | (T & ~S)))"),
            new PCLLogicalOperation (138, 0x0e06, "DSTnoa"     , "D & (S | ~T)"),
            new PCLLogicalOperation (139, 0x0666, "DSTDxoxn"   , "~(D ^ (S | (T ^ D)))"),
            new PCLLogicalOperation (140, 0x0e08, "SDTnoa"     , "S & (D | ~T)"),
            new PCLLogicalOperation (141, 0x0668, "SDTSxoxn"   , "~(S ^ (D | (T ^ S)))"),
            new PCLLogicalOperation (142, 0x1d7c, "SSDxTDxax"  , "S ^ ((S ^ D) & (T ^ D))"),
            new PCLLogicalOperation (143, 0x0ce5, "TDSanan"    , "~(T & ~(D & S))"),
            new PCLLogicalOperation (144, 0x0c45, "TDSxna"     , "T & ~(D ^ S)"),
            new PCLLogicalOperation (145, 0x1e08, "SDTSnoaxn"  , "~(S ^ (D & (T | ~S)))"),
            new PCLLogicalOperation (146, 0x17a9, "DTSDToaxx"  , "D ^ (T ^ (S & (D | T)))"),
            new PCLLogicalOperation (147, 0x01c4, "STDaxn"     , "~(S ^ (T & D))"),
            new PCLLogicalOperation (148, 0x17aa, "TSDTSoaxx"  , "T ^ (S ^ (D & (T | S)))"),
            new PCLLogicalOperation (149, 0x01c9, "DTSaxn"     , "~(D ^ (T & S))"),
            new PCLLogicalOperation (150, 0x0169, "DTSxx"      , "D ^ (T ^ S)"),
            new PCLLogicalOperation (151, 0x588a, "TSDTSonoxx" , "T ^ (S ^ (D | ~(T | S)))"),
            new PCLLogicalOperation (152, 0x1888, "SDTSonoxn"  , "~(S ^ (D | ~(T | S)))"),
            new PCLLogicalOperation (153, 0x0066, "DSxn"       , "~(D ^ S)"),
            new PCLLogicalOperation (154, 0x0709, "DTSnax"     , "D ^ (T & ~S)"),
            new PCLLogicalOperation (155, 0x07a8, "SDTSoaxn"   , "~(S ^ (D & (T | S)))"),
            new PCLLogicalOperation (156, 0x0704, "STDnax"     , "S ^ (T & ~D)"),
            new PCLLogicalOperation (157, 0x07a6, "DSTDoaxn"   , "~(D ^ (S & (T | D)))"),
            new PCLLogicalOperation (158, 0x16e6, "DSTDSaoxx"  , "D ^ (S ^ (T | (D & S)))"),
            new PCLLogicalOperation (159, 0x0345, "TDSxan"     , "~(T & (D ^ S))"),
            new PCLLogicalOperation (160, 0x00c9, "DTa"        , "D & T"),
            new PCLLogicalOperation (161, 0x1b05, "TDSTnaoxn"  , "~(T ^ (D | (S & ~T)))"),
            new PCLLogicalOperation (162, 0x0e09, "DTSnoa"     , "D & (T | ~S)"),
            new PCLLogicalOperation (163, 0x0669, "DTSDxoxn"   , "~(D ^ (T | (S ^ D)))"),
            new PCLLogicalOperation (164, 0x1885, "TDSTonoxn"  , "~(T ^ (D | ~(S | T)))"),
            new PCLLogicalOperation (165, 0x0065, "TDxn"       , "~(T ^ D)"),
            new PCLLogicalOperation (166, 0x0706, "DSTnax"     , "D ^ (S & ~T)"),
            new PCLLogicalOperation (167, 0x07a5, "TDSToaxn"   , "~(T ^ (D & (S | T)))"),
            new PCLLogicalOperation (168, 0x03a9, "DTSoa"      , "D & (T | S)"),
            new PCLLogicalOperation (169, 0x0189, "DTSoxn"     , "~(D ^ (T | S))"),
            new PCLLogicalOperation (170, 0x0029, "D"          , "D"),
            new PCLLogicalOperation (171, 0x0889, "DTSono"     , "D | ~(T | S)"),
            new PCLLogicalOperation (172, 0x0744, "STDSxax"    , "S ^ (T & (D ^ S))"),
            new PCLLogicalOperation (173, 0x06e9, "DTSDaoxn"   , "~(D ^ (T | (S & D)))"),
            new PCLLogicalOperation (174, 0x0b06, "DSTnao"     , "D | (S & ~T)"),
            new PCLLogicalOperation (175, 0x0229, "DTno"       , "D | ~T"),
            new PCLLogicalOperation (176, 0x0e05, "TDSnoa"     , "T & (D | ~S)"),
            new PCLLogicalOperation (177, 0x0665, "TDSTxoxn"   , "~(T ^ (D | (S ^ T)))"),
            new PCLLogicalOperation (178, 0x1974, "SSTxDSxox"  , "S ^ ((S ^ T) | (D ^ S))"),
            new PCLLogicalOperation (179, 0x0ce8, "SDTanan"    , "~(S & ~(D & T))"),
            new PCLLogicalOperation (180, 0x070a, "TSDnax"     , "T ^ (S & ~D)"),
            new PCLLogicalOperation (181, 0x07a9, "DTSDoaxn"   , "~(D ^ (T & (S | D)))"),
            new PCLLogicalOperation (182, 0x16e9, "DTSDTaoxx"  , "D ^ (T ^ (S | (D & T)))"),
            new PCLLogicalOperation (183, 0x0348, "SDTxan"     , "~(S & (D ^ T))"),
            new PCLLogicalOperation (184, 0x074a, "TSDTxax"    , "T ^ (S & (D ^ T))"),
            new PCLLogicalOperation (185, 0x06e6, "DSTDaoxn"   , "~(D ^ (S | (T & D)))"),
            new PCLLogicalOperation (186, 0x0b09, "DTSnao"     , "D | (T & ~S)"),
            new PCLLogicalOperation (187, 0x0226, "DSno"       , "D | ~S"),
            new PCLLogicalOperation (188, 0x1ce4, "STDSanax"   , "S ^ (T & ~(D & S))"),
            new PCLLogicalOperation (189, 0x0d7d, "SDxTDxan"   , "~((S ^ D) & (T ^ D))"),
            new PCLLogicalOperation (190, 0x0269, "DTSxo"      , "D | (T ^ S)"),
            new PCLLogicalOperation (191, 0x08c9, "DTSano"     , "D | ~(T & S)"),
            new PCLLogicalOperation (192, 0x00ca, "TSa"        , "T & S"),
            new PCLLogicalOperation (193, 0x1b04, "STDSnaoxn"  , "~(S ^ (T | (D & ~S)))"),
            new PCLLogicalOperation (194, 0x1884, "STDSonoxn"  , "~(S ^ (T | ~(D | S)))"),
            new PCLLogicalOperation (195, 0x006a, "TSxn"       , "~(T ^ S)"),
            new PCLLogicalOperation (196, 0x0e04, "STDnoa"     , "S & (T | ~D)"),
            new PCLLogicalOperation (197, 0x0664, "STDSxoxn"   , "~(S ^ (T | (D ^ S)))"),
            new PCLLogicalOperation (198, 0x0708, "SDTnax"     , "S ^ (D & ~T)"),
            new PCLLogicalOperation (199, 0x07aa, "TSDToaxn"   , "~(T ^ (S & (D | T)))"),
            new PCLLogicalOperation (200, 0x03a8, "SDToa"      , "S & (D | T)"),
            new PCLLogicalOperation (201, 0x0184, "STDoxn"     , "~(S ^ (T | D))"),
            new PCLLogicalOperation (202, 0x0749, "DTSDxax"    , "D ^ (T & (S ^ D))"),
            new PCLLogicalOperation (203, 0x06e4, "STDSaoxn"   , "~(S ^ (T | (D & S)))"),
            new PCLLogicalOperation (204, 0x0020, "S"          , "S"),
            new PCLLogicalOperation (205, 0x0888, "SDTono"     , "S | ~(D | T)"),
            new PCLLogicalOperation (206, 0x0b08, "SDTnao"     , "S | (D & ~T)"),
            new PCLLogicalOperation (207, 0x0224, "STno"       , "S | ~T"),
            new PCLLogicalOperation (208, 0x0e0a, "TSDnoa"     , "T & (S | ~D)"),
            new PCLLogicalOperation (209, 0x066a, "TSDTxoxn"   , "~(T ^ (S | (D ^ T)))"),
            new PCLLogicalOperation (210, 0x0705, "TDSnax"     , "T ^ (D & ~S)"),
            new PCLLogicalOperation (211, 0x07a4, "STDSoaxn"   , "~(S ^ (T & (D | S)))"),
            new PCLLogicalOperation (212, 0x1d78, "SSTxTDxax"  , "S ^ ((S ^ T) & (T ^ D))"),
            new PCLLogicalOperation (213, 0x0ce9, "DTSanan"    , "~(D & ~(T & S))"),
            new PCLLogicalOperation (214, 0x16ea, "TSDTSaoxx"  , "T ^ (S ^ (D | (T & S)))"),
            new PCLLogicalOperation (215, 0x0349, "DTSxan"     , "~(D & (T ^ S))"),
            new PCLLogicalOperation (216, 0x0745, "TDSTxax"    , "T ^ (D & (S ^ T))"),
            new PCLLogicalOperation (217, 0x06e8, "SDTSaoxn"   , "~(S ^ (D | (T & S)))"),
            new PCLLogicalOperation (218, 0x1ce9, "DTSDanax"   , "D ^ (T & ~(S & D))"),
            new PCLLogicalOperation (219, 0x0d75, "STxDSxan"   , "~((S ^ T) & (D ^ S))"),
            new PCLLogicalOperation (220, 0x0b04, "STDnao"     , "S | (T & ~D)"),
            new PCLLogicalOperation (221, 0x0228, "SDno"       , "S | ~D"),
            new PCLLogicalOperation (222, 0x0268, "SDTxo"      , "S | (D ^ T)"),
            new PCLLogicalOperation (223, 0x08c8, "SDTano"     , "S | ~(D & T)"),
            new PCLLogicalOperation (224, 0x03a5, "TDSoa"      , "T & (D | S)"),
            new PCLLogicalOperation (225, 0x0185, "TDSoxn"     , "~(T ^ (D | S))"),
            new PCLLogicalOperation (226, 0x0746, "DSTDxax"    , "D ^ (S & (T ^ D))"),
            new PCLLogicalOperation (227, 0x06ea, "TSDTaoxn"   , "~(T ^ (S | (D & T)))"),
            new PCLLogicalOperation (228, 0x0748, "SDTSxax"    , "S ^ (D & (T ^ S))"),
            new PCLLogicalOperation (229, 0x06e5, "TDSTaoxn"   , "~(T ^ (D | (S & T)))"),
            new PCLLogicalOperation (230, 0x1ce8, "SDTSanax"   , "S ^ (D & ~(T & S))"),
            new PCLLogicalOperation (231, 0x0d79, "STxTDxan"   , "~((S ^ T) & (T ^ D))"),
            new PCLLogicalOperation (232, 0x1d74, "SSTxDSxax"  , "S ^ ((S ^ T) & (D ^ S))"),
            new PCLLogicalOperation (233, 0x5ce6, "DSTDSanaxxn", "~(D ^ (S ^ (T & ~(D & S))))"),
            new PCLLogicalOperation (234, 0x02e9, "DTSao"      , "D | (T & S)"),
            new PCLLogicalOperation (235, 0x0849, "DTSxno"     , "D | ~(T ^ S)"),
            new PCLLogicalOperation (236, 0x02e8, "SDTao"      , "S | (D & T)"),
            new PCLLogicalOperation (237, 0x0848, "SDTxno"     , "S | ~(D ^ T)"),
            new PCLLogicalOperation (238, 0x0086, "DSo"        , "D | S"),
            new PCLLogicalOperation (239, 0x0a08, "SDTnoo"     , "S | (D | ~T)"),
            new PCLLogicalOperation (240, 0x0021, "T"          , "T"),
            new PCLLogicalOperation (241, 0x0885, "TDSono"     , "T | ~(D | S)"),
            new PCLLogicalOperation (242, 0x0b05, "TDSnao"     , "T | (D & ~S)"),
            new PCLLogicalOperation (243, 0x022a, "TSno"       , "T | ~S"),
            new PCLLogicalOperation (244, 0x0b0a, "TSDnao"     , "T | (S & ~D)"),
            new PCLLogicalOperation (245, 0x0225, "TDno"       , "T | ~D"),
            new PCLLogicalOperation (246, 0x0265, "TDSxo"      , "T | (D ^ S)"),
            new PCLLogicalOperation (247, 0x08c5, "TDSano"     , "T | ~(D & S)"),
            new PCLLogicalOperation (248, 0x02e5, "TDSao"      , "T | (D & S)"),
            new PCLLogicalOperation (249, 0x0845, "TDSxno"     , "T | ~(D ^ S)"),
            new PCLLogicalOperation (250, 0x0089, "DTo"        , "D | T"),
            new PCLLogicalOperation (251, 0x0a09, "DTSnoo"     , "D | (T | ~S)"),
            new PCLLogicalOperation (252, 0x008a, "TSo"        , "T | S"),
            new PCLLogicalOperation (253, 0x0a0a, "TSDnoo"     , "T | (S | ~D)"),
            new PCLLogicalOperation (254, 0x02a9, "DTSoo"      , "D | (T | S)"),
            new PCLLogicalOperation (255, 0x0062, "1"          , "White")
        };

        private static readonly int _ropCount = _rops.GetUpperBound(0) + 1;

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // A c t I n f i x                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the Infix representation of the action associated with this //
        // operator.                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string ActInfix(int index)
        {
            return _rops[index].ActInfix;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t C o u n t                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return count of Logical Operation definitions.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static int GetCount()
        {
            return _ropCount;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c L o n g                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the long form of the ROP description associated with the    //
        // specified Logical Operation index; this includes:                  //
        //  - the ROP index                                                   //
        //  - the Postfix representation of the action                        //
        //  - the Infix representation of the action                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDescLong(int index)
        {
            return _rops[index].GetDescLong();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t D e s c S h o r t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the short form of the ROP description associated with the   //
        // specified Logical Operation index; this includes:                  //
        //  - the ROP index                                                   //
        //  - the Postfix representation of the action                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static string GetDescShort(int index)
        {
            return _rops[index].getDescShort();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // g e t R O P I d                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Return the ROP identifier of the ROP associated with the specified //
        // Logical Operation index.                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static short GetROPId(int index)
        {
            return _rops[index].GetROPId();
        }
    }
}