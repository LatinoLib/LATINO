/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    CacheRecord.cs
 *  Desc:    Search engine cache item
 *  Created: Mar-2007
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;

namespace Latino.Web
{
    /* .-----------------------------------------------------------------------
       |		
       |  Class CacheRecord
       |
       '-----------------------------------------------------------------------
    */
    internal class CacheRecord : ISerializable
    {
        public long TotalHits;
        public int ActualSize;
        public string ResultSetXml;
        public DateTime TimeStamp;

        public CacheRecord()
        {
        }

        public CacheRecord(BinarySerializer reader)
        {
            Load(reader);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            writer.WriteLong(TotalHits);
            writer.WriteInt(ActualSize);
            writer.WriteString(ResultSetXml);
            writer.WriteDouble(TimeStamp.ToOADate());
        }

        public void Load(BinarySerializer reader)
        {
            TotalHits = reader.ReadLong();
            ActualSize = reader.ReadInt();
            ResultSetXml = reader.ReadString();
            TimeStamp = DateTime.FromOADate(reader.ReadDouble());
        }
    }
}