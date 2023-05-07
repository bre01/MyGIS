using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using My.GIS;
namespace Wpf8F
{
    /// <summary>
    /// Interaction logic for AttributeWindow.xaml
    /// </summary>
    public partial class AttributeWindow : Window
    {
        public AttributeWindow(Layer layer)
        {
            InitializeComponent();

            for (int i = 0; i < layer.Fields.Count; i++)
            {
                //dataGridView1.Columns.Add(layer.Fields[i].Name, layer.Fields[i].Name);
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = layer.Fields[i].Name;
                column.Binding = new Binding(layer.Fields[i].Name);
                AttributeGrid.Columns.Add(column);
            }
            for (int j = 0; j < layer.FeatureCount(); j++)
            {
                dynamic row = new ExpandoObject();
                for (int i = 0; i < layer.Fields.Count; i++)
                {
                    //dataGridView1.Rows.Add();
                    //for (int j = 0; j < layer.Fields.Count; j++)
                    //{
                    //    dataGridView1.Rows[i].Cells[j].Value = layer.GetFeature(i).GetAttribute(j);
                    //}
                    ((IDictionary<string, object>)row)[layer.Fields[i].Name] = layer.GetFeature(j).GetAttribute(i);

                }
                AttributeGrid.Items.Add(row);
            }
            //DataGridTextColumn textColumn = new DataGridTextColumn();
            //textColumn.Header = "Name";
            //AttributeGrid.
            //AttributeGrid
        }
        //public class Feature
        //{

        //    public Feature(Layer layer)
        //    {
        //        List<String> nameList = new List<string>;
        //        ArrayList typeList = new ArrayList();
        //        foreach (var field in layer.Fields)
        //        {
        //            nameList.Add(field.Name);
        //            typeList.Add(field.DataType);
        //        }


        //    }
        //    public Feature GetAFeature

        //    public List<Feature> GetFeatureList(Layer layer)
        //    {
        //        List<Feature> list = new List<Feature>();
        //        list.Add(new Feature());
        //        foreach (var field in layer.Fields)
        //        {

        //        }
        //    }
        //}

    }
}
