using System;

namespace QRPrint
{
	public struct AppConfig
	{
		public static object GetValue(string ID)
		{
			switch (ID)
			{
			case "SerialPortName" :
				return "/dev/tty.usbserial-A600dP3F";
			case "SerialPortBaudRate" :
				return 14400;
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

