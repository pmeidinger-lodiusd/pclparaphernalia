﻿using System.Collections.ObjectModel;

namespace PCLParaphernalia
{
    /// <summary>
    ///
    /// <para>
    /// Class defines the initial / current settings of PCL Character
    /// Complement/Requirement Collection bits, as used in (unbound) Font
    /// headers and Symbol Set definitions.
    /// </para>
    /// <para>
    /// A font and a symbol set are compatible only if the result of ANDing the
    /// (64-bit) Character Complement field of the font definition with the
    /// (64-bit) Character Requirements field of the symbol set definition is
    /// 64 bits (8 bytes) of zero.
    /// </para>
    /// <para>
    /// In the descriptions below:
    ///     bit 0   is the least significant bit of the eighth byte
    ///     bit 63  is the most  significant bit of the first  byte
    /// </para>
    /// <para>© Chris Hutchinson 2013</para>
    ///
    /// </summary>
    internal class PCLCharCollItems
    {
        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d C o m p L i s t U n i c o d e                              //
        //                                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Character Complement bits for TrueType (Unicode-indexed) font.     //
        //                                                                    //
        // Bits 2,1,0:  Symbol index identifier: 1 1 0 = Unicode              //
        // Bits 63->3:  Collection compatibilty bits                          //
        //              unset: font is compatible with associated collection  //
        //              set:   font is not compatible with collection         //
        //                                                                    //
        // Because this is the Complement of the corresponding symbol set     //
        // Requirements field, the IsChecked field is set if the bit is NOT   //
        // set.                                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ObservableCollection<PCLCharCollItem> LoadCompListUnicode()
        {
            var objColEmp = new ObservableCollection<PCLCharCollItem>();

            PCLCharCollections.BitType bitType;
            int collsCt = PCLCharCollections.GetCollsCount();

            int bitNo;
            string desc;

            for (int i = 0; i < collsCt; i++)
            {
                bitNo = PCLCharCollections.GetBitNo(i);
                bitType = PCLCharCollections.GetBitType(i);
                desc = PCLCharCollections.GetDescUnicode(i);

                bool itemEnabled;
                bool itemChecked;

                if (bitType == PCLCharCollections.BitType.Index_0)
                {
                    itemEnabled = false;
                    itemChecked = true;
                }
                else if (bitType == PCLCharCollections.BitType.Index_1)
                {
                    itemEnabled = false;
                    itemChecked = false;
                }
                else if (bitType == PCLCharCollections.BitType.Index_2)
                {
                    itemEnabled = false;
                    itemChecked = false;
                }
                else
                {
                    itemEnabled = true;
                    itemChecked = false;
                }

                objColEmp.Add(new PCLCharCollItem(bitNo, bitType, desc, itemEnabled, itemChecked));
            }

            return objColEmp;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d R e q L i s t M S L                                        //
        //                                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Character Requirements bits for Intellifont (MSL-indexed) symbol   //
        // set.                                                               //
        //                                                                    //
        // Bits 2,1,0:  Symbol index identifier: 0 0 0 = MSL                  //
        // Bits 63->3:  Collection compatibilty bits                          //
        //              unset: associated collection is not required          //
        //              set:   associated collection is required              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ObservableCollection<PCLCharCollItem> LoadReqListMSL()
        {
            var objColEmp = new ObservableCollection<PCLCharCollItem>();

            PCLCharCollections.BitType bitType;
            int collsCt = PCLCharCollections.GetCollsCount();

            int bitNo;
            string desc;

            for (int i = 0; i < collsCt; i++)
            {
                bitNo = PCLCharCollections.GetBitNo(i);
                bitType = PCLCharCollections.GetBitType(i);
                desc = PCLCharCollections.GetDescMSL(i);

                bool itemEnabled;
                bool itemChecked;

                if (bitType == PCLCharCollections.BitType.Collection)
                {
                    itemEnabled = true;
                    itemChecked = false;
                }
                else
                {
                    itemEnabled = false;
                    itemChecked = false;
                }

                objColEmp.Add(new PCLCharCollItem(bitNo, bitType, desc, itemEnabled, itemChecked));
            }

            return objColEmp;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l o a d R e q L i s t U n i c o d e                                //
        //                                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Character Requirements bits for TrueType (Unicode-indexed) symbol  //
        // set.                                                               //
        //                                                                    //
        // Bits 2,1,0:  Symbol index identifier: 0 0 1 = Unicode              //
        // Bits 63->3:  Collection compatibilty bits                          //
        //              unset: associated collection is not required          //
        //              set:   associated collection is required              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public ObservableCollection<PCLCharCollItem> LoadReqListUnicode()
        {
            var objColEmp = new ObservableCollection<PCLCharCollItem>();

            PCLCharCollections.BitType bitType;
            int collsCt = PCLCharCollections.GetCollsCount();

            int bitNo;
            string desc;

            for (int i = 0; i < collsCt; i++)
            {
                bitNo = PCLCharCollections.GetBitNo(i);
                bitType = PCLCharCollections.GetBitType(i);
                desc = PCLCharCollections.GetDescUnicode(i);

                bool itemEnabled;
                bool itemChecked;

                if (bitType == PCLCharCollections.BitType.Collection)
                {
                    itemEnabled = true;
                    itemChecked = false;
                }
                else if (bitType == PCLCharCollections.BitType.Index_0)
                {
                    itemEnabled = false;
                    itemChecked = true;
                }
                else
                {
                    itemEnabled = false;
                    itemChecked = false;
                }

                objColEmp.Add(new PCLCharCollItem(bitNo, bitType, desc, itemEnabled, itemChecked));
            }

            return objColEmp;
        }
    }
}