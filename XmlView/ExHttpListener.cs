// ExHttpListener.cs 
using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;

class CHttpServer {

	private HttpListener	m_Http;

	public CHttpServer( String prefixes ){
		if( !HttpListener.IsSupported ){
			return;
		}

		if( prefixes == null || prefixes.Length == 0 ){
			throw new ArgumentException( "prefixes" );
		}
		
		m_Http	= new HttpListener();
		m_Http.Prefixes.Add( prefixes );
		m_Http.Start();

		try{
			while( true )
			{
				Console.WriteLine();
				Console.WriteLine( "Listening..." );

				HttpListenerContext		context		= m_Http.GetContext();
				HttpListenerRequest		request		= context.Request;
				HttpListenerResponse	response	= context.Response;

				ShowRequestProperties( request );
				ShowRequestData( request );

				response.StatusCode			= (int)HttpStatusCode.OK;
				response.ContentType		= MediaTypeNames.Text.Html;
				response.ContentEncoding	= Encoding.UTF8;
				
				if( request.HttpMethod == "GET" ){
					StreamWriter	sw	= new StreamWriter( response.OutputStream );
					sw.WriteLine( "<html><body>hello world</body></html>" );
					sw.Flush();
				}

				response.Close();
			}
		}
		catch( Exception e ){
			Console.WriteLine( "**** Exception ****: {0}", e.ToString() );
		}
		finally{
			m_Http.Stop();
		}
	}

	public void		ShowRequestProperties( HttpListenerRequest request )
	{
	    // Display the MIME types that can be used in the response.
	    string[] types = request.AcceptTypes;
	    if (types != null)
	    {
	        Console.WriteLine("Acceptable MIME types:");
	        foreach( string s in types ){
	            Console.WriteLine( s );
	        }
	    }

	    // Display the language preferences for the response.
	    types	= request.UserLanguages;
	    if( types != null )
	    {
	        Console.WriteLine( "Acceptable natural languages:" );
	        foreach( string l in types ){
	            Console.WriteLine( l );
	        }
	    }

	    // Display the URL used by the client.
	    Console.WriteLine( "URL         : {0}", request.Url.OriginalString	);
	    Console.WriteLine( "Raw URL     : {0}", request.RawUrl				);
	    Console.WriteLine( "Query       : {0}", request.QueryString			);

	    // Display the referring URI.
	    Console.WriteLine( "Referred by : {0}", request.UrlReferrer			);

	    // Display the HTTP method.
	    Console.WriteLine( "HTTP Method : {0}", request.HttpMethod			);

	    // Display the host information specified by the client;
	    Console.WriteLine( "Host name   : {0}", request.UserHostName		);
	    Console.WriteLine( "Host address: {0}", request.UserHostAddress		);
	    Console.WriteLine( "User agent  : {0}", request.UserAgent			);
	}

	public void		ShowRequestData( HttpListenerRequest request )
	{
	    if( !request.HasEntityBody ){
	        Console.WriteLine("No client data was sent with the request.");
	        return;
	    }

	    System.IO.Stream		body		= request.InputStream;
	    System.Text.Encoding	encoding	= request.ContentEncoding;
	    System.IO.StreamReader	reader		= new System.IO.StreamReader( body, encoding );

	    if( request.ContentType != null ){
	        Console.WriteLine("Client data content type {0}", request.ContentType);
	    }
	    Console.WriteLine("Client data content length {0}", request.ContentLength64);

	    Console.WriteLine("----- Start of client data: -----");
	    // Convert the data to a string and display it on the console.
	    string s = reader.ReadToEnd();
	    Console.WriteLine(s);
	    Console.WriteLine("----- End of client data: -----");

		reader.Close();
		body.Close();
	    // If you are finished with the request, it should be closed also.
	}
};

class CMain {
	public static void	Main( String[] args ){
		foreach( String str in args ){
			Console.WriteLine( str );
		}
		new CHttpServer( args[0] );
	}
}
