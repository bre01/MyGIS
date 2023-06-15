using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My.GIS
{
    public partial class GISPanel : UserControl
    {
        MOUSECOMMAND _mouseCommand = MOUSECOMMAND.Pan;
        int _startX = 0;
        int _startY = 0;
        int _mouseMovingX = 0;
        int _mouseMovingY = 0;
        bool _mouseOnMap = false;
        //Layer _layer = null;
        GISDocument _document = new GISDocument();
        MapAndClientConverter _converter = null;
        AttributeWindow _attributeWindow = null;
        Dictionary<Layer, AttributeWindow> _allAttributeWindow = new Dictionary<Layer, AttributeWindow>();
        Bitmap _backWindow;
        public GISDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        public GISPanel()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.MouseWheel += new MouseEventHandler(GISPanel_MouseWheel);
            panToolStripMenuItem.Checked = true;
            this.AutoSize = true;

        }
        private void GISPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int i = e.Delta;
            if (i < 0)
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
            else if (i > 0)
            {
                GISMapExtent e1 = _converter.GetDisplayExtent();
                GISVertex mouseOnMapLocation = _converter.ToMapVertex(new Point(e.X, e.Y));
                double newWidth = e1.Width / GISConst.ZoomOutFactor;
                double newHeight = e1.Height / GISConst.ZoomOutFactor;
                double newMinX = mouseOnMapLocation.x - (mouseOnMapLocation.x - e1.MinX) / GISConst.ZoomOutFactor;
                double newMinY = mouseOnMapLocation.y - (mouseOnMapLocation.y - e1.MinY) / GISConst.ZoomOutFactor;
                _converter.UpdateDisplayExtent(new GISMapExtent(newMinX, newMinX + newWidth, newMinY, newMinY + newHeight));

            }
            DrawMap();
            UpdateStatusBar();
        }
        public void UpdateAndDraw()
        {
            if (_converter == null)
            {
                if (_document.IsEmpty()) return;
                _converter = new MapAndClientConverter(new GISMapExtent(_document.Extent), ClientRectangle);
            }
            _converter.UpdateConverter(new GISMapExtent(_document.Extent), ClientRectangle);
            //_converter.UpdateConverter(_layer.DisplayExtent, this.ClientRectangle);
            DrawMap();
            UpdateStatusBar();
        }
        void UpdateStatusBar()
        {
            //toolStripStatusLabel1.Text = _layer.Selection.Count.ToString();
            toolStripStatusLabel1.Text = _document.Layers.Count.ToString();
            //x_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _document.Extent.minX(), _document.Extent.maxX());
            //y_extent_box.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _document.Extent.minY(), _document.Extent.maxY());
            //displayX.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _converter.GetDisplayExtent().minX(), _converter.GetDisplayExtent().maxX());
            //displayY.Text = String.Format("Min:" + "{0:0.000}" + " Max:" + "{1:0.00}", _converter.GetDisplayExtent().minY(), _converter.GetDisplayExtent().maxY());
        }


        private void DrawMap()
        {
            if (ClientRectangle.Width * ClientRectangle.Height == 0)
            {
                return;
            }
            //_converter.UpdateConverter(_layer.DisplayExtent, this.ClientRectangle);
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
        //private void map_button_Click(object sender, EventArgs e)
        //{
        //    GISMapActions mapActions = GISMapActions.zoomin;
        //    if ((Button)sender == button3)
        //    {
        //        mapActions = GISMapActions.zoomin;
        //    }
        //    else if ((Button)sender == button4)
        //    {
        //        mapActions = GISMapActions.zoomout;
        //    }
        //    else if ((Button)sender == button5)
        //    {
        //        mapActions = GISMapActions.moveViewDown;
        //    }
        //    else if ((Button)sender == button6)
        //    {
        //        mapActions = GISMapActions.moveViewUp;
        //    }
        //    else if ((Button)sender == button7)
        //    {
        //        mapActions = GISMapActions.moveViewRight;
        //    }
        //    else if ((Button)sender == button8)
        //    {
        //        mapActions = GISMapActions.moveViewLeft;
        //    }
        //    _converter.ChangeView(mapActions);
        //    this.DrawMap();



        //}


        private void GISPanel_SizeChanged(object sender, EventArgs e)
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
            AttributeWindow attributeWindow = null;
            if (_allAttributeWindow.ContainsKey(layer))
            {
                attributeWindow = _allAttributeWindow[layer];
                _allAttributeWindow.Remove(layer);
            }
            if (attributeWindow == null)
            {
                attributeWindow = new AttributeWindow(layer, this);
            }
            if (attributeWindow.IsDisposed)
            {
                attributeWindow = new AttributeWindow(layer, this);
            }
            _allAttributeWindow.Add(layer, attributeWindow);
            attributeWindow.Show();
            if (attributeWindow.WindowState == FormWindowState.Minimized)
            {
                _attributeWindow.WindowState = FormWindowState.Normal;
                _attributeWindow.BringToFront();
            }
        }



        private void UpdateAttributeWindow()
        {
            if (_document.IsEmpty()) return;
            //if (_attributeWindow == null) return;
            //if (_attributeWindow.IsDisposed) return;
            //_attributeWindow.UpdateData();
            foreach (AttributeWindow attributeWindow in _allAttributeWindow.Values)
            {
                if (attributeWindow == null) continue;
                if (attributeWindow.IsDisposed) continue;
                attributeWindow.UpdateData();
            }
        }

        private void GISPanel_Paint(object sender, PaintEventArgs e)
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

        private void GISPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _startX = e.X;
            _startY = e.Y;
            _mouseOnMap = (e.Button == MouseButtons.Left && _mouseCommand != MOUSECOMMAND.Unused);
        }

        private void GISPanel_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseMovingX = e.X;
            _mouseMovingY = e.Y;
            if (_mouseOnMap) Invalidate();
        }

        private void GISPanel_MouseUp(object sender, MouseEventArgs e)
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
                    //case MOUSECOMMAND.Zoom:
                    //    GISMapExtent e1 = _converter.GetDisplayExtent();
                    //    GISVertex mouseOnMapLocation = _converter.ToMapVertex(new Point(e.X, e.Y));
                    //    double newWidth = e1.Width / GISConst.ZoomOutFactor;
                    //    double newHeight = e1.Height / GISConst.ZoomOutFactor;
                    //    double newMinX = mouseOnMapLocation.x - (mouseOnMapLocation.x - e1.MinX) / GISConst.ZoomOutFactor;
                    //    double newMinY = mouseOnMapLocation.y - (mouseOnMapLocation.y - e1.MinY) / GISConst.ZoomOutFactor;
                    //    _converter.UpdateDisplayExtent(new GISMapExtent(newMinX, newMinX + newWidth, newMinY, newMinY + newHeight));
                    //    break;

            }
        }



        private void GISPanel_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this.PointToScreen(new Point(e.X, e.Y)));
            }
        }
        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                //panToolStripMenuItem.Checked = false;
                ((ToolStripMenuItem)sender).Checked = true;
                if (sender.Equals(zoomInToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.ZoomIn;
                    panToolStripMenuItem.Checked = false;
                }
                else if (sender.Equals(zoomOutToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.ZoomOut;
                    panToolStripMenuItem.Checked = false;
                }
                else if (sender.Equals(panToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.Pan;
                }
                else if (sender.Equals(selectToolStripMenuItem))
                {
                    _mouseCommand = MOUSECOMMAND.Select;
                    panToolStripMenuItem.Checked = false;
                }
            }
        }

    }
}
