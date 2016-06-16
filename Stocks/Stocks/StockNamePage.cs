using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace Stocks
{
    public class StockNamePage : TabPage
    {
        private DataGridView dataGridView1 = null;

        public StockNamePage()
            : base()
        {
            InitializeDataGridView();
        }

        public StockNamePage(string text)
            : base(text)
        {
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            this.dataGridView1 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();

            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 99);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(240, 150);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.Dock = DockStyle.Fill;

            foreach (var col in Enum.GetValues(typeof(issue_columns)))
            {
                this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Name = col.ToString(),
                    DataPropertyName = col.ToString(),
                });
            }

            this.Controls.Add(this.dataGridView1);

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
        }

        public void DefaultValue()
        {
            var src = new BindingSource();
            src.DataSource = typeof(StockName);

            for (UInt16 i = 1; i < 256; i++)
            {
                src.Add(new StockName() { code = i, symbol = "symbol" + i, name = "name" + i });
            }

            this.dataGridView1.DataSource = src;
        }

        public void Load(String filename)
        {
            var doc = new XmlDocument();

            try
            {
                doc.Load(filename);
            }
            finally
            {
            }
        }

        public void Save(String filename)
        {
            var wr = new XmlTextWriter(filename, System.Text.Encoding.UTF8);

            try
            {
                wr.WriteStartDocument();
                wr.WriteStartElement("issue_table");
                wr.WriteString(Environment.NewLine);

                foreach (var row in this.dataGridView1.Rows.Cast<DataGridViewRow>())
                {
                    if( row.IsNewRow )
                    {
                        continue;
                    }

                    wr.WriteString("\t");
                    wr.WriteStartElement("issue");
                    wr.WriteString(Environment.NewLine);

                    //foreach (var cell in row.Cells.Cast<DataGridViewCell>())
                    //{
                    //    cell.ToString
                    //}
                    int idx = 0;
                    foreach (var item in Enum.GetValues(typeof(issue_columns)))
                    {
                        wr.WriteString("\t\t");
                        wr.WriteStartElement(item.ToString());
                        wr.WriteValue(row.Cells[idx].Value.ToString());
                        wr.WriteEndElement();
                        wr.WriteString(Environment.NewLine);
                        idx++;
                    }

                    wr.WriteString("\t");
                    wr.WriteEndElement();
                    wr.WriteString(Environment.NewLine);
                }

                wr.WriteEndElement();
            //    wr.WriteString(Environment.NewLine);
            }
            finally
            {
                wr.Close();
            }
        }

    }
}