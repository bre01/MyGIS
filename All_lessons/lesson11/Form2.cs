using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using My.GIS;

namespace lesson11
{
    public partial class Form2 : Form
    {
        Layer _layer;
        bool _fromMapWindow = true;
        Form1 _mapWindow = null;
        public Form2(Layer layer, Form1 mapWindow)
        {
            InitializeComponent();
            _layer = layer;
            _mapWindow = mapWindow;
            //for (int i = 0; i < layer.Fields.Count; i++)
            //{
            //    dataGridView1.Columns.Add(layer.Fields[i].Name, layer.Fields[i].Name);
            //}
            //for (int i = 0; i < layer.FeatureCount(); i++)
            //{
            //    dataGridView1.Rows.Add();
            //    for (int j = 0; j < layer.Fields.Count; j++)
            //    {
            //        dataGridView1.Rows[i].Cells[j].Value = layer.GetFeature(i).GetAttribute(j);
            //    }
            //}

        }
        private void Form2_Shown(object sender, EventArgs e)
        {
            _fromMapWindow = true;
            FillValue();
            _fromMapWindow = false;
        }
        private void FillValue()
        {
            dataGridView1.Columns.Add("ID", "ID");
            for (int i = 0; i < _layer.Fields.Count; i++)
            {
                dataGridView1.Columns.Add(_layer.Fields[i].Name, _layer.Fields[i].Name);
            }
            for (int i = 0; i < _layer.FeatureCount(); i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = _layer.GetFeature(i).ID;
                for (int j = 0; j < _layer.Fields.Count; j++)
                {
                    dataGridView1.Rows[i].Cells[j + 1].Value = _layer.GetFeature(i).GetAttribute(j);
                }
                dataGridView1.Rows[i].Selected = _layer.GetFeature(i).Selected;
            }

        }
        public void UpdateData()
        {
            _fromMapWindow = true;
            dataGridView1.ClearSelection();
            foreach (GISFeature feature in _layer.Selection)
            {
                SelectRowByID(feature.ID).Selected = true;
            }
            _fromMapWindow = false;
        }
        public DataGridViewRow SelectRowByID(int id)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((int)(row.Cells[0].Value) == id)
                {
                    return row;
                }
            }
            return null;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (_fromMapWindow)
            {
                return;
            }
            if (_layer.Selection.Count == 0 && dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            _layer.ClearSelection();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Cells[0].Value != null)
                {
                    _layer.AddSelectedFeatureByID((int)(row.Cells[0].Value));
                }
            }
            _mapWindow.UpdateAndDraw();


        }
    }
}
