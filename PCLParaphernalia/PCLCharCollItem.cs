﻿using System.ComponentModel;

namespace PCLParaphernalia
{
    class PCLCharCollItem : INotifyPropertyChanged
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        private PCLCharCollections.eBitType _bitType;

        private int _bitNo;

        private string _desc;

        private bool _isChecked;
        private bool _isEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        //--------------------------------------------------------------------//
        //                                              C o n s t r u c t o r //
        // P C L C h a r C o l l i t e m                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLCharCollItem(int bitNo,
                                PCLCharCollections.eBitType bitType,
                                string desc,
                                bool isEnabled,
                                bool isChecked)
        {
            _bitNo = bitNo;
            _bitType = bitType;
            _desc = desc;
            _isEnabled = isEnabled;
            _isChecked = isChecked;
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // B i t N o                                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        public int BitNo
        {
            get { return _bitNo; }
            set { _bitNo = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // B i t T y p e                                                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public PCLCharCollections.eBitType BitType
        {
            get { return _bitType; }
            set { _bitType = value; }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // D e s c                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public string Desc
        {
            get { return _desc; }
            set { _desc = value; }
        }

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

                OnPropertyChanged("IsChecked");
            }
        }

        //--------------------------------------------------------------------//
        //                                                    P r o p e r t y //
        // I s E n a b l e d                                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        public bool IsEnabled
        {
            get { return _isEnabled; }

            set { _isEnabled = value; }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // o n P r o p e r t y C h a n g e d                                  //
        //                                                                    //
        //--------------------------------------------------------------------//

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
