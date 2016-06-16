using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Stocks
{
    public partial class MainForm : Form
    {
        public enum MenuFile { New, Open, Save, Close, }
        public enum MenuHelp { Version, }

        private MainMenu mainMenu = new MainMenu();
        private TabControl tabCtrl = new TabControl();
        private TabPage[] pages = new TabPage[3];

        public MainForm()
        {
            InitializeComponent();
            InitializeMainMenu();
            InitializeTab();
        }

        private void InitializeTab()
        {
            this.pages[0] = new StockNamePage()
            {
                Text = "StockName",
                TabIndex = 0,
                Size = this.tabCtrl.ClientSize,
            };
            this.pages[1] = new TabPage()
            {
                Text = "Page2",
                TabIndex = 1,
                Size = this.tabCtrl.ClientSize,
            };
            this.pages[2] = new TabPage()
            {
                Text = "Page3",
                TabIndex = 2,
                Size = this.tabCtrl.ClientSize,
            };

            this.tabCtrl.Dock = DockStyle.Fill;
            this.tabCtrl.TabPages.AddRange(pages);
            this.Controls.Add(this.tabCtrl);
        }

        private void InitializeMainMenu()
        {
            var menuFile = new MenuItem[]{
                new MenuItem("New(&N)"),
                new MenuItem("Open(&O)"),
                new MenuItem("Save(&S)"),
                new MenuItem("Close(&X)"),
            };

            var menuHelp = new MenuItem[]{
                new MenuItem("Version(&V)"),
            };

            foreach (var item in menuFile)
            {
                item.Click += new EventHandler(this.OnClickMenuFile);
            }

            foreach (var item in menuHelp)
            {
                item.Click += new EventHandler(this.OnClickMenuHelp);
            }

            this.mainMenu.MenuItems.Add(new MenuItem("File(&F)", menuFile));
            this.mainMenu.MenuItems.Add(new MenuItem("Help(&H)", menuHelp));
            this.Menu = mainMenu;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ((StockNamePage)this.pages[0]).DefaultValue();
        }

        private void OnClickMenuFile(object sender, EventArgs e)
        {
            switch (((MenuItem)sender).Index)
            {
                case (int)MenuFile.Open:
                    Open();
                    break;

                case (int)MenuFile.Save:
                    Save();
                    break;

                case (int)MenuFile.Close:
                    this.Close();
                    break;

                default:
                    break;
            }
        }

        private void OnClickMenuHelp(object sender, EventArgs e)
        {
        }

        private void Open()
        {
            var dlg = new OpenFileDialog()
            {
                FileName = "StockName.xml",
                Filter = "XML Files(*.xml)|*.xml",
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ((StockNamePage)this.pages[0]).Load(dlg.FileName);
            }
        }

        private void Save()
        {
            var dlg = new SaveFileDialog()
            {
                FileName = "StockName.xml",
                Filter = "XML Files(*.xml)|*.xml",
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ((StockNamePage)this.pages[0]).Save(dlg.FileName);
            }
        }

    }
}
