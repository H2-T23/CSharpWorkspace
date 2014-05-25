// pop3.cs 
using System;
//	using System.Net;


namespace SMTP {

	class CSmtp {
		public int			ENC		= 50220;
		public int			PORT	= 0;
		public String		SERVER	= null;

		public CSmtp(){
			Console.WriteLine("new SMTP");
		}

		public CSmtp( String strServer, int nPort ){
			SERVER	= strServer;
			PORT	= nPort;
		}

		/******************************************************************************************
		 * <summary>
		 * 
		 * <param name="strUser"></param>
		 * <param name="strPass"></param>
		 */
		public void		Send( String strUser, String strPass
							, String strFwdAddr, String strToAddr
							, String strSubject, String strBody
							, String strFils)
		{
			System.Net.Mail.MailMessage	msg	= null;

			try{
				msg	= new System.Net.Mail.MailMessage();

				System.Text.Encoding enc = System.Text.Encoding.GetEncoding( ENC );
				
				msg.Subject				= strSubject;
				msg.SubjectEncoding		= enc;
				
				msg.Body				= strBody;
				msg.BodyEncoding		= enc;
				
				msg.From				= new System.Net.Mail.MailAddress( strFwdAddr );
				
				String[] Tos	= strToAddr.Split( '.' );
				for( int i=0; i<Tos.Length; i++ ){
					if( Tos[ i ] != "" ){
						msg.To.Add( new System.Net.Mail.MailAddress( Tos[ i ] ) );
					}
				}

				System.Net.Mail.Attachment	atth;

				String[] Files	= strFils.Split( '\t' );
				for( int i=0; i<Files.Length; i++ )
				{
					if( Files[ i ] != "" )
					{
						if( System.IO.File.Exists( Files[ i ] ) ){
							atth	= new System.Net.Mail.Attachment( Files[ i ] );
							atth.NameEncoding	= enc;
							msg.Attachments.Add( atth );
						}
					}
				}

				
				System.Net.Mail.SmtpClient	client = new System.Net.Mail.SmtpClient();
				client.Host		= SERVER;
				client.Port		= PORT;

				if( strUser != "" ){
					client.Credentials	= new System.Net.NetworkCredential( strUser, strPass );
				}

				client.Send( msg );
			}
			catch( Exception e ){
				throw e;
			}
			finally{
				if( msg != null ){
					msg.Dispose();
				}
			}
		}
		/******************************************************************************************
		 * <summary>
		 * POP Before SMTP認証のため、POPサーバに接続
		 * <param name="strUser"></param>
		 * <param name="strPass"></param>
		 */
		public void		PopBeforeSmtp( String strUser, String strPass )
		{
			System.Net.Sockets.NetworkStream	stream	= null;
			System.Net.Sockets.TcpClient		tcp		= null;

			try{
				String	strResult;

				tcp		= new System.Net.Sockets.TcpClient();
				tcp.Connect( SERVER, PORT );
				stream	= tcp.GetStream();

				strResult	= WriteAndRead( stream, "" );
				if( strResult.IndexOf( "+OK" ) != 0 ){
					throw new Exception( "Failed: POP Server Connection." );
				}

				strResult	= WriteAndRead( stream, "USER " + strUser + "\r\n" );
				if( strResult.IndexOf( "+OK" ) != 0 ){
					throw new Exception( "Error: User ID." );
				}

				strResult	= WriteAndRead( stream, "PASS " + strPass + "\r\n" );
				if( strResult.IndexOf( "+OK" ) != 0 ){
					throw new Exception( "Error: Pass" );
				}

				strResult	= WriteAndRead( stream, "STAT" + "\r\n" );
				if( strResult.IndexOf( "+OK" ) != 0 ){
				}

				strResult	= WriteAndRead( stream, "QUIT" + "\r\n" );

			}
			catch( Exception e ){
				throw e;
			}
			finally{
				if( stream != null ){
					stream.Close();
					stream.Dispose();
				}
				if( tcp != null ){
					tcp.Close();
				}
			}
		}
		/******************************************************************************************
		 * <summary>
		 * POP Before SMTP認証のため、POPサーバに接続
		 * <param name="strUser"></param>
		 * <param name="strPass"></param>
		 */
		private String	WriteAndRead( System.Net.Sockets.NetworkStream stream, String strRequest )
		{
			Byte[]	byData;

			if( strRequest != "" ){
				byData	= System.Text.Encoding.ASCII.GetBytes( strRequest );
				stream.Write( byData, 0, byData.Length );
			}

			for( int i=0; i<300; i++ ){
				if( stream.DataAvailable ){
					break;
				}
				System.Threading.Thread.Sleep( 10 );
			}

			String	strResponse	= "";
			byData	= new Byte[ 1024 ];
			while( stream.DataAvailable ){
				int	i	= stream.Read( byData, 0, byData.Length );
				if( i > 0 ){
					Array.Resize<Byte>( ref byData, i );
					strResponse	= strResponse + System.Text.Encoding.ASCII.GetString( byData );
				}
			}

			return strResponse;
		}
	}// End of "class CSmtp".
}

namespace POP3 {

	class Program {
		public static void Main(String[] args){
			new SMTP.CSmtp();
			System.Console.WriteLine("POP3");
		}
	}
}
