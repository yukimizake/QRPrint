using System;
using System.IO.Ports;
using ThermalDotNet;

namespace QRPrint
{
	public class PrinterHandler
	{
		public ThermalPrinter printer;
		public bool isPrinting = false;
		
		private SerialPort _serialPort;
		
		private string _serialPortName;
		private int _serialBaudRate;
		
		public PrinterHandler(string serialPortName, int serialPortBaudRate)
		{
			_serialPortName = serialPortName;
			_serialBaudRate = serialPortBaudRate;
			
			_constructor();
		}
		
		private void _constructor()
		{
			_serialPort = new SerialPort(_serialPortName,_serialBaudRate);
			_serialPort.Open();
			printer = new ThermalPrinter(_serialPort,2,180,1);
			printer.PictureLineSleepTimeMs = 1;
			//TODO reading printer config
		}
		
		~PrinterHandler ()
		{
			_serialPort.Close();
		}
		
		public void Reset()
		{
			printer.Reset();
			System.Threading.Thread.Sleep(100);
		}
	}
}