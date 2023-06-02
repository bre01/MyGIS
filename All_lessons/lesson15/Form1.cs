using My.GIS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace lesson15
{
    public partial class Form1 : Form
    {
        MOUSECOMMAND _mouseCommand = MOUSECOMMAND.Unused;
        int _startX = 0;
        int _startY = 0;
        int _mouseMovingX = 0;
        int _mouseMovingY = 0;
        bool _mouseOnMap = false;
        //Layer _layer = null;
        GISDocument _document=new GISDocument();
        MapAndClientConverter _converter = null;
        Form2 _attributeWindow = null;
        Dictionary<Layer, Form2> _allAttributeWindow = new Dictionary<Layer, Form2>();
        Bitmap _backWindow;
        public Form1()
        {
            InitializeComponent();
            //_converter = new MapAndClientConverter(new GISMapExtent(new GISVertex(0, 0), new GISVertex(100, 100)), ClientRectangle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Shapefile file |*.shp";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            ShapefileTools shapefileTools = new ShapefileTools();
            _layer = shapefileTools.ReadShapefile(openFileDialog.FileName);
            _layer.DrawAttributeOrNot = false;
            MessageBox.Show("Read " + _layer.FeatureCount() + " objects");
            UpdateAndDraw();
            shape_box.Text = _layer.ShapeType.ToString();
            x_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.OriginalExtent.minX(), _layer.OriginalExtent.maxX());
            y_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.OriginalExtent.minY(), _layer.OriginalExtent.maxY());
            displayX.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _converter.GetDisplayExtent().minX(), _converter.GetDisplayExtent().maxX());
            displayY.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _converter.GetDisplayExtent().minY(), _converter.GetDisplayExtent().maxY());

            
            /*_converter.UpdateConverter(_layer.Extent,ClientRectangle);
            DrawMap();*/

            /*ShapefileTools shapfileTools = new ShapefileTools();
            _layer = shapfileTools.ReadShapefile(@"C:\Users\bre\Downloads\LANDrop\GisData\chap10\thermal.shp");
            string fileName = null;
            _layer.DrawAttributeOrNot = false;
            MessageBox.Show("Fount" + _layer.FeatureCount() + "Points" + fileName);*/

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //update map button clicked
            /*_converter.UpdateConverter(_layer.Extent, ClientRectangle);
            DrawMap();*/
            UpdateAndDraw();
        }
        public void UpdateAndDraw()
        {
            if (_converter == null)
            {
                if (_document.IsEmpty()) return;
                _converter = new MapAndClientConverter(new GISMapExtent(_document.Extent), ClientRectangle);
            }
            
            //_converter.UpdateConverter(_layer.DisplayExtent, this.ClientRectangle);
            DrawMap();
            UpdateStatusBar();
        }
        void UpdateStatusBar()
        {
            //toolStripStatusLabel1.Text = _layer.Selection.Count.ToString();
            toolStripStatusLabel1.Text=_document.Layers.Count.ToString();
            x_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.OriginalExtent.minX(), _layer.OriginalExtent.maxX());
            y_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _layer.OriginalExtent.minY(), _layer.OriginalExtent.maxY());
            displayX.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _converter.GetDisplayExtent().minX(), _converter.GetDisplayExtent().maxX());
            displayY.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _converter.GetDisplayExtent().minY(), _converter.GetDisplayExtent().maxY());
        }


        private void DrawMap()
        {
            if (ClientRectangle.Width * ClientRectangle.Height == 0)
            {
                return;
            }
            //_converter.UpdateConverter(_layer.DisplayExtent, this.ClientRectangle);
            _converter.UpdateRectangle(ClientRectangle);
            if (_backWindow != null)
            {
                _backWindow.Dispose();
            }
            _backWindow = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics backGraphics = Graphics.FromImage(_backWindow);

            //Graphics graphics = CreateGraphics();
            backGraphics.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), ClientRectangle);
            _document.Draw(backGraphics, _converter);
            Graphics frontGraphics = CreateGraphics();
            frontGraphics.DrawImage(_backWindow, 0, 0);

        }
        private void map_button_Click(object sender, EventArgs e)
        {
            GISMapActions mapActions = GISMapActions.zoomin;
            if ((Button)sender == button3)
            {
                mapActions = GISMapActions.zoomin;
            }
            else if ((Button)sender == button4)
            {
                mapActions = GISMapActions.zoomout;
            }
            else if ((Button)sender == button5)
            {
                mapActions = GISMapActions.moveViewDown;
            }
            else if ((Button)sender == button6)
            {
                mapActions = GISMapActions.moveViewUp;
            }
            else if ((Button)sender == button7)
            {
                mapActions = GISMapActions.moveViewRight;
            }
            else if ((Button)sender == button8)
            {
                mapActions = GISMapActions.moveViewLeft;
            }
            _converter.ChangeView(mapActions);
            this.DrawMap();



        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (_document.IsEmpty())
                UpdateAndDraw();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Form2 form2 = new Form2(_layer);
            //form2.Show();
            OpenAttributeWindow();

        }
        private void OpenAttributeWindow()
        {
            //if (_Layer == null) return;
            //if (_attributeWindow == null) _attributeWindow = new Form2(_layer, this);
            //if (_attributeWindow.IsDisposed) _attributeWindow = new Form2(_layer, this);
            //_attributeWindow.Show();
            //if (_attributeWindow.WindowState == FormWindowState.Minimized) _attributeWindow.WindowState = FormWindowState.Normal;
            //_attributeWindow.BringToFront();
        }
        public void OpenAttributeWindow(Layer layer)
        {
            Form2 attributeWindow = null;
            if(_allAttributeWindow.ContainsKey(layer))
            {
                attributeWindow= _allAttributeWindow[layer];
                _allAttributeWindow.Remove(layer);
            }
            if(attributeWindow == null)
            {
                attributeWindow=new Form2(layer, this);
            }
            if (attributeWindow.IsDisposed)
            {
                attributeWindow=new Form2(layer, this);
            }
            _allAttributeWindow.Add(layer, attributeWindow);
            _attributeWindow.Show();
            if(attributeWindow.WindowState==FormWindowState.Minimized)
            {
                _attributeWindow.WindowState=FormWindowState.Normal;
                _attributeWindow.BringToFront();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string fileName = dialog.FileName;
            MyFiles.WriteFile(_layer, fileName);
            MessageBox.Show("done");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string fileName = dialog.FileName;
            _layer = MyFiles.ReadFile(fileName);
            MessageBox.Show("Read " + _layer.FeatureCount() + " objects");
            _converter.UpdateConverter(_layer.DisplayExtent, ClientRectangle);
            DrawMap();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (_layer == null) return;
            _layer.ClearSelection();
            UpdateAndDraw();
            toolStripStatusLabel1.Text = "0";
            UpdateAttributeWindow();
        }
        private void UpdateAttributeWindow()
        {
            if (_document.IsEmpty()) return;
            //if (_attributeWindow == null) return;
            //if (_attributeWindow.IsDisposed) return;
            //_attributeWindow.UpdateData();
            foreach(Form2 attributeWindow in _allAttributeWindow.Values)
            {
                if (attributeWindow != null) continue;
                if (attributeWindow.IsDisposed) continue;
                attributeWindow.UpdateData();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_backWindow != null)
            {
                //e.Graphics.DrawImage(_backWindow, 0, 0);
                //if (_mouseOnMap)
                //{
                if (_mouseCommand == MOUSECOMMAND.Pan)
                {
                    e.Graphics.DrawImage(_backWindow, _mouseMovingX - _startX, _mouseMovingY - _startY);
                }
                else if (_mouseCommand != MOUSECOMMAND.Unused)
                {
                    e.Graphics.DrawImage(_backWindow, 0, 0);
                    e.Graphics.FillRectangle(new SolidBrush(GISConst.ZoomSelectBoxColor),
                        new Rectangle(Math.Min(_startX, _mouseMovingX), Math.Min(_startY, _mouseMovingY), Math.Abs(_startX - _mouseMovingX), Math.Abs(_startY - _mouseMovingY)));
                }
                else
                {
                    e.Graphics.DrawImage(_backWindow, 0, 0);
                }
                //}
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _startX = e.X;
            _startY = e.Y;
            _mouseOnMap = (e.Button == MouseButtons.Left && _mouseCommand != MOUSECOMMAND.Unused);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseMovingX = e.X;
            _mouseMovingY = e.Y;
            if (_mouseOnMap) Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_document.IsEmpty()) return;
            if (_mouseOnMap == false) return;
            _mouseOnMap = false;
            switch (_mouseCommand)
            {
                case MOUSECOMMAND.Select:
                    if (Control.ModifierKeys != Keys.Control) _document.ClearSelection();
                    SelectResult sr = SelectResult.UnknownType;
                    if (e.X == _startX && e.Y == _startY)
                    {
                        GISVertex v = _converter.ToMapVertex(new Point(e.X, e.Y));
                        sr = _document.Select(v, _converter);
                    }
                    else
                    {
                        GISMapExtent extent = _converter.ScreenRectToExtent(e.X, _startX, e.Y, _startY);
                        sr = _document.Select(extent);
                    }
                    if (sr == SelectResult.Ok || Control.ModifierKeys != Keys.Control)
                    {
                        UpdateAndDraw();
                        UpdateAttributeWindow();
                    }
                    break;
                case MOUSECOMMAND.ZoomIn:
                    if (e.X == _startX && e.Y == _startY)
                    {
                        GISVertex mouseOnMapLocation = _converter.ToMapVertex(new Point(e.X, e.Y));
                        GISMapExtent e1 = _converter.GetDisplayExtent();
                        double newWidth = e1.Width * GISConst.ZoomInFactor;
                        double newHeight = e1.Height * GISConst.ZoomInFactor;
                        double newMinX = mouseOnMapLocation.x - (mouseOnMapLocation.x - e1.MinX) * GISConst.ZoomInFactor;
                        double newMinY = mouseOnMapLocation.y - (mouseOnMapLocation.y - e1.MinY) * GISConst.ZoomInFactor;
                        // _layer.DisplayExtent = new GISMapExtent(newMinX, newMinX + newWidth, newMinY, newMinY + newHeight);
                        _converter.UpdateDisplayExtent(new GISMapExtent(newMinX, newMinX + newWidth, newMinY, newMinY + newHeight));
                    }
                    else
                    {
                        //_layer.DisplayExtent = _converter.ScreenRectToExtent(e.X, _startX, e.Y, _startY);
                        _converter.UpdateDisplayExtent(_converter.ScreenRectToExtent(e.X, _startX, e.Y, _startY));
                    }
                    //UpdateAndDraw();
                    DrawMap();
                    UpdateStatusBar();
                    break;
                case MOUSECOMMAND.ZoomOut:
                    if (e.X == _startX && e.Y == _startY)
                    {
                        GISMapExtent e1 = _converter.GetDisplayExtent();
                        GISVertex mouseOnMapLocation = _converter.ToMapVertex(new Point(e.X, e.Y));
                        double newWidth = e1.Width / GISConst.ZoomOutFactor;
                        double newHeight = e1.Height / GISConst.ZoomOutFactor;
                        double newMinX = mouseOnMapLocation.x - (mouseOnMapLocation.x - e1.MinX) / GISConst.ZoomOutFactor;
                        double newMinY = mouseOnMapLocation.y - (mouseOnMapLocation.y - e1.MinY) / GISConst.ZoomOutFactor;
                        _converter.UpdateDisplayExtent(new GISMapExtent(newMinX, newMinX + newWidth, newMinY, newMinY + newHeight));

                    }
                    else
                    {
                        GISMapExtent e3 = _converter.ScreenRectToExtent(e.X, _startX, e.Y, _startY);
                        GISMapExtent e1 = _converter.GetDisplayExtent();
                        double newWidth = e1.Width * e1.Width / e3.Width;
                        double newHeight = e1.Height * e1.Height / e3.Height;
                        double newMinX = e3.MinX - (e3.MinX - e1.MinX) * newWidth / e1.Width;
                        double newMinY = e3.MinY - (e3.MinY - e1.MinY) * newHeight / e1.Height;
                        _converter.UpdateDisplayExtent(new GISMapExtent(newMinX, newMinX + newWidth, newMinY, newMinY + newHeight));
                    }
                    DrawMap();
                    UpdateStatusBar();
                    break;
                case MOUSECOMMAND.Pan:
                    if (e.X != _startX || e.Y != _startY)
                    {
                        GISMapExtent e1 = _converter.GetDisplayExtent();
                        GISVertex mouseOnMapLocation1 = _converter.ToMapVertex(new Point(_startX, _startY));
                        GISVertex mouseOnMapLocation2 = _converter.ToMapVertex(new Point(e.X, e.Y));
                        double newWidth = e1.Width;
                        double newHeight = e1.Height;
                        double newMinX = e1.MinX - (mouseOnMapLocation2.x - mouseOnMapLocation1.x);
                        double newMinY = e1.MinY - (mouseOnMapLocation2.y - mouseOnMapLocation1.y);
                        _converter.UpdateDisplayExtent(new GISMapExtent(newMinX, newMinX + newWidth, newMinY, newMinY + newHeight));
                        DrawMap();
                        UpdateStatusBar();
                    }
                    break;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_document.IsEmpty()) return;
            if (sender.Equals(openDocumentToolStripMenuItem))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "GIS Document (*." + GISConst.MYDOC + ")|*." + GISConst.MYDOC;
                dialog.RestoreDirectory = false;
                dialog.FilterIndex = 1;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                _document.Read(dialog.FileName);
                if (_document.IsEmpty() == false)
                {
                    UpdateAndDraw();
                }

            }
            else if (sender.Equals(layersToolStripMenuItem))
            {
                DocumentWindow layerControl = new DocumentWindow(_document, this);
                layerControl.ShowDialog();
            }
            else if (sender.Equals(zoomToLayerToolStripMenuItem))
            {
                UpdateAndDraw();
            }
            else
            {
                selectToolStripMenuItem.Checked = false;
                zoomInToolStripMenuItem.Checked = false;
                zoomOutToolStripMenuItem.Checked = false;
                panToolStripMenuItem.Checked = false;
                ((ToolStripMenuItem)sender).Checked = true;
                if (sender.Equals(zoomInToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.ZoomIn;
                }
                else if (sender.Equals(zoomOutToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.ZoomOut;
                }
                else if (sender.Equals(panToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.Pan;
                }
                else if (sender.Equals(selectToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.Select;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this.PointToScreen(new Point(e.X, e.Y)));
            }
        }
    }


}
