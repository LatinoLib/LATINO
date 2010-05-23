/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    FilledDrawnObject.cs
 *  Desc:    Filled drawable object
 *  Created: Mar-2008
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Drawing;

namespace Latino.Visualization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Abstract class FilledDrawnObject
       |
       '-----------------------------------------------------------------------
    */
    public abstract class FilledDrawnObject : DrawnObject
    {
        protected Brush mBrush
            = Brushes.White;
        public Brush Brush
        {
            get { return mBrush; }
            set 
            {
                Utils.ThrowException(value == null ? new ArgumentNullException("Brush") : null);
                mBrush = value; 
            }
        }
    }
}