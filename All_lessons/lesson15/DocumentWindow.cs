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

namespace lesson15
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
                list.Items.Insert(0, _document.Layers[i].Name);
            }
            if (_document.Layers.Count > 0)
            {
                list.SelectedIndex = 0;
            }
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (list.SelectedItems == null) { return; }

            Layer layer = _document.GetLayer(list.SelectedItem.ToString());
            //layer.Selectable=checkBox1.Checked;
            //layer.Visible=checkBox2.Checked;
            //layer.DrawAttributeOrNot=checkBox3.Checked;
            checkBox1.Checked = layer.Selectable;
            checkBox2.Checked = layer.Visible;
            checkBox3.Checked = layer.DrawAttributeOrNot;
            //layer.LabelIndex = comboBox1.SelectedIndex;
            comboBox1.Items.Clear();
            for (int i = 0; i < layer.Fields.Count; i++)
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
            Layer layer = _document.AddLayer(dialog.FileName);
            list.Items.Insert(0, layer.Name);
            list.SelectedIndex = 0;
            _mapWindow.UpdateAndDraw();
            //_mapWindow.UpdateAndDraw();
            
        }
        private void Clicked(object sender, EventArgs e)
        {
            //event handler of three checkboxs
            if (list.SelectedItems == null) { return; }

            Layer layer = _document.GetLayer(list.SelectedItem.ToString());
            layer.Selectable = checkBox1.Checked;
            layer.Visible = checkBox2.Checked;
            layer.DrawAttributeOrNot = checkBox3.Checked;
            layer.LabelIndex = comboBox1.SelectedIndex;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem == null) return;
            for (int i = 0; i < list.Items.Count; i++)
            {
                if (i != list.SelectedIndex)
                {
                    if (list.Items[i].ToString() == textBox1.Text)
                    {
                        MessageBox.Show("layer exists");
                        return;
                    }
                    Layer layer = _document.GetLayer(list.SelectedItem.ToString());
                    layer.Name = textBox1.Text;
                    list.SelectedItem = textBox1.Text;

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem == null) return;
            _document.RemoveLayer(list.SelectedItem.ToString());
            list.Items.Remove(list.SelectedItem);
            if (list.Items.Count > 0) list.SelectedIndex = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem == null) return;
            if (list.SelectedIndex == 0) { return; }
            string selectedName = list.SelectedItem.ToString();
            string upperName = list.Items[list.SelectedIndex - 1].ToString();
            list.Items[list.SelectedIndex - 1] = selectedName;
            list.Items[list.SelectedIndex] = upperName;
            _document.SwitchLayer(selectedName, upperName);
            list.SelectedIndex -= 1;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem == null)
            {
                return;
            }
            if (list.Items.Count == 1) return;
            if (list.SelectedIndex == list.Items.Count - 1) { return; }
            string selectedName = list.SelectedItem.ToString();
            string lowerName = list.Items[list.SelectedIndex + 1].ToString();
            list.Items[list.SelectedIndex + 1] = selectedName;
            list.Items[list.SelectedIndex] = lowerName;
            _document.SwitchLayer(selectedName, lowerName);
            list.SelectedIndex += 1;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (list.SelectedItem == null) { return; }
            SaveFileDialog dialog= new SaveFileDialog();
            dialog.Filter = "GIS file(*." + GISConst.MYFILE + ")|*." + GISConst.MYFILE;
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = false;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                Layer layer=_document.GetLayer(list.SelectedItem.ToString());
                MyFiles.WriteFile(layer, dialog.FileName);
                MessageBox.Show("Done!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog= new SaveFileDialog();
            dialog.Filter = "GIS Document(*." + GISConst.MYDOC + ")|*." + GISConst.MYDOC;
            dialog.FilterIndex=1;
            dialog.RestoreDirectory = false;
            if(dialog.ShowDialog()== DialogResult.OK)
            {
                _document.Write(dialog.FileName);
                MessageBox.Show("Done!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(list.SelectedItem==null) { return; }
            Layer layer = _document.GetLayer(list.SelectedItem.ToString());
            _mapWindow.OpenAttributeWindow(layer);
        }

        private void DocumentWindow_Load(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            _mapWindow.UpdateAndDraw();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            _mapWindow.UpdateAndDraw();
            Close();
        }
        //private void DocumentWindowShow(object sender, EventArgs e)
        //{

        //}
    }
}
