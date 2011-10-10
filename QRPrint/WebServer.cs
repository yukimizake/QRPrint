using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace QRPrint
{
	public class WebServer
	{
	    HttpListener _listener;   
	
	    public WebServer(string uriPrefix)
		{
	        System.Threading.ThreadPool.SetMaxThreads(50, 1000);
	        System.Threading.ThreadPool.SetMinThreads(50, 50);
			
	        _listener = new HttpListener();
	        _listener.Prefixes.Add(uriPrefix);
	    }
	
	    public void Start()
		{                       
	        _listener.Start();
	        while (true)
	            try {
	                HttpListenerContext request = _listener.GetContext();
	                ThreadPool.QueueUserWorkItem(ProcessRequest, request);
	            	}
				catch (HttpListenerException) { break; }  
	            catch (InvalidOperationException) { break; }
	    }
	
	    public void Stop()
		{
			_listener.Stop();
		}
	
	    void ProcessRequest(object listenerContext)
		{
	        try {
	            HttpListenerContext context = (HttpListenerContext)listenerContext;
				
				//response
				QueryHandler qh = new QueryHandler(context);
				Console.WriteLine(qh.InspectContext());
				
				byte[] response = Encoding.UTF8.GetBytes(qh.ProcessInfo());
				
				context.Response.StatusCode = qh.StatusCode;
				context.Response.ContentLength64 = response.Length;
				context.Response.ContentType = "text/xml; charset=UTF-8";
				
            	Stream s = context.Response.OutputStream;
                s.Write(response, 0, response.Length);
				s.Close();

	        } catch (Exception ex) { Console.WriteLine("Request error: " + ex); }
	    }		
	}
}

