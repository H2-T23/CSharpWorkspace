// XmlRpcValue.cs 
using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Collections.Generic;

namespace XMLRPC {
	class CDateTime {
		public static CDateTime 	Current(){
			return new CDateTime();
		}

		public CDateTime(){
		}

		public override String	ToString(){
			return "";
		}
	}

	class CBase64 {
		public CBase64(){
		}

		public override String	ToString(){
			return "";
		}
	}

	interface IXmlValue {
		String		ToXml();
	}

	class CXmlRpcValue : IXmlValue {
		String			m_strValue;
		String			m_strType;

		public CXmlRpcValue(){}
		public CXmlRpcValue( int v )		{ Set(v); }
		public CXmlRpcValue( bool v )		{ Set(v); }
		public CXmlRpcValue( double v )		{ Set(v); }
		public CXmlRpcValue( String v )		{ Set(v); }
		public CXmlRpcValue( CDateTime v )	{ Set(v); }
		public CXmlRpcValue( CBase64 v )	{ Set(v); }

		public void		Set( int v )		{ m_strValue	= v.ToString();	m_strType = "int";		}
		public void		Set( bool v )		{ m_strValue	= v.ToString();	m_strType = "boolean";	}
		public void		Set( double v )		{ m_strValue	= v.ToString();	m_strType = "double";	}
		public void		Set( String v )		{ m_strValue	= v;			m_strType = "string";	}
		public void		Set( CDateTime v )	{ m_strValue	= v.ToString();	m_strType = "dateTime.iso8601";	}
		public void		Set( CBase64 v )	{ m_strValue	= v.ToString();	m_strType = "base64";	}

		public String	ToXml() {
			return String.Format("<value><{0}>{1}</{2}></value>", m_strType, m_strValue, m_strType);
		}
	}

	class CXmlRpcValueArray : IXmlValue {
		private List<IXmlValue>			m_Nodes		= new List<IXmlValue>();

		public CXmlRpcValueArray(){}
		public CXmlRpcValueArray( IXmlValue v ){
			m_Nodes.Add( v );
		}

		public void		Add( IXmlValue v ){
			m_Nodes.Add( v );
		}

		public String	ToXml(){
			String	strXml	= "<array><data>\n";

			foreach( IXmlValue v in m_Nodes ){
				strXml	+= "\t";
				strXml	+= v.ToXml();
				strXml	+= "\n";
			}

			strXml	+= "</data></array>\n";
			return strXml;
		}
	}

	class CXmlRpcValueStruct : IXmlValue {

		protected class CXmlRpcValueStructMember : IXmlValue {
			private readonly String			m_Name;
			private readonly IXmlValue		m_Value;

			public CXmlRpcValueStructMember(String str, IXmlValue value){
				this.m_Name		= str;
				this.m_Value	= value;
			}

			public String	ToXml(){
				return String.Format( "<member><name>{0}</name>{1}</member>", m_Name, m_Value.ToXml() );
			}
		}

		private List<CXmlRpcValueStructMember>	m_Nodes		= new List<CXmlRpcValueStructMember>();

		public CXmlRpcValueStruct(){}
		public CXmlRpcValueStruct( String name, IXmlValue v ){
			m_Nodes.Add( new CXmlRpcValueStructMember(name, v) );
		}

		public void		Add( String name, IXmlValue v ){
			m_Nodes.Add( new CXmlRpcValueStructMember(name, v) );
		}

		public String	ToXml(){
			String	strXml	= "<struct>\n";

			foreach( CXmlRpcValueStructMember v in m_Nodes ){
				strXml	+= "\t";
				strXml	+= v.ToXml();
				strXml	+= "\n";
			}

			strXml	+= "</struct>\n";
			return strXml;
		}
	}

	class CXmlRpcParams : IXmlValue {
		private List<IXmlValue>		m_Nodes	= new List<IXmlValue>();

		public CXmlRpcParams(){
		}
		public CXmlRpcParams( IXmlValue value ){
			m_Nodes.Add( value );
		}

		public void		Add( IXmlValue value ){
			m_Nodes.Add( value );
		}

		public String	ToXml() {
			String	strXml	= "<params><param>\n";

			foreach( IXmlValue v in m_Nodes ){
				strXml	+= v.ToXml();
			}

			strXml	+= "</param></params>\n";
			return strXml;
		}
	}

	class CXmlRpcResponse : IXmlValue {
		public String	ToXml(){
			return "";
		}
	}

	class CXmlRpcRequest : IXmlValue {
		private String				m_strMethod;
		private CXmlRpcParams		m_XmlRpcValueParams;

		public CXmlRpcRequest( String strMethod, CXmlRpcParams Params ){
			m_strMethod				= strMethod;
			m_XmlRpcValueParams		= Params;
		}

		public String	ToXml(){
			String	strXml	= "<?xml version=\"1.0\"?>\n";
			strXml += "<methodCall>\n";
			strXml += String.Format( "<methodName>{0}</methodName>\n", m_strMethod );
			strXml += m_XmlRpcValueParams.ToXml();
			strXml += "</methodCall>";
			return strXml;
		}
	}

	class CTest {
		public CTest(){
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
		}
	}
}

//public class CMain {
//	public static void	Main( String[] args ){
//		new CTest();
//	}
//}
