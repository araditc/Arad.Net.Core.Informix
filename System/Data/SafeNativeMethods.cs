using System;
using System.Runtime.InteropServices;

namespace Arad.Net.Core.Informix.System.Data;

internal class SafeNativeMethods
{
	internal static IntPtr LocalAlloc(IntPtr initialSize)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(initialSize);
		ZeroMemory(intPtr, (int)initialSize);
		return intPtr;
	}

	internal static void LocalFree(IntPtr ptr)
	{
		Marshal.FreeHGlobal(ptr);
	}

	internal static void ZeroMemory(IntPtr ptr, int length)
	{
		byte[] source = new byte[length];
		Marshal.Copy(source, 0, ptr, length);
	}
}
