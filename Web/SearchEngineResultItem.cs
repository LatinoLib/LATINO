/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    SearchEngineResultItem.cs
 *  Desc:    Search engine result set item
 *  Created: Nov-2006
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Xml;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Struct SearchEngineResultItem
       |
       '-----------------------------------------------------------------------
    */
    public class SearchEngineResultItem : ISerializable, IXmlSerializable
    {
        private string mTitle;
        private string mSnippet;
        private string mUrl;
        private double mRelevance;

        public SearchEngineResultItem(string title, string snippet, string url, double relevance)
        {
            mTitle = title;
            mSnippet = snippet;
            mUrl = url;
            mRelevance = relevance;
        }

        public SearchEngineResultItem(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mTitle = reader.ReadString();
            mSnippet = reader.ReadString();
            mUrl = reader.ReadString();
            mRelevance = reader.ReadDouble();
        }

        public SearchEngineResultItem(XmlReader xmlReader)
        {
            Utils.ThrowException(xmlReader == null ? new ArgumentNullException("xmlReader") : null);
            mTitle = null;
            mSnippet = null;
            mUrl = null;
            mRelevance = -1;
            while (xmlReader.Read()) // throws XmlException
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "Result")
                    {
                        string relevanceStr = xmlReader.GetAttribute("relevance");
                        if (relevanceStr != null) { mRelevance = XmlConvert.ToDouble(relevanceStr); } // throws OverflowException, FormatException
                        mUrl = xmlReader.GetAttribute("url");
                    }
                    else if (xmlReader.Name == "Title")
                    {
                        if (!xmlReader.IsEmptyElement)
                        {
                            xmlReader.Read(); // throws XmlException
                            Utils.ThrowException((xmlReader.NodeType != XmlNodeType.Text && xmlReader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            mTitle = xmlReader.Value;
                        }
                    }
                    else if (xmlReader.Name == "Snippet")
                    {
                        if (!xmlReader.IsEmptyElement)
                        {
                            xmlReader.Read(); // throws XmlException
                            Utils.ThrowException((xmlReader.NodeType != XmlNodeType.Text && xmlReader.NodeType != XmlNodeType.CDATA) ? new XmlFormatException() : null);
                            mSnippet = xmlReader.Value;
                        }
                    }
                }
                else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Result")
                {
                    break;
                }
            }
        }

        public string Title
        {
            get { return mTitle; }
        }

        public string Snippet
        {
            get { return mSnippet; }
        }

        public string Url
        {
            get { return mUrl; }
        }

        internal void SetUrl(string url)
        {
            mUrl = url;
        }

        public double Relevance
        {
            get { return mRelevance; }
        }

        // *** IXmlSerializable interface implementation ***

        public void SaveXml(XmlWriter xmlWriter)
        {
            Utils.ThrowException(xmlWriter == null ? new ArgumentNullException("xmlWriter") : null);
            // the following statements throw InvalidOperationException if the writer is closed
            xmlWriter.WriteStartElement("Result");
            xmlWriter.WriteAttributeString("relevance", XmlConvert.ToString(mRelevance));
            xmlWriter.WriteAttributeString("url", mUrl);
            xmlWriter.WriteStartElement("Title");
            xmlWriter.WriteString(mTitle);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("Snippet");
            xmlWriter.WriteString(mSnippet);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteString(mTitle);
            writer.WriteString(mSnippet);
            writer.WriteString(mUrl);
            writer.WriteDouble(mRelevance);
        }
    }
}