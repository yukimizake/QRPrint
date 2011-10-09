using System;

namespace QRPrint
{
	class MainClass
	{
		public static PrinterHandler printerHandler;
		
		
		public static void Main (string[] args)
		{	
			string hostname = "*";
			int portNumber = 5555;
			
			string serverString = String.Format("http://{0}:{1}/",hostname,portNumber.ToString());
			WebServer server = new WebServer(serverString);
			Console.WriteLine("Server " + hostname + " started on port " + portNumber);
			
			printerHandler = new PrinterHandler("/dev/tty.usbserial-A600dP3F",9600);
			
			server.Start();
			Console.ReadLine();
			server.Stop();
		}
	}
}
