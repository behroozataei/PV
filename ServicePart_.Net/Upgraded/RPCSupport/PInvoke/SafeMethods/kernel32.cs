using System.Runtime.InteropServices;

namespace RPCSupport.PInvoke.SafeNative
{
	public static class kernel32
	{

		public static int GetComputerName(ref string lpBuffer, ref int nSize)
		{
			return RPCSupport.PInvoke.UnsafeNative.kernel32.GetComputerName(ref lpBuffer, ref nSize);
		}
		public static void Sleep(int dwMiliseconds)
		{
			RPCSupport.PInvoke.UnsafeNative.kernel32.Sleep(dwMiliseconds);
		}
	}
}