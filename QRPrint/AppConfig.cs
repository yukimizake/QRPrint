using System;

namespace QRPrint
{
	public struct AppConfig
	{
		public static string GetValue(string ID)
		{
			switch (ID.ToLower())
			{
			case "SerialPortName" :
				return "/dev/ttyUSB0";
			case "SerialPortBaudRate" :
				return "9600";
			default:
				return "";
			}
		}
		
		public static void SetValue(string ID)
		{
			return;
		}
	}
}

