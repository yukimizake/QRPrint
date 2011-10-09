using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Web;
using ThermalDotNet;

namespace QRPrint
{
	public class QueryHandler
	{	
		private XmlDocument _xmlResponse = new XmlDocument();
		private XmlNode _docNode;
		private HttpListenerContext	_context;
		private NameValueCollection _postParameters;
		
		public QueryHandler(HttpListenerContext listenerContext)
		{
			if (listenerContext != null) {
				_context = listenerContext;
			} else {
				throw new Exception("listenerContext cannot be null");
			}
			
			StreamReader sr = new StreamReader(_context.Request.InputStream);
			_postParameters = HttpUtility.ParseQueryString(sr.ReadToEnd());
			
			_docNode = _xmlResponse.CreateXmlDeclaration("1.0", "UTF-8", null);
			_xmlResponse.AppendChild(_docNode);
		}
		
		public string ProcessInfo()
		{
			switch (_context.Request.Url.AbsolutePath.Trim('/').ToLower())
			{
			case "echo":
				return _getInfoToXml();
			case "printline":
				return Print();
			case "printqrcode":
				return PrintQRCode();
			default:
				return "";
			}
		}
		
		private string Print()
		{
			ThermalPrinter printer = MainClass.printerHandler.printer;
			string result = "0";
			string line;
			
			if (_postParameters.Get("text") != null) {
				line = _postParameters.GetValues("text")[0];
				if (!MainClass.printerHandler.isPrinting && !(line == null || line== "")) {
					MainClass.printerHandler.Reset();
					MainClass.printerHandler.isPrinting = true;
					line = line.Replace("\\n","\n");
					printer.WriteLine(line);
					printer.LineFeed();
					printer.LineFeed();
					printer.LineFeed();
					MainClass.printerHandler.isPrinting = false;
					result = "1";
				}
			}
			
			//Response
			XmlNode rootNode = _xmlResponse.CreateElement("response");
	    	rootNode.AppendChild(_nodify("DateTime",DateTime.UtcNow.ToUniversalTime().ToString()));
	    	rootNode.AppendChild(_nodify("Method", _context.Request.HttpMethod));
			rootNode.AppendChild(_nodify("Result", result));
			_xmlResponse.AppendChild(rootNode);
	    	return _xmlResponse.InnerXml;
		}
		
		private string PrintQRCode()
		{
			ThermalPrinter printer = MainClass.printerHandler.printer;
			string result = "0";
			string url;
			
			if (_postParameters.Get("url") != null) {
				url = _postParameters.GetValues("url")[0];
				if (!MainClass.printerHandler.isPrinting && !(url == null || url == "")) {
					
					MainClass.printerHandler.Reset();
					MainClass.printerHandler.isPrinting = true;
					
					//Prints QR Code
					printer.PrintImage(new QRHandler(url).GetImage());
					
					System.Threading.Thread.Sleep(1000);
					
					//Prints URL
					if (url.StartsWith("http://",true,System.Globalization.CultureInfo.CurrentCulture))
					{
						url = url.Substring(7);
					}
					if (url.StartsWith("www.",true,System.Globalization.CultureInfo.CurrentCulture))
					{
						url = url.Substring(4);
					}
					url = url.TrimEnd('/');
					if (url.Length > 32) {
						url = url.Substring(0,29) + "...";
					}
					printer.SetLineSpacing(0);
					printer.LineFeed();
					printer.SetAlignCenter();
					
					if (url.Length > 16) {
						printer.WriteLine(url);
					} else {
						printer.WriteLine(url,ThermalPrinter.PrintingStyle.DoubleWidth);
					}
					printer.LineFeed();
					printer.LineFeed();
					printer.LineFeed();
					MainClass.printerHandler.isPrinting = false;
					
					result = "1";
				}
			}
			
			//Response
			XmlNode rootNode = _xmlResponse.CreateElement("response");
	    	rootNode.AppendChild(_nodify("DateTime",DateTime.UtcNow.ToUniversalTime().ToString()));
	    	rootNode.AppendChild(_nodify("Method", _context.Request.HttpMethod));
			rootNode.AppendChild(_nodify("Result", result));
			_xmlResponse.AppendChild(rootNode);
	    	return _xmlResponse.InnerXml;
		}
		
		private string _getInfoToXml()
		{
	    	XmlNode rootNode = _xmlResponse.CreateElement("response");
			
	    	rootNode.AppendChild(_nodify("DateTime",DateTime.UtcNow.ToUniversalTime().ToString()));
	    	rootNode.AppendChild(_nodify("Method",_context.Request.HttpMethod));
			rootNode.AppendChild(_nodify("Url.AbsoluteUri",_context.Request.Url.AbsoluteUri));
			rootNode.AppendChild(_nodify("Url.AbsolutePath",_context.Request.Url.AbsolutePath));
			
			//Reading GET query string
			if (_context.Request.Url.Query.Length > 0)
				rootNode.AppendChild(_nodify("GET_Query",_context.Request.Url.Query));
			
			if (_context.Request.QueryString.Count > 0)
			{
				XmlNode getParametersNode = _xmlResponse.CreateElement("get_parameters");
				for (int i = 0; i < _context.Request.QueryString.Count; i++)
				{
					getParametersNode.AppendChild(
						_nodify(
						_context.Request.QueryString.GetKey(i),
						_context.Request.QueryString.GetValues(i)[0])
						);
				}
				rootNode.AppendChild(getParametersNode);
			}
			
			//Reading POST data
			if (_postParameters.Count > 0)
			{
				XmlNode postParametersNode = _xmlResponse.CreateElement("post_parameters");
				for (int i = 0; i < _postParameters.Count; i++)
				{
					Console.WriteLine ("adding key value pair : {0}: {1} ",_postParameters.AllKeys[i],_postParameters.GetValues(i)[0] );
					postParametersNode.AppendChild(_nodify(_postParameters.AllKeys[i],_postParameters.GetValues(i)[0]));
				}
				rootNode.AppendChild(postParametersNode);
			}
			
	    	_xmlResponse.AppendChild(rootNode);
	    	return _xmlResponse.InnerXml;
		}
		
		private XmlNode _addDictionary(XmlNode node, Dictionary<string,string> dictValues)
		{
			XmlNode newNode = node.Clone();
			foreach (KeyValuePair<string, string> item in dictValues)
			{
				newNode.AppendChild(_nodify(item.Key,item.Value));
			}
			return newNode;
		}
		
		private XmlNode _nodify(string nodeName, string nodeValue)
        {
			if (nodeName == null || nodeName.Trim() == "")
			{
				nodeName = "bad_name_given";
			}
			
			char[] nodeNameCharArray = nodeName.ToCharArray();
			int i = 0;
			foreach (char currentChar in nodeNameCharArray)
			{ 
				if (!Char.IsLetterOrDigit(currentChar)) {
					nodeNameCharArray[i] = '_';
				}
				i++;
			}
			nodeName = new String(nodeNameCharArray);
            XmlNode tempNode = _xmlResponse.CreateElement(nodeName);
            tempNode.AppendChild(_xmlResponse.CreateTextNode(nodeValue));
            return tempNode;
        }
		
		public string InspectContext()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(">>> " + DateTime.Now.ToString() + " Request inspection <<<\n");
			sb.AppendLine("UrlReferrer        " + _context.Request.UrlReferrer);
			sb.AppendLine("UserAgent          " + _context.Request.UserAgent);
			sb.AppendLine("UserHostName       " + _context.Request.UserHostName);
			sb.AppendLine("UserHostAddress    " + _context.Request.UserHostAddress);
			sb.AppendLine("IsSecureConnection " + _context.Request.IsSecureConnection);
			
			sb.AppendLine();
			foreach (string item in _context.Request.AcceptTypes)
			{
				sb.AppendLine("Accept type: " + item);
			}
			
			sb.AppendLine();
			sb.AppendLine("Content type            " + _context.Request.ContentType);
			sb.AppendLine("Content encoding        " + _context.Request.ContentEncoding);
			sb.AppendLine("Content ContentLength64 " + _context.Request.ContentLength64);
			
			sb.AppendLine();
			sb.AppendLine("Method             " + _context.Request.HttpMethod);
	    	sb.AppendLine("Url.AbsoluteUri    " + _context.Request.Url.AbsoluteUri);
	    	sb.AppendLine("Url.OriginalString " + _context.Request.Url.OriginalString);
			sb.AppendLine("RawUrl             " + _context.Request.RawUrl);
			sb.AppendLine("Url.PathAndQuery   " + _context.Request.Url.PathAndQuery);
	    	sb.AppendLine("Url.AbsolutePath   " + _context.Request.Url.AbsolutePath);
	    	sb.AppendLine("Url.Query          " + _context.Request.Url.Query);

			//Reading GET query string
			if (_context.Request.QueryString.Count > 0)
			{
				sb.AppendLine();
				sb.AppendLine("Query string");	
				for (int i = 0; i < _context.Request.QueryString.Count; i++)
				{
					sb.AppendLine(	"GET Key   : " + _context.Request.QueryString.GetKey(i) + "\n" +
									"GET Value : " + _context.Request.QueryString.GetValues(i)[0]);
				}
			}
			
			//Reading POST data	
			if (_postParameters.Count > 0) {
				sb.AppendLine();
				sb.AppendLine("POST data");
				for (int i = 0; i < _postParameters.Count; i++)
				{
					sb.AppendLine("POST Key  : " 	+ _postParameters.AllKeys[i]);
					sb.AppendLine("POST Value: " 	+ _postParameters.GetValues(i)[0]);
				}
			}
			
			sb.AppendLine("\nFin de requête.");
	    	sb.AppendLine();
			return sb.ToString();
		}

	}
}

