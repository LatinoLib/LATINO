/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          FilledDrawnObject.cs
 *  Version:       1.0
 *  Desc:		   Filled drawable object
 *  Author:        Miha Grcar
 *  Created on:    Mar-2008
 *  Last modified: May-2009
 *  Revision:      May-2009
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