// ExampleFolderBrowserDialog.cs 
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;


class FolderSelectForm : Form {
	private FolderBrowserDialog		SelectFolderDlg		= new FolderBrowserDialog();
	private OpenFileDialog			OpenFileDlg			= new OpenFileDialog();
	private RichTextBox				RichText			= new RichTextBox();

	private MainMenu				menuMain			= new MainMenu();
	private MenuItem				miFile, miOpen;
	private MenuItem				miFolder, miClose;

	private String					strFolderPath;
	private bool					IsFileOpened		= false;

	public FolderSelectForm(){
		InitMenu();
		InitCtrls();

		ClientSize	= new Size(300,300);
		Text		= "RTF Document Browser";
	}

	protected void	InitMenu(){
		miFile		= new MenuItem("File");
		miOpen		= new MenuItem("Open"			, new EventHandler(OnMenuFile));
		miClose		= new MenuItem("Close"			, new EventHandler(OnMenuClose));
		miFolder	= new MenuItem("Select Folder"	, new EventHandler(OnMenuSelectFolder));

		menuMain.MenuItems.Add( miFile );
		
		miFile.MenuItems.AddRange( new MenuItem[]{miOpen, miClose, miFolder} );

		Menu	= menuMain;
	}

	protected void	InitCtrls(){
		OpenFileDlg.DefaultExt 				= "rtf";
		OpenFileDlg.Filter					= "rtf files(*.rtf)|*.rtf";

		SelectFolderDlg.Description			= "Select Folder";
		SelectFolderDlg.ShowNewFolderButton	= false;
		SelectFolderDlg.RootFolder			= Environment.SpecialFolder.Personal;
		SelectFolderDlg.SelectedPath		= "d:\\tsutsumi0000";

		RichText.AcceptsTab					= true;
		RichText.Location					= new Point(0, 0);
		RichText.Dock						= DockStyle.Fill;
	//	RichText.Anchor						= (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);

		Controls.Add( RichText );
	}

	private void	OnMenuFile( Object sender, EventArgs args ){
	}

	private void	OnMenuClose( Object sender, EventArgs args ){
	}

	private void	OnMenuSelectFolder( Object sender, EventArgs args ){
		if( SelectFolderDlg.ShowDialog() == DialogResult.OK ){
			strFolderPath	= SelectFolderDlg.SelectedPath;
			if( !IsFileOpened ){
			}
		}
	}
}

public class CEntryPoint {
	[STAThreadAttribute]
	public static void	Main(){
		Application.Run( new FolderSelectForm() );
	}
}
