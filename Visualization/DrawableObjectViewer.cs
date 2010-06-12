/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    DrawableObjectViewer.cs
 *  Desc:    Drawable object viewer
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Text;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Control DrawableObjectViewer
       |
       '-----------------------------------------------------------------------
    */
    public partial class DrawableObjectViewer : UserControl
    {   
        private const int RENDER_LAYER = 1;
        private const int MAIN_LAYER   = 2;

        private float mScaleFactor
            = 1;
        
        private Color mCanvasColor
            = Color.White;
        private Size mCanvasSize
            = new Size(800, 600);

        private Dictionary<int, BitmapInfo> mBmpCache
            = new Dictionary<int, BitmapInfo>();

        private IDrawableObject mDrawableObject
            = null;

        private EditableBitmap canvasView
            = null;

        private ArrayList<IDrawableObject> mTargetObjs
            = new ArrayList<IDrawableObject>();
        private Set<IDrawableObject> mTargetObjSet
            = new Set<IDrawableObject>();

        private ContextMenuStrip mCanvasMenu
            = null;
        
        private ToolTip mDrawableObjectTip
            = new ToolTip();
        
        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        private static void SetupGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
        }

        private BitmapInfo PrepareBitmap(int id, int width, int height)
        {
            if (!mBmpCache.ContainsKey(id))
            {
                EditableBitmap bmp = new EditableBitmap(width, height, PixelFormat.Format24bppRgb);
                Graphics gfx = Graphics.FromImage(bmp.Bitmap);
                SetupGraphics(gfx);                               
                gfx.Clear(mCanvasColor); 
                BitmapInfo bmpInfo = new BitmapInfo(bmp, gfx);
                mBmpCache.Add(id, bmpInfo);
                return bmpInfo;
            }
            else
            {
                BitmapInfo bmpInfo = mBmpCache[id];
                if (bmpInfo.Bitmap.Width >= width && bmpInfo.Bitmap.Height >= height) 
                {
                    bmpInfo.Graphics.Clip = new Region(new Rectangle(0, 0, width, height)); 
                    bmpInfo.Graphics.Clear(mCanvasColor); 
                    return bmpInfo;
                } 
                // remove old bitmap 
                bmpInfo.Dispose();
                mBmpCache.Remove(id);
                // create new bitmap
                EditableBitmap bmp = new EditableBitmap(width, height, PixelFormat.Format24bppRgb);
                Graphics gfx = Graphics.FromImage(bmp.Bitmap);
                SetupGraphics(gfx);
                gfx.Clear(mCanvasColor); 
                bmpInfo = new BitmapInfo(bmp, gfx);
                mBmpCache.Add(id, bmpInfo);
                return bmpInfo;
            }
        }

        private void SetupCanvas(int width, int height)
        {
            if (canvasView != null) { canvasView.Dispose(); }
            BitmapInfo mainLayer = mBmpCache[MAIN_LAYER];
            canvasView = mainLayer.EditableBitmap.CreateView(new Rectangle(0, 0, width, height)); 
        }

        private Rectangle GetEnclosingRect(RectangleF rect)
        {
            return new Rectangle((int)Math.Floor(rect.X), (int)Math.Floor(rect.Y), (int)Math.Ceiling(rect.Width + 1f), (int)Math.Ceiling(rect.Height + 1f));
        }
        
        public DrawableObjectViewer()
        {
            InitializeComponent();
            if (picBoxCanvas.Image == null)
            {
                Draw();
            }
        }

        [Browsable(false)]        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDrawableObject DrawableObject
        {
            get { return mDrawableObject; }
            set 
            { 
                mDrawableObject = value;
                mTargetObjSet.Clear();
                mTargetObjs.Clear();
                Draw();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ArrayList<IDrawableObject>.ReadOnly TargetObjects
        {
            get { return mTargetObjs; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Set<IDrawableObject>.ReadOnly TargetObjectSet
        {
            get { return mTargetObjSet; }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Size), "800, 600")]
        public Size CanvasSize
        {
            get { return mCanvasSize; }
            set 
            {
                //Utils.ThrowException(mDrawableObject != null ? new InvalidOperationException() : null);
                mCanvasSize = value;
                Draw();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color CanvasColor
        {
            get { return mCanvasColor; }
            set 
            {
                //Utils.ThrowException(mDrawableObject != null ? new InvalidOperationException() : null);
                mCanvasColor = value;
                Draw();
            }
        }

        [Category("Appearance")]
        [DefaultValue(1f)]
        public float ScaleFactor
        {
            get { return mScaleFactor; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("ScaleFactor") : null);
                mScaleFactor = value;
                Draw();
            }
        }

        [Category("Layout")]
        [DefaultValue(true)]
        new public bool AutoScroll
        {
            get { return base.AutoScroll; }
            set { base.AutoScroll = value; }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Cursor), "Default")]
        public Cursor CanvasCursor
        {
            get { return picBoxCanvas.Cursor; }
            set { picBoxCanvas.Cursor = value; }
        }

        [Category("Behavior")]
        [DefaultValue(null)]
        public ContextMenuStrip CanvasContextMenuStrip
        {
            get { return mCanvasMenu; }
            set { mCanvasMenu = value; }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolTip DrawableObjectToolTip
        {
            get { return mDrawableObjectTip; }            
        }

        public void ExtendBoundingArea(BoundingArea boundingArea, params IDrawableObject[] drawableObjects)
        {
            Utils.ThrowException(boundingArea == null ? new ArgumentNullException("boundingArea") : null);
            Utils.ThrowException(drawableObjects == null ? new ArgumentNullException("drawableObjects") : null);
            TransformParams tr = new TransformParams(0, 0, mScaleFactor);
            foreach (IDrawableObject drawableObject in drawableObjects)
            {
                boundingArea.AddRectangles(drawableObject.GetBoundingArea(tr).Rectangles);
            }
        }

        public void Draw()
        {
            const int WM_SETREDRAW = 0x000B;
            SendMessage(Handle, WM_SETREDRAW, 0, IntPtr.Zero); // supress redrawing
            TransformParams tr = new TransformParams(0, 0, mScaleFactor);
            int width = (int)Math.Ceiling(tr.Transform(mCanvasSize.Width));
            int height = (int)Math.Ceiling(tr.Transform(mCanvasSize.Height));
            if (mDrawableObject == null)
            {
                PrepareBitmap(MAIN_LAYER, width, height);
                SetupCanvas(width, height);
                picBoxCanvas.Image = canvasView.Bitmap;
            }
            else
            {
                BitmapInfo mainLayer = PrepareBitmap(MAIN_LAYER, width, height);
                mDrawableObject.Draw(mainLayer.Graphics, tr);
                SetupCanvas(width, height);
                picBoxCanvas.Image = canvasView.Bitmap;
            }
            SendMessage(Handle, WM_SETREDRAW, 1, IntPtr.Zero); // resume redrawing
            AutoScroll = true; // update scrollbars
            Refresh(); // repaint control
        }

        public void Draw(params IDrawableObject[] drawableObjects)
        {
            Draw(new BoundingArea(), drawableObjects);
        }

        // ****** Debugging ******
        private int mDrawCount
            = 0;
        private TimeSpan mDrawTime
            = TimeSpan.Zero;
        private ArrayList<double> mDrawInfo
            = new ArrayList<double>();

        private void FpsInfo_Click(object sender, EventArgs e)
        {
            double stdev = 0;
            double avg = (double)mDrawTime.TotalMilliseconds / (double)mDrawCount;
            foreach (double val in mDrawInfo)
            {
                stdev += (val - avg) * (val - avg);
            }
            stdev = Math.Sqrt(stdev / (double)mDrawInfo.Count);
            mDrawCount = 0;
            mDrawTime = TimeSpan.Zero;
            mDrawInfo.Clear();
            FpsInfo.Text = string.Format("{0:0.00} ({1:0.00}) ms / draw", avg, stdev);           
        }
        // ***********************

        public void Draw(BoundingArea.ReadOnly boundingArea, params IDrawableObject[] drawableObjects)
        {            
            Utils.ThrowException(boundingArea == null ? new ArgumentNullException("boundingArea") : null);
            Utils.ThrowException(drawableObjects == null ? new ArgumentNullException("drawableObjects") : null);
            DateTime startTime = DateTime.Now;
            BoundingArea extendedArea = boundingArea.GetWritableCopy();
            ExtendBoundingArea(extendedArea, drawableObjects);
#if !NO_BB_SIMPLIFICATION
            extendedArea.Optimize();
#endif
            TransformParams tr = new TransformParams(0, 0, mScaleFactor);
            Set<IDrawableObject> outdatedObjects = new Set<IDrawableObject>(drawableObjects);
            drawableObjects = mDrawableObject.GetObjectsIn(extendedArea, tr);

            Rectangle enclosingRect = GetEnclosingRect(extendedArea.BoundingBox);

            BitmapInfo renderLayer = PrepareBitmap(RENDER_LAYER, enclosingRect.Width, enclosingRect.Height);
            TransformParams renderTr = new TransformParams(-enclosingRect.X, -enclosingRect.Y, mScaleFactor);
            
            BoundingArea extendedAreaTr = extendedArea.Clone();
            extendedAreaTr.Transform(new TransformParams(-enclosingRect.X, -enclosingRect.Y, 1));

            for (int i = drawableObjects.Length - 1; i >= 0; i--)
            {
                if (outdatedObjects.Contains(drawableObjects[i]))
                {
                    drawableObjects[i].Draw(renderLayer.Graphics, renderTr);
                }
                else
                {
                    drawableObjects[i].Draw(renderLayer.Graphics, renderTr, extendedAreaTr);
                }
            }
            BitmapInfo mainLayer = mBmpCache[MAIN_LAYER];
            Graphics canvasGfx = Graphics.FromHwnd(picBoxCanvas.Handle);
            foreach (RectangleF rect in extendedArea.Rectangles)
            {
                Rectangle viewArea = GetEnclosingRect(rect);
                viewArea.X -= enclosingRect.X;
                viewArea.Y -= enclosingRect.Y;
                viewArea.Intersect(new Rectangle(0, 0, enclosingRect.Width, enclosingRect.Height));
                EditableBitmap view = renderLayer.EditableBitmap.CreateView(viewArea);
                mainLayer.Graphics.DrawImageUnscaled(view.Bitmap, viewArea.X + enclosingRect.X, viewArea.Y + enclosingRect.Y);
                // clipping to visible area?!?
                canvasGfx.DrawImageUnscaled(view.Bitmap, viewArea.X + enclosingRect.X, viewArea.Y + enclosingRect.Y);
                //viewOnView.Dispose();
                view.Dispose();
            }
            canvasGfx.Dispose();

            TimeSpan drawTime = DateTime.Now - startTime;
            mDrawTime += drawTime;
            mDrawCount++;

            FpsInfo.Text = string.Format("{0:0.00} ms / draw", (double)mDrawTime.TotalMilliseconds / (double)mDrawCount);
            mDrawInfo.Add(drawTime.TotalMilliseconds);
            FpsInfo.Refresh();
        }

        public delegate void DrawableObjectEventHandler(object sender, DrawableObjectEventArgs args);

        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectClick
            = null;
        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectDoubleClick
            = null;

        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectMouseEnter
            = null;
        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectMouseLeave
            = null;

        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectMouseDown
            = null;
        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectMouseUp
            = null;
        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectMouseMove
            = null;
        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectMouseHover
            = null;

        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectContextMenuStripRequest
            = null;
        [Category("DrawableObject Event")]
        public event DrawableObjectEventHandler DrawableObjectToolTipRequest
            = null;

        [Category("Canvas Event")]
        public event MouseEventHandler CanvasClick
            = null;
        [Category("Canvas Event")]
        public event MouseEventHandler CanvasDoubleClick
            = null;

        //[Category("Canvas Event")]
        //public event (Mouse?)EventHandler CanvasMouseEnter
        //    = null;
        //[Category("Canvas Event")]
        //public event EventHandler CanvasMouseLeave
        //    = null;

        [Category("Canvas Event")]
        public event MouseEventHandler CanvasMouseDown
            = null;
        [Category("Canvas Event")]
        public event MouseEventHandler CanvasMouseUp
            = null;
        [Category("Canvas Event")]
        public event MouseEventHandler CanvasMouseMove
            = null;
        [Category("Canvas Event")]
        public event EventHandler CanvasMouseHover
            = null;

        private void DrawableObjectViewer_MouseMove(object sender, MouseEventArgs args)
        {
            const int WM_ACTIVATE = 0x0006;
            if (mDrawableObject != null)
            {
                TransformParams tr = new TransformParams(0, 0, mScaleFactor);
                float[] distArray = null;
                IDrawableObject[] newTargetObjs = mDrawableObject.GetObjectsAt(args.X, args.Y, tr, ref distArray);
                Set<IDrawableObject> newTargetObjSet = new Set<IDrawableObject>(newTargetObjs);
                ArrayList<IDrawableObject> exitObjs = new ArrayList<IDrawableObject>();
                ArrayList<IDrawableObject> enterObjs = new ArrayList<IDrawableObject>();
                if (DrawableObjectMouseLeave != null || DrawableObjectToolTipRequest != null)
                {                    
                    foreach (IDrawableObject obj in mTargetObjs)
                    {
                        if (!newTargetObjSet.Contains(obj))
                        {
                            exitObjs.Add(obj);
                        }
                    }
                }
                if (DrawableObjectMouseEnter != null || DrawableObjectToolTipRequest != null)
                {                   
                    foreach (IDrawableObject obj in newTargetObjs)
                    {
                        if (!mTargetObjSet.Contains(obj))
                        {
                            enterObjs.Add(obj);
                        }
                    }
                }
                mTargetObjSet = newTargetObjSet;
                mTargetObjs.Clear();
                mTargetObjs.AddRange(newTargetObjs);
                if (DrawableObjectMouseLeave != null && exitObjs.Count > 0)
                {
                    DrawableObjectMouseLeave(this, new DrawableObjectEventArgs(args, exitObjs));
                }
                if (DrawableObjectMouseEnter != null && enterObjs.Count > 0)
                {
                    DrawableObjectMouseEnter(this, new DrawableObjectEventArgs(args, enterObjs));
                }
                if (DrawableObjectToolTipRequest != null && (enterObjs.Count > 0 || exitObjs.Count > 0))
                {
                    if (newTargetObjs.Length > 0)
                    {
                        mDrawableObjectTip.SetToolTip(picBoxCanvas, null);
                        DrawableObjectEventArgs eventArgs = new DrawableObjectEventArgs(args, new ArrayList<IDrawableObject>(newTargetObjs));
                        DrawableObjectToolTipRequest(this, eventArgs);
                        mDrawableObjectTip.SetToolTip(picBoxCanvas, eventArgs.ToolTipText);
                        SendMessage(picBoxCanvas.Handle, WM_ACTIVATE, 0, IntPtr.Zero); // *** I'm not sure why this is required but it is :)
                    }
                    else
                    {
                        mDrawableObjectTip.SetToolTip(picBoxCanvas, null);
                    }
                }
            }
            if (mTargetObjs.Count == 0)
            {
                if (CanvasMouseMove != null) 
                { 
                    CanvasMouseMove(this, args); 
                }
            }
            else 
            {
                if (DrawableObjectMouseMove != null) 
                { 
                    DrawableObjectMouseMove(this, new DrawableObjectEventArgs(args, mTargetObjs)); 
                }
            }
        }

        private void DrawableObjectViewer_MouseHover(object sender, EventArgs args)
        {
            if (mTargetObjs.Count > 0)
            {
                if (DrawableObjectMouseHover != null)
                {
                    DrawableObjectMouseHover(this, new DrawableObjectEventArgs(mTargetObjs));
                }
            }
            else
            {
                if (CanvasMouseHover != null)
                {
                    CanvasMouseHover(this, EventArgs.Empty);
                }
            }
        }

        private void DrawableObjectViewer_MouseDown(object sender, MouseEventArgs args)
        {
            Focus();
            if (mTargetObjs.Count > 0)
            {
                if (DrawableObjectMouseDown != null)
                {
                    DrawableObjectMouseDown(this, new DrawableObjectEventArgs(args, mTargetObjs));
                }
            }
            else
            {
                if (CanvasMouseDown != null)
                {
                    CanvasMouseDown(this, args);
                }
            }
        }

        private void DrawableObjectViewer_MouseUp(object sender, MouseEventArgs args)
        {
            if (mTargetObjs.Count > 0)
            {
                if (DrawableObjectMouseUp != null)
                {
                    DrawableObjectMouseUp(this, new DrawableObjectEventArgs(args, mTargetObjs));
                }
                if ((args.Button & MouseButtons.Right) == MouseButtons.Right && DrawableObjectContextMenuStripRequest != null)
                {
                    DrawableObjectEventArgs eventArgs = new DrawableObjectEventArgs(args, mTargetObjs);
                    DrawableObjectContextMenuStripRequest(this, eventArgs);
                    if (eventArgs.ContextMenuStrip != null)
                    {
                        eventArgs.ContextMenuStrip.Show(picBoxCanvas, args.X, args.Y);
                    }
                }
            }
            else
            {
                if (CanvasMouseUp != null)
                {
                    CanvasMouseUp(this, args);
                }
                if ((args.Button & MouseButtons.Right) == MouseButtons.Right && mCanvasMenu != null)
                {
                    mCanvasMenu.Show(picBoxCanvas, args.X, args.Y);
                }
            }
        }

        private void DrawableObjectViewer_MouseClick(object sender, MouseEventArgs args)
        {
            if (DrawableObjectClick != null && mTargetObjs.Count > 0)
            {
                DrawableObjectClick(this, new DrawableObjectEventArgs(args, mTargetObjs));
            }
            if (CanvasClick != null && mTargetObjs.Count == 0)
            {
                CanvasClick(this, args);
            }
        }

        private void DrawableObjectViewer_MouseDoubleClick(object sender, MouseEventArgs args)
        {
            if (DrawableObjectDoubleClick != null && mTargetObjs.Count > 0)
            {
                DrawableObjectDoubleClick(this, new DrawableObjectEventArgs(args, mTargetObjs));
            }
            if (CanvasDoubleClick != null && mTargetObjs.Count == 0)
            {
                CanvasDoubleClick(this, args);
            }
        }

        private void DrawableObjectViewer_MouseLeave(object sender, EventArgs args)
        {            
            if (mTargetObjSet.Count > 0)
            {
                ArrayList<IDrawableObject> aux = mTargetObjs;
                mTargetObjSet.Clear();
                mTargetObjs = new ArrayList<IDrawableObject>();
                if (DrawableObjectMouseLeave != null)
                {
                    DrawableObjectMouseLeave(this, new DrawableObjectEventArgs(aux)); 
                }
                mDrawableObjectTip.SetToolTip(picBoxCanvas, null);
            }
        }

        /* .-----------------------------------------------------------------------
           |
           |  Class BitmapInfo
           |
           '-----------------------------------------------------------------------
        */
        private class BitmapInfo : IDisposable
        {
            public EditableBitmap EditableBitmap
                = null;
            public Graphics Graphics
                = null;
            public BitmapInfo(EditableBitmap bmp, Graphics gfx)
            {
                EditableBitmap = bmp;
                Graphics = gfx;
            }
            public Bitmap Bitmap
            {
                get { return EditableBitmap.Bitmap; }
            }
            // *** IDisposable interface implementation ***
            public void Dispose()
            {
                if (Graphics != null) 
                { 
                    Graphics.Dispose(); 
                }
                if (EditableBitmap != null) 
                { 
                    EditableBitmap.Dispose(); 
                }
            }
        }
    }
}