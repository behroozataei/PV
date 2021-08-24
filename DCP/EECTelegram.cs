using System;

namespace DCP
{
	internal class EECTelegram
	{
		public string m_TelegramID = "";
		public DateTime m_Date = DateTime.UtcNow;
		public string m_Time = "";
		public string m_ResidualTime = ""; //Integer
		public string m_ResidualEnergy = ""; //Integer
		public string m_OverLoad1 = ""; //Integer
		public string m_OverLoad2 = ""; //Integer
		public string m_ResidualEnergyEnd = ""; //Integer

		// This sub compose all fields into a string for sending via a Socket
		public void PrepareToSend(ref string strData)
		{
			//--------------------------------------------------------------------------
			//Mixing all data parts:

			strData = GeneralModule.TelegramID_EECSend + " ; ";
			strData = strData + m_Date + " ; ";
			strData = strData + m_Time + " ; ";
			strData = strData + m_ResidualTime + " ; ";
			strData = strData + m_ResidualEnergy + " ; ";
			strData = strData + m_OverLoad1 + " ; ";
			strData = strData + m_OverLoad2 + " ; ";
			strData = strData + m_ResidualEnergyEnd + " ; ";
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
			if (m_TelegramID != GeneralModule.TelegramID_EECReceived)
			{
				return false;
			}

			strData = strData.Substring(strData.IndexOf(';') + 1);

			// Split Date part
			m_Date = DateTime.Parse(strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length)));
			strData = strData.Substring(strData.IndexOf(';') + 1);

			// Split Time part
			m_Time = strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length));
			strData = strData.Substring(strData.IndexOf(';') + 1);

			// Split Residual Time part
			m_ResidualTime = strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length));
			strData = strData.Substring(strData.IndexOf(';') + 1);

			// Split Residual Energy part
			m_ResidualEnergy = strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length));
			strData = strData.Substring(strData.IndexOf(';') + 1);

			// Split Power Overload EAF Group 1 part
			m_OverLoad1 = strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length));
			strData = strData.Substring(strData.IndexOf(';') + 1);

			// Split Power Overload EAF Group 2 part
			m_OverLoad2 = strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length));
			strData = strData.Substring(strData.IndexOf(';') + 1);

			// Split Residual Power part
			m_ResidualEnergyEnd = strData.Substring(0, Math.Min(strData.IndexOf(';'), strData.Length));

			return result;
		}
	}
}