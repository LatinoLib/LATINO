/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          DrawableObjectEventArgs.cs
 *  Version:       1.0
 *  Desc:		   Drawable object event arguments class
 *  Author:        Miha Grcar
 *  Created on:    Jun-2009
 *  Last modified: Dec-2009
 *  Revision:      Dec-2009
 *
 ***************************************************************************/

using System.Windows.Forms;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |
       |  Class DrawableObjectEventArgs
       |
       '-----------------------------------------------------------------------
    */
    public class DrawableObjectEventArgs : MouseEventArgs
    {
        private ArrayList<IDrawableObject> mDrawableObjects
            = null;
        new public static readonly DrawableObjectEventArgs Empty
            = new DrawableObjectEventArgs(MouseButtons.None, 0, 0, 0, 0);
        private ContextMenuStrip mContextMenu
            = null;
        private string mToolTipText
            = null;

        public DrawableObjectEventArgs(MouseEventArgs mouseArgs) : base(mouseArgs.Button, mouseArgs.Clicks, mouseArgs.X, mouseArgs.Y, mouseArgs.Delta)
        { 
        }

        public DrawableObjectEventArgs(MouseEventArgs mouseArgs, ArrayList<IDrawableObject> drawableObjects) : this(mouseArgs)
        {
            mDrawableObjects = drawableObjects;
        }

        public DrawableObjectEventArgs(MouseButtons buttons, int clicks, int x, int y, int delta) : base(buttons, clicks, x, y, delta)
        {
        }

        public DrawableObjectEventArgs(MouseButtons buttons, int clicks, int x, int y, int delta, ArrayList<IDrawableObject> drawableObjects) : base(buttons, clicks, x, y, delta) 
        {
            mDrawableObjects = drawableObjects;
        }

        public DrawableObjectEventArgs(ArrayList<IDrawableObject> drawableObjects) : this(MouseButtons.None, 0, 0, 0, 0, drawableObjects)
        {
        }

        public ArrayList<IDrawableObject>.ReadOnly DrawableObjects
        {
            get { return mDrawableObjects; }
        }

        public ContextMenuStrip ContextMenuStrip
        {
            get { return mContextMenu; }
            set { mContextMenu = value; }
        }

        public string ToolTipText
        {
            get { return mToolTipText; }
            set { mToolTipText = value; }
        }
    }
}
