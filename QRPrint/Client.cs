using System.Net;
using System.IO;
using System.Text;

namespace QRPrint
{
	public class Client
	{
		static string RestGet(string url)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
			StreamReader reader =  new StreamReader(resp.GetResponseStream(),Encoding.GetEncoding(28605));
			string result = null;
			result = reader.ReadToEnd();
			return result;
		}
	}
}