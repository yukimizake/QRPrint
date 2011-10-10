using System;

namespace QRPrint
{
	class MainClass
	{
		public static PrinterHandler printerHandler;
		public static bool serverRunning = true;
		
		public static void Main (string[] args)
		{	
			string hostname = "*";
			int portNumber = 5555;
			
			string serverString = String.Format("http://{0}:{1}/",hostname,portNumber.ToString());
			WebServer server = new WebServer(serverString);
			Console.WriteLine("Server " + hostname + " started on port " + portNumber);
			
			printerHandler = new PrinterHandler(
				(string)AppConfig.GetValue("SerialPortName"),
				(int)AppConfig.GetValue("SerialPortBaudRate")
				);
			
			server.Start();
			
			while (serverRunning) {
				System.Threading.Thread.Sleep(1000);
			}
			
			server.Stop();
		}
	}
}
