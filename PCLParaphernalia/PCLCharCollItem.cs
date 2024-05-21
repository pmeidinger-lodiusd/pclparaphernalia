using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PCLParaphernalia
{
    class PCLCharCollItem : INotifyPropertyChanged
    {
        private bool _isChecked;

        public event PropertyChangedEventHandler PropertyChanged;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L C h a r C o l l i t e m                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLCharCollItem(int bitNo,
                                PCLCharCollections.BitType bitType,
                                string desc,
                                bool isEnabled,
                                bool isChecked)
        {
            BitNo = bitNo;
            BitType = bitType;
            Desc = desc;
            IsEnabled = isEnabled;
            _isChecked = isChecked;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // B i t N o                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int BitNo { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // B i t T y p e                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLCharCollections.BitType BitType { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Desc { get; set; }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s C h e c k e d                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                _isChecked = value;

                onPropertyChanged(nameof(IsChecked));
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s E n a b l e d                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsEnabled { get; set; }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // o n P r o p e r t y C h a n g e d                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(
                    this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
