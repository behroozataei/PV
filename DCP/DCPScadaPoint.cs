using System;

namespace DCP
{
    public sealed class DCPScadaPoint
    {
        public DCPScadaPoint(Guid id, string name, string networkPath, PointDirectionType pointDirectionType)
        {
            Id = id;
            Name = name;
            NetworkPath = networkPath;
            PointDirectionType = pointDirectionType;
        }

        public string Name { get; }
        public string NetworkPath { get; }
        public Guid Id { get; }
        public int Quality { get; set; }
        public PointDirectionType PointDirectionType { get; }
        public float Value { get; set; }
    }

    public enum PointDirectionType
    {
        Input,
        Output
    }

    public enum DigitalStatus
    {
        Intransit = 0,
        Open = 1,
        Close = 2,
        Disturb = 3
    }

    public enum SinglePointStatus
    {
        Disappear = 0,
        Appear = 1
    }

    public sealed class GeneralModule
    {
        // Circuit Breaker / Disconnector Switch Status is:
        //   0: Invalid status
        //   1: Open
        //   2: Close
        //   0,Intransit;1,Open;2,Close;3,Disturbed
        public const int BREAKER_INVALID = 0;
        public const int BREAKER_OPEN = 1;
        public const int BREAKER_CLOSE = 2;

        public const int STATUS_OFF = 1;
        public const int STATUS_ON = 2;

        public const int STATUS_APPEARED = 2;
        public const int STATUS_DISAPPEARED = 1;

        public const string LOGFILEPATH = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\DC_Log\\";
        public const string LOGTRACEFILEPATH = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\DC_Log\\DC_Trace.txt";
        public const string LOGFILEPATHDESKAPP = "D:\\SIEMENS\\PowerCC\\Logs\\MSCFunctions\\DC_Log\\";


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
    }
}