using My.GIS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lesson14
{
    public partial class DocumentWindow : Form
    {
        GISDocument _document;
        Form1 _mapWindow;
        public DocumentWindow(GISDocument document, Form1 mapWindow)
        {
            InitializeComponent();
            _document = document;
            _mapWindow = mapWindow;
        }

        private void Documents_Shown(object sender, EventArgs e)
        {
            for (int i = 0; i < _document.Layers.Count; i++)
            {
                listBox1.Items.Insert(0, _document.Layers[i].Name);
            }
            if(_document.Layers.Count > 0)
            {
                listBox1.SelectedIndex = 0; 
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems==null) { return; }    

            Layer layer=_document.GetLayer(listBox1.SelectedItem.ToString());
            //layer.Selectable=checkBox1.Checked;
            //layer.Visible=checkBox2.Checked;
            //layer.DrawAttributeOrNot=checkBox3.Checked;
            checkBox1.Checked = layer.Selectable;
            checkBox2.Checked = layer.Visible;
            checkBox3.Checked = layer.DrawAttributeOrNot;
            //layer.LabelIndex = comboBox1.SelectedIndex;
            comboBox1.Items.Clear();
            for(int i = 0; i < layer.Fields.Count; i++)
            {
                comboBox1.Items.Add(layer.Fields[i].Name);
            }
            comboBox1.SelectedIndex = layer.LabelIndex;
            label1.Text = layer.path;
            textBox1.Text = layer.Name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "GIS Files(*." + GISConst.SHPFILE + ",*." + GISConst.MYFILE + ")|*." + GISConst.SHPFILE + ";*." + GISConst.MYFILE;
            dialog.RestoreDirectory = false;
            dialog.FilterIndex = 1;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() != DialogResult.OK) return;

        }
        private void Clicked(object sender, EventArgs e)
        {
            //event handler of three checkboxs
            if(listBox1.SelectedItems==null) { return; }    

            Layer layer=_document.GetLayer(listBox1.SelectedItem.ToString());
            layer.Selectable=checkBox1.Checked;
            layer.Visible=checkBox2.Checked;
            layer.DrawAttributeOrNot=checkBox3.Checked;
            layer.LabelIndex = comboBox1.SelectedIndex;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            for(int i=0;i<listBox1.Items.Count;i++)
            {
                if (i != listBox1.SelectedIndex)
                {
                    if (listBox1.Items[i].ToString() == textBox1.Text)
                    {
                        MessageBox.Show("layer exists");
                        return;
                    }
                    Layer layer = _document.GetLayer(listBox1.SelectedItem.ToString());
                    layer.Name = textBox1.Text;
                    listBox1.SelectedItem = textBox1.Text;

                }
            }
        }
        //private void DocumentWindowShow(object sender, EventArgs e)
        //{

        //}
    }
}
