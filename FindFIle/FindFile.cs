// FildFile.cs 
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Threading;

namespace FINDFILE {

	class CCodeFileInfo {
		public CCodeFileInfo(){}
		public CCodeFileInfo( String str ){
			 SetFilePath( str );
		}

		public String			strFilePath;
		public long				nFileSize;
		public int				nLineNum;

		public override String	ToString(){
			return String.Format( "{0} ({1}|{2})", strFilePath, nFileSize, nLineNum );
		}

		public void		SetFilePath( String str ){
			strFilePath	= str;
			FileSize();
			LineNum();
		}

		protected void	FileSize(){
			try{
				FileInfo info = new FileInfo(strFilePath);
				nFileSize	= info.Length;
			}catch( Exception ){
				nFileSize	= 0;
			}
		}

		protected void	LineNum(){
			nLineNum	= 0;
			int	nLine	= 0;
			using( StreamReader sr = new StreamReader(strFilePath) ){
				String	str;
				do{
					str	= sr.ReadLine();
					if( str != null )
					{
						nLine++;
						String	strTrim	= str.Trim();
						if( strTrim.Length > 0 && !strTrim.StartsWith("//") )
						{
							nLineNum++;
						//	Console.WriteLine( "{0}:*{1}", nLine++, str );
						}
						else
						{
						//	Console.WriteLine( "{0}: {1}", nLine++, str );
						}
					}
				}while( str != null );
			}
		}
	}

	class CCCodeFileInfoListView : ListView {
		public CCCodeFileInfoListView(){
			Dock					= DockStyle.Fill;
			View					= View.Details;
		//	LabelEdit				= true;
			AllowColumnReorder		= true;
		//	CheckBoxes				= true;
			FullRowSelect			= true;
			GridLines				= true;
		//	Sorting					= SortOrder.Ascending;

			Columns.Add("No"		, -2, HorizontalAlignment.Left);
			Columns.Add("FilePath"	, 300, HorizontalAlignment.Left);
			Columns.Add("Size"		, -2, HorizontalAlignment.Left);
			Columns.Add("Line"		, -2, HorizontalAlignment.Left);

			Append( "sampleA.xml" );
			Append( "sampleB.xml" );
			Append( new String[]{"D.cvs","E.cvs","F.exe"} );
		}

		public void		Append( CCodeFileInfo CodeInfo ){
			Items.Add( new ListViewItem(new String[]{(Items.Count+1).ToString(), CodeInfo.strFilePath, CodeInfo.nFileSize.ToString(), CodeInfo.nLineNum.ToString()}) );
		}

		public void		Append( String strFilePath ){
			Items.Add( new ListViewItem(new String[]{(Items.Count+1).ToString(), strFilePath}) );
		}

		public void		Append( String strFilePath, long lSize, int nLine ){
			Items.Add( new ListViewItem(new String[]{(Items.Count+1).ToString(), strFilePath, lSize.ToString(), nLine.ToString()}) );
		}

		public void		Append( String strFilePath, String strSize, String strLine ){
			Items.Add( new ListViewItem(new String[]{(Items.Count+1).ToString(), strFilePath, strSize, strLine}) );
		}

		public void		Append( String[] strFilePaths ){
			foreach( String s in strFilePaths ){
				this.Append( s );
			}			
		}

		public void		RemoveAll(){
			Items.Clear();	
		}
	}

	class CInfoForm : Form {
		private CCCodeFileInfoListView	m_listview = new CCCodeFileInfoListView();

		public CInfoForm(){
			Load += new EventHandler(OnLoadInfoForm);			
		}

		private void	OnLoadInfoForm( Object sender, EventArgs args ){
			ClientSize	= new Size(300,300);
			Controls.Add( m_listview );	
		}
	}

	class CMainForm : Form {
	//	protected ListBox					m_listbox;
		protected CCCodeFileInfoListView	m_listview			= new CCCodeFileInfoListView();

		protected String					m_SelectedPath		= "C:\\";

		private FolderBrowserDialog			SelectFolderDlg		= new FolderBrowserDialog();

		private MainMenu					menuMain			= new MainMenu();
		private MenuItem					miFile, miOpen;
		private MenuItem					miFolder, miClose;

		private StatusBar					m_StatusBar	= new StatusBar();
		private StatusBarPanel				m_statPane1	= new StatusBarPanel();
		private StatusBarPanel				m_statPane2	= new StatusBarPanel();

		protected static ArrayList			m_AllFiles		= new ArrayList();
		protected static long				m_lTotalSize	= 0;

		public void		CalcFileSize(){
			m_lTotalSize	= 0;
			foreach( String str in m_AllFiles )
			{
				try{
					FileInfo info = new FileInfo(str);
					m_lTotalSize	+= info.Length;
				}catch( Exception ){
				}
			}

		//	MessageBox.Show(m_lTotalSize.ToString(), "");
		}

		public static void	GetAllFiles( String strFolder, String strPattern, ref ArrayList files ){
			try{
				String[]	fs	= Directory.GetFiles( strFolder, strPattern );
				files.AddRange( fs );

				String[]	ds	= Directory.GetDirectories( strFolder );
				foreach( String str in ds ){
			//		Console.WriteLine( str );
					GetAllFiles( str, strPattern, ref files );
				}
			}
			catch( Exception ){
				return;
			}
		}

		public CMainForm(){
			InitMenu();
			InitStatusBar();
			InitControls();
		}

		public CMainForm( String strTitle, Size siz ){
			this.Text			= strTitle;
			this.ClientSize		= siz;
			InitMenu();
			InitStatusBar();
			InitControls();
		}

		protected void	InitMenu(){
			miFile		= new MenuItem("File");
			miOpen		= new MenuItem("Open"			, new EventHandler(OnMenuOpen));
			miClose		= new MenuItem("Close"			, new EventHandler(OnMenuClose));
			miFolder	= new MenuItem("Select Folder"	, new EventHandler(OnMenuSelectFolder));

			menuMain.MenuItems.Add( miFile );
			
			miFile.MenuItems.AddRange( new MenuItem[]{miOpen, miClose, miFolder} );

			Menu	= menuMain;
		}

		protected void	InitStatusBar(){
			m_statPane1.BorderStyle	= StatusBarPanelBorderStyle.Sunken;
			m_statPane1.Text			= "Ready...";
			m_statPane1.AutoSize		= StatusBarPanelAutoSize.Spring;

			m_statPane2.BorderStyle	= StatusBarPanelBorderStyle.Raised;
			m_statPane2.Text			= System.DateTime.Today.ToLongDateString();
			m_statPane2.AutoSize		= StatusBarPanelAutoSize.Contents;
			m_statPane2.ToolTipText	= "Started: " + System.DateTime.Now.ToShortTimeString();

			m_StatusBar.ShowPanels	= true;

			m_StatusBar.Panels.Add( m_statPane1 );
			m_StatusBar.Panels.Add( m_statPane2 );

			Controls.Add( m_StatusBar );
		}

		protected void	InitControls(){
		//	m_listbox	= new ListBox(){
		//		Dock	= DockStyle.Fill,
		//	};
		//
		//	Controls.Add( m_listbox );
			Controls.Add( m_listview );

			SelectFolderDlg.Description			= "Select Folder";
			SelectFolderDlg.ShowNewFolderButton	= false;
		//	SelectFolderDlg.RootFolder			= Environment.SpecialFolder.Personal;
			SelectFolderDlg.SelectedPath		= m_SelectedPath;
		}

		public void		DoTask(){
			int	nCount	= 0;

		//	m_listbox.Items.Clear();
			m_listview.RemoveAll();
			m_AllFiles.Clear();

			ArrayList	files	= new ArrayList();
		//	GetAllFiles( @"C:\Windows\System32", "*.dll", ref files );
		//	GetAllFiles( @"D:\Users\Tsutsumi\a25ProjectHEADDev0", "*.cpp", ref files );
			GetAllFiles( m_SelectedPath, "*.cpp", ref files );
			m_AllFiles.AddRange( files );
			nCount	+= files.Count;
		//	m_listbox.Items.AddRange( files.ToArray() );
			files.Clear();

		//	GetAllFiles( @"D:\Users\Tsutsumi\a25ProjectHEADDev0", "*.h", ref files );
			GetAllFiles( m_SelectedPath, "*.h", ref files );
			m_AllFiles.AddRange( files );
			nCount	+= files.Count;
		//	m_listbox.Items.AddRange( files.ToArray() );
			files.Clear();

		//	MessageBox.Show( nCount.ToString() );

		//	Console.WriteLine( "Count: {0}/{1}", nCount, m_AllFiles.Count );

		//	foreach( String str in files ){
		//		Console.WriteLine( str );
		//	}	

		//	m_listbox.Items.AddRange( files.ToArray() );
		//	m_listbox.Items.AddRange( m_AllFiles.ToArray() );	
		//	m_listview.Append( (String[])m_AllFiles.ToArray() );

		//	new Thread( new ThreadStart(CalcFileSize) ).Start();
			CalcFileSize();
		}

		public void		DoTaskResult( IAsyncResult r ){
			long	nTotalSize		= 0;
			int		nTotalLine		= 0;

			m_listview.BeginUpdate();
			foreach( String str in m_AllFiles.ToArray() ){
				CCodeFileInfo	info = new CCodeFileInfo(str);
				nTotalSize	+= info.nFileSize;
				nTotalLine	+= info.nLineNum;
				m_listview.Append( info );
			}
			m_listview.Append( "", nTotalSize.ToString(), nTotalLine.ToString() );
			m_listview.EndUpdate();

			m_statPane1.Text	= String.Format( "{0}KB({1})", m_lTotalSize/1024, m_lTotalSize.ToString());
		}

		private void	OnMenuOpen( Object sender, EventArgs args ){
			(new CInfoForm()).Show();			
		}

		private void	OnMenuClose( Object sender, EventArgs args ){
			MessageBox.Show((new CCodeFileInfo( "FindFile.cs" )).ToString());
		}

		private void	OnMenuSelectFolder( Object sender, EventArgs args ){
			SelectFolderDlg.SelectedPath		= m_SelectedPath;

			if( SelectFolderDlg.ShowDialog() == DialogResult.OK ){
				m_SelectedPath	= SelectFolderDlg.SelectedPath;

				Action act = DoTask;
				act.BeginInvoke( DoTaskResult, null );
			}
		}
	}

	class CMain {
		[STAThreadAttribute]
		public static void	Main( String[] args ){
			Application.EnableVisualStyles();
			Application.Run(new CMainForm("MainForm",new Size(600,600)));
		}
	}
}
