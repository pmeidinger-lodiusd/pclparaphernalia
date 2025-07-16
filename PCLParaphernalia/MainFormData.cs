namespace PCLParaphernalia;

/// <summary>
/// 
/// Class manages temporary storage of common data for the main form.
/// 
/// © Chris Hutchinson 2011
/// 
/// </summary>

static class MainFormData
{
    //--------------------------------------------------------------------//
    //                                                        F i e l d s //
    // Fields (class variables).                                          //
    //                                                                    //
    //--------------------------------------------------------------------//

    private static double _windowScale = 1.0;

    private static bool _versionChange = false;

    private static int _versionMajorOld = -1;
    private static int _versionMinorOld = -1;
    private static int _versionBuildOld = -1;
    private static int _versionRevisionOld = -1;

    private static int _versionMajorCrnt = -1;
    private static int _versionMinorCrnt = -1;
    private static int _versionBuildCrnt = -1;
    private static int _versionRevisionCrnt = -1;

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // c h e c k O l d V e r s i o n                                      //
    //--------------------------------------------------------------------//
    //                                                                    //
    // This function checks if this is the first run of a new version,    //
    // and if so, if the old version was the one specified by the         //
    // supplied paprameters.                                              //
    //                                                                    //
    // Version data was first used after version 2.5.0.0                  //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool CheckOldVersion(int major,
                                           int minor,
                                           int build,
                                           int revision)
    {
        bool oldMatch = false;

        if (VersionChange)
        {
            if ((major == _versionMajorOld) &&
                (minor == _versionMinorOld) &&
                (build == _versionBuildOld) &&
                (revision == _versionRevisionOld))
            {
                oldMatch = true;
            }
        }

        return oldMatch;
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // g e t V e r s i o n D a t a                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Retrieve stored version data; first used after version 2.5.0.0     //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void GetVersionData(bool crnt,
                                       ref int major,
                                       ref int minor,
                                       ref int build,
                                       ref int revision)
    {
        if (crnt)
        {
            major = _versionMajorCrnt;
            minor = _versionMinorCrnt;
            build = _versionBuildCrnt;
            revision = _versionRevisionCrnt;
        }
        else
        {
            major = _versionMajorOld;
            minor = _versionMinorOld;
            build = _versionBuildOld;
            revision = _versionRevisionOld;
        }
    }

    //--------------------------------------------------------------------//
    //                                                        M e t h o d //
    // s e t V e r s i o n D a t a                                        //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Set version data; first used after version 2.5.0.0                 //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static void SetVersionData(bool crnt,
                                       int major,
                                       int minor,
                                       int build,
                                       int revision)
    {
        if (crnt)
        {
            _versionMajorCrnt = major;
            _versionMinorCrnt = minor;
            _versionBuildCrnt = build;
            _versionRevisionCrnt = revision;
        }
        else
        {
            _versionMajorOld = major;
            _versionMinorOld = minor;
            _versionBuildOld = build;
            _versionRevisionOld = revision;
        }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // V e r s i o n C h a n g e                                          //
    //--------------------------------------------------------------------//
    //                                                                    //
    // Return or set boolean indicating if version has changed.           //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static bool VersionChange
    {
        get { return _versionChange; }
        set
        {
            _versionChange = value;
        }
    }

    //--------------------------------------------------------------------//
    //                                                    P r o p e r t y //
    // W i n d o w S c a l e                                              //
    //                                                                    //
    //--------------------------------------------------------------------//

    public static double WindowScale
    {
        get { return _windowScale; }
        set
        {
            _windowScale = value;
        }
    }
}
