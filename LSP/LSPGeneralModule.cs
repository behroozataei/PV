//*************************************************************************************
// @author   Ali.A.Kaji
// @version  1.0
//
// Development Environment       : MS-Visual Basic 6.0
// Name of the Application       : EEC_Service_App.vbp
// Creation/Modification History :
//
// Ali.A.Kaji       23-May-2007       Created
//
// Overview of Application       :
//
//
//***************************************************************************************
using System;

namespace LSP
{
    internal enum eApp_Status
    {
        StatusOff = 1,
        StatusOn = 2
    }

    internal enum eApp_Disapp
    {
        Disappear = 0,
        Appear = 1
    }

    internal enum eShedPointQuality
    {
        Valid = 1,
        Substituted,
        Previous,
        Invalid
    }

    //
    internal enum eBreaker_Status
    {
        BIntransient = 0,
        BOpen = 1,
        BClose = 2,
        BDisturbed = 3
    }

    //
    internal enum eCombItem_Status
    {
        SDo_Not_Care = 0,
        SOff = 1,
        SOn = 2
    }

    // Types of load shedding
    internal enum eShedType
    {
        None = 0,
        SomeLoads = 1,
        AllLoads = 2
    }

    //
    internal enum eSYSStatus
    {
        SYSStat_Unknown = 0,
        SYSStat_Started = 1,
        SYSStat_Terminated = 2
    }

    //
    internal enum ENUM_STATUS
    {
        Disable = 0,
        Enable = 1
    }

    internal enum Breaker_Status
    {
        BIntransient = 0,
        BOpen = 1,
        bClose = 2,
        BDisturbed = 3
    }

    internal static class GeneralModule
    {
        // Variables need for ART
        public static object eEECState = null;

        // Circuit Breaker / Disconnector Switch Status is:
        //   0: Invalid status
        //   1: Open
        //   2: Close
        //   0:Intransit; 1:Open; 2:Close; 3:Disturbed
        //Public Const BREAKER_INVALID = 0
        //Public Const BREAKER_OPEN = 1
        //Public Const BREAKER_CLOSE = 2
        public static byte bOpen = (byte)Breaker_Status.BOpen;
        public static byte bClose = (byte)Breaker_Status.bClose;
        public static byte bDisturbed = (byte)Breaker_Status.BDisturbed;
        public static byte bTransient = (byte)Breaker_Status.BIntransient;

        //
        public const int STATUS_OFF = 1;
        public const int STATUS_ON = 2;

        public const int STATUS_APPEARED = 2;
        public const int STATUS_DISAPPEARED = 1;

        public const string LOGFILEPATH = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\LSP_Log\\";
        public const string LOGTRACEFILEPATH = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\LSP_Log\\LSP_Trace.txt";
        public const string LOGFILEPATHDESKAPP = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\LSP_Log\\";


        //==============================================================================
        // CONSTANTS for TCP Connections
        //==============================================================================

        // Port number for EEC Client
        public const int EECPortNo = 12120;

        // Port number for RPC Client
        public const int RPCPortNo = 12121;

        // Port number for PCS Client
        public const int PCSPortNo = 12122;

        // Port number for LSP Client
        public const int LSPPortNo = 12123;

        // IP Address for this machine, Two IP should be used, one for RTS1 and one for RTS2
        // Loopback ip: 127.0.0.1
        public const string strLocalIPDevelopment = "172.25.4.20";

        public const string strLocalIPRTS1 = "192.168.21.31";
        public const string strLocalIPRTS2 = "192.168.21.32";

        // Telegram ID's
        public const string TelegramID_EECReceived = "1A11";
        public const string TelegramID_RPCReceived = "1A12";
        public const string TelegramID_LSPReceived = "1A13";
        public const string TelegramID_EAFGroupReceived = "1A14";

        public const string TelegramID_EECSend = "1B11";
        public const string TelegramID_RPCSend = "1B12";
        public const string TelegramID_LSPSend = "1B13";
        public const string TelegramID_EAFGroupSend = "1B14";

        public const int WS_VERSION_REQD = 0x101;
        static readonly public int WS_VERSION_MAJOR = WS_VERSION_REQD / 0x100 & 0xFF;
        static readonly public int WS_VERSION_MINOR = WS_VERSION_REQD & 0xFF;
        public const int MIN_SOCKETS_REQD = 1;
        public const int WSADESCRIPTION_LEN = 256;
        public const int WSASYS_STATUS_LEN = 128;

        //Set the connection parameters for database

        //
        public static string DataSource = "";
        public static string UserName = "";
        public static string Password = "";

        internal static void setParams()
        {
            DataSource = "APPS";
            UserName = "MODEL";
            Password = "MODEL";
        }

        internal static string GetMachineName()
        {
            string result = "";
            try
            {
                string strBuffer = new string('\0', 512);
                int nLen = 0;

                nLen = strBuffer.Length;
                if (/*EECSupport.PInvoke.SafeNative.kernel32.GetComputerName(ref strBuffer, ref nLen)*/ strBuffer.Length != 0)
                {
                    /*strBuffer = StringsHelper.GetFixedLengthString(strBuffer, 512);*/
                    // Returns nonzero if successful, and modifies the length argument
                    result = strBuffer.Substring(0, Math.Min(nLen, strBuffer.Length));
                }
                else
                {
                    /*strBuffer = StringsHelper.GetFixedLengthString(strBuffer, 512);*/
                }
            }
            catch
            {
                result = " ";
            }

            return result;
        }

        internal static string BreakerStatus(byte aBreakerStat)
        {

            string result = "";
            if (aBreakerStat == ((byte)Breaker_Status.bClose))
            {
                result = "CLOSE";
            }
            else
            {
                if (aBreakerStat == ((byte)Breaker_Status.BDisturbed))
                {
                    result = "DISTURBED";
                }
                else
                {
                    if (aBreakerStat == ((byte)Breaker_Status.BIntransient))
                    {
                        result = "INTRANSIENT";
                    }
                    else
                    {
                        if (aBreakerStat == ((byte)Breaker_Status.BOpen))
                        {
                            result = "OPEN";
                        }
                    }
                }
            }

            return result;
        }

        // The maximum number of LSP Params which are listed in the T_CLSPPARAMS
        public const int nPOINTS = 250;

        // The maximum number of LSP Shed Points which are listed in the T_COCSHEDPOINTS
        public const int nSHEDPOINTS = 100;

        // Array used to store the Shed Data Points from T_CLSPSHEDPOINT
        ////public static SCADAPoint[] arrShedPoint = new SCADAPoint[nSHEDPOINTS + 1];

        // Array used to store the data from T_CLSPPARAMS
        public static string[,] arrLSPParams = new string[nPOINTS + 1, 3];

        public const string strOverloadMessage = "OVERLOAD APPEARED";

        public const string str4ValMessage = "OVERLOAD WARNING";

        public static int MissedShotNo = 0;

        public static int LastTrueCycleNo = 0;

        public static System.DateTime LastTrueTime = DateTime.FromOADate(0);

        public static bool SkipReadEval = false;
    }
}