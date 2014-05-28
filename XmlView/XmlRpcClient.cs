// XmlRpcClient.cs 
using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Collections.Generic;
using XMLRPC;

namespace XMLRPC
{
	public class CXmlRpcClient {
		private ASCIIEncoding		m_Encoding	= new ASCIIEncoding();
		private String				m_strURI;

		public CXmlRpcClient( String strURI ){
			m_strURI	= strURI;
		}

		public CXmlRpcClient( String strHost, int nPort ){
			m_strURI	= String.Format( "http://{0}:{1}/RPC2", strHost, nPort );
		}

		public void		Post( String strBody ){
			HttpWebRequest		m_HttpReq	= (HttpWebRequest)WebRequest.Create( m_strURI );
			m_HttpReq.Method				= "POST";
			m_HttpReq.UserAgent				= "XML-RPC";
			m_HttpReq.ContentType			= "text/xml";
			m_HttpReq.KeepAlive				= true;

			byte[]	buffer	= m_Encoding.GetBytes( strBody );
			m_HttpReq.ContentLength			= buffer.Length;
			
			Stream	sout	= m_HttpReq.GetRequestStream();
			sout.Write( buffer, 0, buffer.Length );
			sout.Close();

			HttpWebResponse	Res		= (HttpWebResponse)m_HttpReq.GetResponse();
			Stream			sRes	= Res.GetResponseStream();
			StreamReader	sReader	= new StreamReader( sRes, Encoding.UTF8 );

			Console.WriteLine( "Content Length :{0}", Res.ContentLength );
			Console.WriteLine( "Content Type:{0}", Res.ContentType );
			Console.WriteLine( sReader.ReadToEnd() );
			sReader.Close();
			Res.Close();
		}
	}
}
public class CMain {
	public static void	Main( String[] args ){
		CXmlRpcValue	vInt		= new CXmlRpcValue( 123 );
		CXmlRpcValue	vDouble		= new CXmlRpcValue( 4.56 );
		CXmlRpcValue	vBoolean	= new CXmlRpcValue( true );
		CXmlRpcValue	vString		= new CXmlRpcValue( "Linq" );

		CXmlRpcValueArray	vArray	= new CXmlRpcValueArray();
		vArray.Add( vInt );
		vArray.Add( vDouble );
		vArray.Add( vBoolean );

		CXmlRpcValueStruct	vStruct	= new CXmlRpcValueStruct();
		vStruct.Add( "chocho", vString );
		vStruct.Add( "cococo", vInt );
		vStruct.Add( "bobobo", vDouble );

		CXmlRpcParams	Params		= new CXmlRpcParams();
		Params.Add( vArray );
		Params.Add( vStruct );
		Console.WriteLine( Params.ToXml() );

		CXmlRpcRequest	GetValue	= new CXmlRpcRequest("GetValue", Params);
		Console.WriteLine( GetValue.ToXml() );

		CXmlRpcClient client = new CXmlRpcClient( "http://localhost:6666/RPC2" );
		client.Post( GetValue.ToXml() );
	}
}

