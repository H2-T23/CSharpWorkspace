// XmlView.cs 
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;


namespace XMlVIEW {

	class CEditTreeView : TreeView {
		public CEditTreeView(){
			CheckBoxes		= true;

			ContextMenuStrip	contextmenu = new ContextMenuStrip();
			contextmenu.Items.Add("Add Node"	, null, new EventHandler(OnAddNode));
			contextmenu.Items.Add("Add Child"	, null, new EventHandler(OnAddChild));
			ContextMenuStrip	= contextmenu;

			MouseDoubleClick 			+= new MouseEventHandler(OnMouseDoubleClick);
			AfterLabelEdit				+= new NodeLabelEditEventHandler(OnAfterLabelEdit);
		}

		private void	OnMouseClick( Object sender, MouseEventArgs args ){
		//	SelectedNode	= GetNodeAt(args.X, args.Y);
		}

		private void	OnMouseDoubleClick( Object sender, MouseEventArgs args ){
		//	MessageBox.Show("","");
			if( !SelectedNode.IsEditing ){
				LabelEdit	= true;
				SelectedNode.BeginEdit();
			}
		}

		private void	OnAfterLabelEdit( Object sender, NodeLabelEditEventArgs args ){
			if( args.Label != null )
			{
				if( (args.Label.Length > 0) && (args.Label.IndexOfAny(new char[]{'@',',','.','!'}) == -1) )
				{
					args.Node.EndEdit( false );
				}
				else
				{
					args.CancelEdit	= true;
					args.Node.BeginEdit();
				}
				LabelEdit	= false;
			}
		}

		private void	OnAddNode( Object sender, EventArgs args ){
		//	MessageBox.Show("OnAddNode");
			BeginUpdate();
			if( TopNode != SelectedNode )
				SelectedNode.Parent.Nodes.Add("New Node");
			EndUpdate();
		}

		private void	OnAddChild( Object sender, EventArgs args ){
		//	MessageBox.Show("OnAddChild");
			BeginUpdate();
			SelectedNode.Nodes.Add("New Child");
			SelectedNode.Expand();
			EndUpdate();
		}
	}

	class CMainForm : Form {
		private	MainMenu			m_menuMain;
		private SplitContainer		m_Splitter;
		private CEditTreeView		m_TreeView;
		private ListView			m_ListView;

		public CMainForm(){
			Load	+= new EventHandler(OnLoadMainForm);
			Closed	+= new EventHandler(OnClosedMainForm);
		}

		private void	OnLoadMainForm( Object sender, EventArgs args ){
			Size	= new Size(600,600);
			InitMenu();
			InitCtrls();

			m_TreeView.BeginUpdate();	
			m_TreeView.Nodes.Add( new TreeNode("root") );
			m_TreeView.Nodes[0].Nodes.Add("Child 1");
			m_TreeView.Nodes[0].Nodes.Add("Child 2");
			m_TreeView.Nodes[0].Nodes[1].Nodes.Add("GrandChild");
			m_TreeView.Nodes[0].Nodes[1].Nodes[0].Nodes.Add("Great GrandChild");
			m_TreeView.EndUpdate();
		}

		private void	PrintTreeNode( TreeNode node, ref StreamWriter fsw ){
			System.Diagnostics.Debug.WriteLine( node.Text );

			String	str	= String.Format("<node>{0}</node>", node.Text);
			fsw.WriteLine( str );
			fsw.WriteLine( node.FullPath );

			foreach( TreeNode n in node.Nodes ){
				PrintTreeNode( n, ref fsw );
			}
		}

		private void	PrintTreeView(){
			StreamWriter	fsw	= null;
			try{
				fsw = File.CreateText(@"node.txt");

				TreeNodeCollection	nodes = m_TreeView.Nodes;
				foreach( TreeNode n in nodes ){
					PrintTreeNode( n, ref fsw );
				}
			}
			finally{
				if( fsw != null ){
					fsw.Dispose();
				}
			}
		}


		private void	OnClosedMainForm( Object sender, EventArgs args ){
		}

		private void	OnFileNew( Object sender, EventArgs args ){
			PrintTreeView();
		}

		private void	OnFileExit( Object sender, EventArgs args ){
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

				m_menuMain	= new MainMenu();
				m_menuMain.MenuItems.AddRange( new MenuItem[]{
					menuFile
				});

				this.Menu	= m_menuMain;

				bRet	= true;;
			}
			catch( Exception e ){
				throw e;
			}

			return bRet;
		}

		protected void	InitCtrls(){
			m_TreeView		= new CEditTreeView(){
				Dock			= DockStyle.Fill,
			};

			m_ListView		= new ListView(){
				Dock			= DockStyle.Fill,
				View			= View.Details,
				GridLines		= true,
			};

			m_Splitter		= new SplitContainer(){
				Dock			= DockStyle.Fill,
				Orientation		= Orientation.Vertical,
			};

			m_Splitter.Panel1.Controls.Add( m_TreeView );
			m_Splitter.Panel2.Controls.Add( m_ListView );

			Controls.Add( m_Splitter );
		}
	}

	class CMain {
		[STAThread]
		public static void	Main( String[] args ){
			Application.EnableVisualStyles();
			Application.Run(new CMainForm());	
		}
	}
}
