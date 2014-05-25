// Lunch.cs 
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;

namespace UTILITY 
{
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
	struct SHFILEINFO {
		public IntPtr		hIcon;
		public IntPtr		iIcon;
		[MarshalAs(UnmanagedType.ByValTStr,SizeConst=260)]
		public string		szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr,SizeConst=80)]
		public string		szTypeName;
	}

	public class CIcon {
		const uint	SHGFI_LARGEICON				= 0x00000000;
		const uint	SHGFI_SMALLICON				= 0x00000001;
		const uint	SHGFI_USEFILEATTRIBUTES		= 0x00000010;
		const uint	SHGFI_ICON					= 0x00000100;

		[DllImport("shell32.dll", CharSet=CharSet.Auto)]
		static extern IntPtr	SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
   
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool		DestroyIcon(IntPtr hIcon);

		public static Image		FileAssociatedImage(String strPath){
			return FileAssociatedImage(strPath, false, File.Exists(strPath));
		}	
		public static Image		FileAssociatedImage(String strPath, bool isLarge){
			return FileAssociatedImage(strPath, isLarge, File.Exists(strPath));
		}
		public static Image		FileAssociatedImage(String strPath, bool isLarge, bool isExists){
			SHFILEINFO	fInfo	= new SHFILEINFO();
			uint flags	= SHGFI_ICON;
			if( !isLarge )
				flags	|= SHGFI_SMALLICON;
			if( !isExists )
				flags	|= SHGFI_USEFILEATTRIBUTES;

			try{
				SHGetFileInfo( strPath, 0, ref fInfo, (uint)Marshal.SizeOf(fInfo), flags );
				if( fInfo.hIcon == IntPtr.Zero )
					return null;
				else
					return Icon.FromHandle( fInfo.hIcon ).ToBitmap();
			}
			finally{
				if( fInfo.hIcon != IntPtr.Zero ){
					DestroyIcon( fInfo.hIcon );
				}
			}
		}
	}
}

namespace LUNCH 
{
	using System.Windows.Forms.VisualStyles;
	using UTILITY;

	class CShutcutButton : Button {
		private PushButtonState		state	= PushButtonState.Normal;
		private	Image				icon;
		public	String				Path;

		public CShutcutButton() : base() {
			FlatStyle	= FlatStyle.Popup;
		}

		public CShutcutButton( String str ) : base() {
			icon		= CIcon.FileAssociatedImage( str, true );

			Path		= str;
		//	Text		= str;
			Image		= icon;
			Width		= icon.Width + 5;
			Height		= icon.Height + 5;
			FlatStyle	= FlatStyle.Popup;
		}

		protected override void		OnPaint( PaintEventArgs args ){
		/*
			Rectangle rc	= this.ClientRectangle;

			switch( state ){
			case PushButtonState.Normal:
				args.Graphics.FillRectangle( Brushes.Gray, rc );
				break;
	
			case PushButtonState.Hot:
				ControlPaint.DrawButton( args.Graphics, rc, ButtonState.Normal );
				break;

			case PushButtonState.Pressed:
				ControlPaint.DrawButton( args.Graphics, rc, ButtonState.Pushed );
				break;
			}

			StringFormat	strFmt = new StringFormat();
			strFmt.Alignment		= StringAlignment.Center;
			strFmt.LineAlignment	= StringAlignment.Center;
			args.Graphics.DrawString( this.Text, this.Font, Brushes.Black, rc, strFmt );
		*/
			base.OnPaint( args );
		}

		protected override void		OnMouseClick( MouseEventArgs args ){
			base.OnMouseClick( args );
			MessageBox.Show(Path, "OnMouseClick");
			Process ps = Process.Start( Path );
		}

		protected override void		OnMouseDown( MouseEventArgs args ){
			base.OnMouseDown( args );
			state	= PushButtonState.Pressed;
		}

		protected override void		OnMouseEnter( EventArgs args ){
			base.OnMouseEnter( args );
			state	= PushButtonState.Hot;
		}

		protected override void		OnMouseLeave( EventArgs args ){
			base.OnMouseLeave( args );
			state	= PushButtonState.Normal;
		}

		protected override void		OnMouseUp( MouseEventArgs args ){
			base.OnMouseUp( args );
			OnMouseEnter( args );		
		}
	}

	class CMainForm : Form {
		protected ToolTip			tooltip	= new ToolTip(){InitialDelay=1000,ReshowDelay=1000,AutoPopDelay=10000,ShowAlways=true,};
		protected FlowLayoutPanel	FlowLayout;
		protected Bitmap			FormImage;

		public CMainForm(){
			FormBorderStyle			= FormBorderStyle.FixedToolWindow;
			MinimizeBox				= false;
			MaximizeBox				= false;
			AllowDrop				= true;

		//	TransparencyKey			= Color.Red;
		//	BackColor				= Color.Red;

		/*
			FormImage	= new Bitmap(@"Happy.png");
			FormImage.MakeTransparent( Color.Blue );
			BackgroundImageLayout	= ImageLayout.Stretch;
			Size					= FormImage.Size;
			BackgroundImage			= FormImage;
		*/
			BackgroundImageLayout	= ImageLayout.Zoom;
			BackgroundImage			= Image.FromFile(@"C:\Users\jupiter\CSharpWorkspace\Lunch\Happy.png");

			Activated		+= new EventHandler(FormActivated);
			Load			+= new EventHandler(FormLoad);
			Closed			+= new EventHandler(FormUnload);
			DragEnter		+= new DragEventHandler(FormDragEnter);
			DragDrop		+= new DragEventHandler(FormDragDrop);
		}

		private void	FormActivated( object sender, EventArgs args ){
		//	MessageBox.Show("Activated");
		}

		private void	FormLoad( object sender, EventArgs args ){
		//	MessageBox.Show("Load","");
			InitControls();	
		}

		private void	FormUnload( object sender, EventArgs args ){
		//	MessageBox.Show("Unload","");
		}

		// オブジェクトがコントロールの境界内にドラッグされると発生する 
		private void	FormDragEnter( object sender, DragEventArgs args ){
			if( args.Data.GetDataPresent( DataFormats.FileDrop ) ){
				String[] strAry	= (String[])args.Data.GetData( DataFormats.FileDrop );
				foreach( String str in strAry ){
					if( !System.IO.File.Exists( str ) ){
						return;
					}
				}
				args.Effect	= DragDropEffects.Copy;
			}
		}

		// ドラッグ＆ドロップ操作が完了したときに発生する 
		private void	FormDragDrop( object sender, DragEventArgs args ){
			String[]	strAry = (String[])args.Data.GetData( DataFormats.FileDrop );
			if( strAry.Length > 0 )
			{
			//	MessageBox.Show(strAry[0],"");
				AddShutcutButton( strAry[0] );
			}
		}

	//	protected override void		OnPaint( PaintEventArgs args ){
	//		Assembly asm = Assembly.GetExecutingAssembly();
	//		Bitmap	bmp = new Bitmap(asm.GetManifestResourceStream(@"Happy.png"));
	//		args.Graphics.DrawImage( bmp, this.ClientRectangle, new Rectangle(0,0,bmp.Width,bmp.Height), GraphicsUnit.Pixel);
	//	}

		protected override void		WndProc( ref Message msg ){
			base.WndProc( ref msg );
			if( (msg.Msg == 0x84) && (msg.Result == (IntPtr)1) ){
		//		msg.Result	= (IntPtr)2;
			}
		}

		protected void	AddShutcutButton( String strShutcut ){
			CShutcutButton btn	= new CShutcutButton( strShutcut );

			tooltip.SetToolTip( btn, strShutcut );
			FlowLayout.Controls.Add( btn );
		}

		protected void	InitControls()
		{
			FlowLayout	= new FlowLayoutPanel(){
				Dock			= DockStyle.Fill,
				FlowDirection	= FlowDirection.LeftToRight,
				AutoScroll		= true,
				Margin			= new Padding(1),
			};

		/*
			for( int i=1; i<=15; i++ ){
				CShutcutButton	btn = new CShutcutButton(){
			//	Button			btn = new Button(){
					Text		= i.ToString(),
					FlatStyle	= FlatStyle.Popup,
				//	Size		= new Size(20, 20),
				//	Style		= BorderStyle.None,
				};

				tooltip.SetToolTip(btn, btn.Text) ;
				FlowLayout.Controls.Add( btn );
			}
		*/
			Controls.Add( FlowLayout );
		}
	}

	class CMain {
		[STAThread]
		public static void	Main( String[] args ){
			Application.Run(new CMainForm());
		}
	}
}
