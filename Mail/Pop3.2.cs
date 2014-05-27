// Pop3.cs 
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net.Security;

using System.Text;

namespace Mail {

	class CPOP {
		public const String		CHAR_SET	= "ISO-2022-JP";
		public String			SERVER;
		public const int		PORT		= 995;
		public bool				IsUseSSL	= true;

		public CPOP(){
		}

		public CPOP( String strServer ){
			Set(strServer);
		}

		protected void	Set( String strServer ){
			SERVER		= strServer;
		}
	}

	class CPOP3 : CPOP {
		public CPOP3(){
		}

		public CPOP3( String strServer ){
			Set( strServer );
		}

		public String		Recv( String strUser, String strPass ){

			String			str	= "";
			SslStream		ssl	= null;
			TcpClient		tcp	= null;
			
			try{
				tcp		= new TcpClient();
				tcp.Connect( SERVER, PORT );

				if( IsUseSSL )
				{
					ssl		= new SslStream( tcp.GetStream() );
					ssl.AuthenticateAsClient( SERVER );

					str		= SendRecv( ssl, "" );
					if( str.StartsWith( "+" ) == false ){
						throw new Exception( "Error: " + str );
					}

					str		= SendRecv( ssl, "USER " + strUser + "\r\n" );
					if( str.StartsWith( "+" ) == false ){
					}

					str		= SendRecv( ssl, "PASS " + strPass + "\r\n" );
					if( str.StartsWith( "+" ) == false ){
					}

					str		= SendRecv( ssl, "RETR 1 \r\n" );
					if( str.StartsWith( "+" ) == false ){
					}

					SendRecv( ssl, "QUIT\r\n" );
				}
			}
			catch( Exception e ){	
				throw e;
			}

			return str;
		}

		protected String	SendRecv( SslStream ssl, String strReq ){
			String	str		= null;
			Byte[]	buffer	= null;
			
			if( !String.IsNullOrEmpty(strReq) ){
				buffer	= System.Text.Encoding.GetEncoding( CHAR_SET ).GetBytes( strReq );

				ssl.Write( buffer, 0, buffer.Length );
				ssl.Flush();

				buffer	= null;
			}

			buffer	= new Byte[]{};

			Array.Resize<Byte>( ref buffer, (1024 * 1024) ); 
			int	i	= ssl.Read( buffer, 0, buffer.Length );
			if( i > 0 )
			{
				Array.Resize<Byte>( ref buffer, i );
				str		= System.Text.Encoding.GetEncoding( CHAR_SET ).GetString( buffer );
			}
			else
			{
				throw new Exception("Error: Recv");
			}

			if( str.StartsWith( "+" ) && (strReq.ToUpper().StartsWith( "RETR")) )
			{
				do{
					Array.Resize<Byte>( ref buffer, (1024 * 1024) );
					i	= ssl.Read( buffer, 0, buffer.Length );
					if( i > 0 )
					{
						Array.Resize<Byte>( ref buffer, i );
						str	+= Encoding.GetEncoding( CHAR_SET ).GetString( buffer );
					}
					else
					{
						throw new Exception("Error: Recv");
					}
				} while( !str.EndsWith( "+\r\n" ) );
			}
			
	
			return str;
		}
	}

	class CInputPanel : Panel {
		private TextBox		Txt1, Txt2, Txt3;

		public CInputPanel(){
			InitControl();

			this.Resize += new EventHandler( OnPanelResize );
		}

		protected bool	InitControl(){
			bool	bRet	= false;
			try{
				Txt1	= new TextBox(){
					Multiline	= false,
					Dock		= DockStyle.Top,	
				};

				Txt2	= new TextBox(){
					Multiline	= false,
					Dock		= DockStyle.Top,
				};

				Txt3	= new TextBox(){
					Multiline	= true,
					Dock		= DockStyle.None,
				};

				this.Controls.Add( Txt1 );
				this.Controls.Add( Txt2 );
				this.Controls.Add( Txt3 );

				bRet	= true;
			}
			catch( Exception e ){
				throw e;
			}
			return bRet;
		}

		private void	OnPanelResize( Object sender, EventArgs args ){
			Txt3.Top	= this.Top + (Txt1.Height + Txt2.Height);
			Txt3.Left	= 0;
			Txt3.Size	= new Size(this.Width, this.Height - (Txt1.Height + Txt2.Height));
		} 
	}

	class CMainForm	: Form {
		private TextBox				Txt1, Txt2;
		private SplitContainer		Splitter;
		private ListView			listview;
		private	MainMenu			menuMain;
		private	CInputPanel			InputPanel;

		public CMainForm(){
			InitMenu();
			InitControl();
		}

		protected bool	InitMenu(){
			bool	bRet	= false;

			try{
				MenuItem	menuNew		= new MenuItem( "New" );
				menuNew.Click += new EventHandler( OnFileNew );
				menuNew.Shortcut	= Shortcut.CtrlN;

				MenuItem	menuExit	= new MenuItem( "Exit" );
				menuExit.Click += new EventHandler( OnFileExit );
				menuExit.Shortcut	= Shortcut.AltF4;

				MenuItem	menuFile	= new MenuItem( "File" );
				menuFile.MenuItems.AddRange( new MenuItem[]{
					menuNew, new MenuItem("-"), menuExit 
				});

				menuMain	= new MainMenu();
				menuMain.MenuItems.AddRange( new MenuItem[]{
					menuFile
				});

				this.Menu	= menuMain;

				bRet	= true;;
			}
			catch( Exception e ){
				throw e;
			}

			return bRet;
		}

		protected bool	InitControl(){
			bool	bRet	= false;
			try{
				Txt1		= new TextBox(){
					Multiline		= true,
					Dock			= DockStyle.Top,
				};

				Txt2		= new TextBox(){
					Multiline		= true,
					Dock			= DockStyle.Fill,
				};

				listview	= new ListView(){
					GridLines		= true,
					View			= View.Details,
					Dock			= DockStyle.Fill,
				};

				InputPanel	= new CInputPanel(){
					Dock			= DockStyle.Fill,
				};

				Splitter = new SplitContainer(){
					Dock			= DockStyle.Fill,
					Orientation		= Orientation.Horizontal,
				};

				Splitter.Panel1.Controls.Add( InputPanel );
				Splitter.Panel2.Controls.Add( listview );

				this.Controls.Add( Splitter );
			
				bRet	= true;
			}
			catch( Exception e ){
				throw e;
			}
			return bRet;
		}


		private void	OnFileNew( Object sender, EventArgs args ){
		/*
			OpenFileDialog	dlg = new OpenFileDialog(){
				Multiselect		= false,
				Filter			= "All Files|*.*"
			};

			DialogResult ret	= dlg.ShowDialog();
			if( ret == DialogResult.OK ){
				this.Text	= dlg.SafeFileName;
			}
		*/
			MessageBox.Show("OnFileNew", "OK");
		}

		private void	OnFileExit( Object sender, EventArgs args ){
			this.Close();
		}
	}

	class CMain {
		public static void	Main( String[] args ){
			CMainForm	Form = new CMainForm();
			Form.Text		= "MainForm";
			Form.ClientSize	= new Size(600, 600);
			Application.EnableVisualStyles();
			Application.Run(Form);
		}
	}
}
