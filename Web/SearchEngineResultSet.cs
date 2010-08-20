/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    SearchEngineResultSet.cs
 *  Desc:    Search engine result set
 *  Created: Nov-2006
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Class SearchEngineResultSet
       |
       '-----------------------------------------------------------------------
    */
    public class SearchEngineResultSet : IEnumerableList<SearchEngineResultItem>, ICloneable, IXmlSerializable, ISerializable
    {
        private ArrayList<SearchEngineResultItem> mItems
            = new ArrayList<SearchEngineResultItem>();

        public SearchEngineResultSet()
        {
        }

        public SearchEngineResultSet(BinarySerializer reader)
        {
            mItems.Load(reader); // throws serialization-related exceptions
        }

        public SearchEngineResultSet(XmlReader xmlReader) : this(xmlReader, int.MaxValue) // throws ArgumentNullException, ArgumentOutOfRangeException, XmlFormatException, XmlException, OverflowException, FormatException
        {
        }

        public SearchEngineResultSet(XmlReader xmlReader, int sizeLimit)
        {
            Utils.ThrowException(xmlReader == null ? new ArgumentNullException("xmlReader") : null);
            Utils.ThrowException(sizeLimit < 0 ? new ArgumentOutOfRangeException("sizeLimit") : null);
            if (sizeLimit != 0)
            {
                while (xmlReader.Read()) // throws XmlException
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ResultSet")
                    {
                        int size = XmlConvert.ToInt32(xmlReader.GetAttribute("size")); // throws ArgumentNullException, OverflowException, FormatException
                        for (int i = 0; i < size; i++)
                        {
                            mItems.Add(new SearchEngineResultItem(xmlReader)); // throws XmlFormatException, XmlException, OverflowException, FormatException
                            if (mItems.Count == sizeLimit) { return; }
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "ResultSet")
                    {
                        break;
                    }
                }
            }
        }

#if PUBLIC_INNER
        public
#else
        internal 
#endif
        ArrayList<SearchEngineResultItem> Inner
        {
            get { return mItems; }
        }

        public string GetXmlStr()
        {
            StringWriter strWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(strWriter);
            xmlWriter.Formatting = Formatting.Indented;
            SaveXml(xmlWriter);
            return strWriter.ToString();
        }

        // *** IEnumerableList<SearchEngineResult> interface implementation ***

        public int Count
        {
            get { return mItems.Count; }
        }

        public SearchEngineResultItem this[int index]
        {
            get
            {
                Utils.ThrowException((index < 0 || index >= mItems.Count) ? new ArgumentOutOfRangeException("index") : null);
                return mItems[index];
            }
        }

        object IEnumerableList.this[int index]
        {
            get { return this[index]; } // throws ArgumentOutOfRangeException
        }

        public IEnumerator<SearchEngineResultItem> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // *** ICloneable interface implementation ***

        public object Clone()
        {
            SearchEngineResultSet clone = new SearchEngineResultSet();
            clone.mItems = mItems.Clone();
            return clone;
        }

        // *** IXmlSerializable interface implementation ***

        public void SaveXml(XmlWriter xmlWriter)
        {
            Utils.ThrowException(xmlWriter == null ? new ArgumentNullException("xmlWriter") : null);
            // the following statements throw InvalidOperationException if the writer is closed
            xmlWriter.WriteStartElement("ResultSet");
            xmlWriter.WriteAttributeString("size", XmlConvert.ToString(Count));
            foreach (SearchEngineResultItem result in mItems)
            {
                result.SaveXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            mItems.Save(writer); // throws serialization-related exceptions
        }
    }
}