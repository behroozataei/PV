using System;

namespace DCP
{
    internal class RPCTelegram
    {
        public string m_TelegramID = "";
        public int m_SVCValue = 0;
        public int m_OverFlux = 0;
        public float m_PowerFactor1 = 0;
        public float m_PowerFactor2 = 0;

        // This sub compose all fields into a string for sending via a Socket
        public void PrepareToSend(ref string strData)
        {
            //Mixing all data parts:

            strData = GeneralModule.TelegramID_RPCSend + " ; ";
            strData = strData + m_SVCValue.ToString() + " ; ";
            strData = strData + m_OverFlux.ToString() + " ; ";
            strData = strData + m_PowerFactor1.ToString() + " ; ";
            strData = strData + m_PowerFactor2.ToString() + " ; ";
        }

        // This sub extract all fields from a string just received via a Socket
        public bool PrepareToUse(string strData)
        {

            bool result = false;
            result = true;

            //--------------------------------------------------------------------------
            //Splitting data parts:

            // Split TelegramID part
            m_TelegramID = strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length));
            if (m_TelegramID != GeneralModule.TelegramID_RPCReceived)
            {
                return false;
            }

            strData = strData.Substring(strData.IndexOf(';') + 1);

            m_SVCValue = Convert.ToInt32(Double.Parse(strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length))));
            strData = strData.Substring(strData.IndexOf(';') + 1);

            m_OverFlux = Convert.ToInt32(Double.Parse(strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length))));
            strData = strData.Substring(strData.IndexOf(';') + 1);

            m_PowerFactor1 = Single.Parse(strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length)));
            strData = strData.Substring(strData.IndexOf(';') + 1);

            m_PowerFactor2 = Single.Parse(strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length)));

            return result;
        }
    }
}