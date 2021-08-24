using System;
using System.Collections.Generic;
using System.Text;

namespace OPC
{
    public class OPCDataEventArgs : EventArgs
    {
        public OPCDataEventArgs()
        {
            Items = new List<OPCDataEventArgs>();
        }
        public string OPCTagName { get; set; }
        public string ShortName { get; set; }
        public object Value { get; set; }
        public DateTime SourceTimestamp { get; set; }
        public string StatusCode { get; set; }
        public List<OPCDataEventArgs> Items { get; set; }
    }
}
