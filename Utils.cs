/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Utils.cs
 *  Desc:    Fundamental LATINO utilities
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Utils
       |
       '-----------------------------------------------------------------------
    */
    public static class Utils
    {
        /* .-----------------------------------------------------------------------
           |
           |  Enum CaseType
           |
           '-----------------------------------------------------------------------
        */
        public enum CaseType
        { 
            abc,
            ABC,
            Abc,
            AbC,
            aBc,
            Other
        }

        public const string DATE_TIME_SIMPLE
            = "yyyy-MM-dd HH:mm:ss K"; // simple date-time format (incl. time zone, if available)
        #region mTimeZones
        private static KeyDat<string, string>[] mTimeZones = new KeyDat<string, string>[] {
            new KeyDat<string, string>("ACDT", "+10:30"),
            new KeyDat<string, string>("ACST", "+09:30"),
            new KeyDat<string, string>("ACT", "+08:00"),
            new KeyDat<string, string>("ADT", "-03:00"),
            new KeyDat<string, string>("AEDT", "+11:00"),
            new KeyDat<string, string>("AEST", "+10:00"),
            new KeyDat<string, string>("AFT", "+04:30"),
            new KeyDat<string, string>("AKDT", "-08:00"),
            new KeyDat<string, string>("AKST", "-09:00"),
            new KeyDat<string, string>("AMST", "+05:00"),
            new KeyDat<string, string>("AMT", "+04:00"),
            new KeyDat<string, string>("ART", "-03:00"),
            new KeyDat<string, string>("AST", "+03:00"),
            new KeyDat<string, string>("AST", "+04:00"),
            new KeyDat<string, string>("AST", "+03:00"),
            new KeyDat<string, string>("AST", "-04:00"),
            new KeyDat<string, string>("AWDT", "+09:00"),
            new KeyDat<string, string>("AWST", "+08:00"),
            new KeyDat<string, string>("AZOST", "-01:00"),
            new KeyDat<string, string>("AZT", "+04:00"),
            new KeyDat<string, string>("BDT", "+08:00"),
            new KeyDat<string, string>("BIOT", "+06:00"),
            new KeyDat<string, string>("BIT", "-12:00"),
            new KeyDat<string, string>("BOT", "-04:00"),
            new KeyDat<string, string>("BRT", "-03:00"),
            new KeyDat<string, string>("BST", "+06:00"),
            new KeyDat<string, string>("BST", "+01:00"),
            new KeyDat<string, string>("BTT", "+06:00"),
            new KeyDat<string, string>("CAT", "+02:00"),
            new KeyDat<string, string>("CCT", "+06:30"),
            new KeyDat<string, string>("CDT", "-05:00"),
            new KeyDat<string, string>("CEDT", "+02:00"),
            new KeyDat<string, string>("CEST", "+02:00"),
            new KeyDat<string, string>("CET", "+01:00"),
            new KeyDat<string, string>("CHAST", "+12:45"),
            new KeyDat<string, string>("CIST", "-08:00"),
            new KeyDat<string, string>("CKT", "-10:00"),
            new KeyDat<string, string>("CLST", "-03:00"),
            new KeyDat<string, string>("CLT", "-04:00"),
            new KeyDat<string, string>("COST", "-04:00"),
            new KeyDat<string, string>("COT", "-05:00"),
            new KeyDat<string, string>("CST", "-06:00"),
            new KeyDat<string, string>("CST", "+08:00"),
            new KeyDat<string, string>("CVT", "-01:00"),
            new KeyDat<string, string>("CXT", "+07:00"),
            new KeyDat<string, string>("ChST", "+10:00"),
            new KeyDat<string, string>("DFT", "+01:00"),
            new KeyDat<string, string>("EAST", "-06:00"),
            new KeyDat<string, string>("EAT", "+03:00"),
            new KeyDat<string, string>("ECT", "-04:00"),
            new KeyDat<string, string>("ECT", "-05:00"),
            new KeyDat<string, string>("EDT", "-04:00"),
            new KeyDat<string, string>("EEDT", "+03:00"),
            new KeyDat<string, string>("EEST", "+03:00"),
            new KeyDat<string, string>("EET", "+02:00"),
            new KeyDat<string, string>("EST", "-05:00"),
            new KeyDat<string, string>("FJT", "+12:00"),
            new KeyDat<string, string>("FKST", "-03:00"),
            new KeyDat<string, string>("FKT", "-04:00"),
            new KeyDat<string, string>("GALT", "-06:00"),
            new KeyDat<string, string>("GET", "+04:00"),
            new KeyDat<string, string>("GFT", "-03:00"),
            new KeyDat<string, string>("GILT", "+12:00"),
            new KeyDat<string, string>("GIT", "-09:00"),
            new KeyDat<string, string>("GMT", "+00:00"),
            new KeyDat<string, string>("GST", "-02:00"),
            new KeyDat<string, string>("GYT", "-04:00"),
            new KeyDat<string, string>("HADT", "-09:00"),
            new KeyDat<string, string>("HAST", "-10:00"),
            new KeyDat<string, string>("HKT", "+08:00"),
            new KeyDat<string, string>("HMT", "+05:00"),
            new KeyDat<string, string>("HST", "-10:00"),
            new KeyDat<string, string>("IRKT", "+08:00"),
            new KeyDat<string, string>("IRST", "+03:30"),
            new KeyDat<string, string>("IST", "+05:30"),
            new KeyDat<string, string>("IST", "+01:00"),
            new KeyDat<string, string>("IST", "+02:00"),
            new KeyDat<string, string>("JST", "+09:00"),
            new KeyDat<string, string>("KRAT", "+07:00"),
            new KeyDat<string, string>("KST", "+09:00"),
            new KeyDat<string, string>("LHST", "+10:30"),
            new KeyDat<string, string>("LINT", "+14:00"),
            new KeyDat<string, string>("MAGT", "+11:00"),
            new KeyDat<string, string>("MDT", "-06:00"),
            new KeyDat<string, string>("MIT", "-09:30"),
            new KeyDat<string, string>("MSD", "+04:00"),
            new KeyDat<string, string>("MSK", "+03:00"),
            new KeyDat<string, string>("MST", "+08:00"),
            new KeyDat<string, string>("MST", "-07:00"),
            new KeyDat<string, string>("MST", "+06:30"),
            new KeyDat<string, string>("MUT", "+04:00"),
            new KeyDat<string, string>("NDT", "-02:30"),
            new KeyDat<string, string>("NFT", "+11:30"),
            new KeyDat<string, string>("NPT", "+05:45"),
            new KeyDat<string, string>("NST", "-03:30"),
            new KeyDat<string, string>("NT", "-03:30"),
            new KeyDat<string, string>("OMST", "+06:00"),
            new KeyDat<string, string>("PDT", "-07:00"),
            new KeyDat<string, string>("PETT", "+12:00"),
            new KeyDat<string, string>("PHOT", "+13:00"),
            new KeyDat<string, string>("PKT", "+05:00"),
            new KeyDat<string, string>("PST", "-08:00"),
            new KeyDat<string, string>("PST", "+08:00"),
            new KeyDat<string, string>("RET", "+04:00"),
            new KeyDat<string, string>("SAMT", "+04:00"),
            new KeyDat<string, string>("SAST", "+02:00"),
            new KeyDat<string, string>("SBT", "+11:00"),
            new KeyDat<string, string>("SCT", "+04:00"),
            new KeyDat<string, string>("SLT", "+05:30"),
            new KeyDat<string, string>("SST", "-11:00"),
            new KeyDat<string, string>("SST", "+08:00"),
            new KeyDat<string, string>("TAHT", "-10:00"),
            new KeyDat<string, string>("THA", "+07:00"),
            new KeyDat<string, string>("UTC", "+00:00"),
            new KeyDat<string, string>("UYST", "-02:00"),
            new KeyDat<string, string>("UYT", "-03:00"),
            new KeyDat<string, string>("VET", "-04:30"),
            new KeyDat<string, string>("VLAT", "+10:00"),
            new KeyDat<string, string>("WAT", "+01:00"),
            new KeyDat<string, string>("WEDT", "+01:00"),
            new KeyDat<string, string>("WEST", "+01:00"),
            new KeyDat<string, string>("WET", "+00:00"),
            new KeyDat<string, string>("YAKT", "+09:00"),
            new KeyDat<string, string>("YEKT", "+05:00") };
        #endregion

        // *** Core utilities ***

        [Conditional("THROW_EXCEPTIONS")]
        public static void ThrowException(Exception exception)
        {
            if (exception != null) { throw exception; }
        }

        public static object Clone(object obj, bool deepClone)
        {
            if (deepClone && obj is IDeeplyCloneable)
            {
                return ((IDeeplyCloneable)obj).DeepClone();
            }
            else if (obj is ICloneable)
            {
                return ((ICloneable)obj).Clone();
            }
            else
            {
                return obj;
            }
        }

        public static bool ObjectEquals(object obj1, object obj2, bool deepCmp)
        {
            if (obj1 == null && obj2 == null) { return true; }
            else if (obj1 == null || obj2 == null) { return false; }
            else if (!obj1.GetType().Equals(obj2.GetType())) { return false; }
            else if (deepCmp && obj1 is IContentEquatable)
            {
                return ((IContentEquatable)obj1).ContentEquals(obj2);
            }
            else
            {
                return obj1.Equals(obj2);
            }
        }

        public static int GetHashCode(object obj)
        {
            ThrowException(obj == null ? new ArgumentNullException("obj") : null);
            if (obj is ISerializable)
            {
                BinarySerializer memSer = new BinarySerializer();
                ((ISerializable)obj).Save(memSer); // throws serialization-related exceptions   
                byte[] buffer = ((MemoryStream)memSer.Stream).GetBuffer();
                MD5CryptoServiceProvider hashAlgo = new MD5CryptoServiceProvider();
                Guid md5Hash = new Guid(hashAlgo.ComputeHash(buffer, 0, (int)memSer.Stream.Position));
                return md5Hash.GetHashCode();
            }
            else
            {
                return obj.GetHashCode();
            }
        }

        public static object ChangeType(object obj, Type newType, IFormatProvider fmtProvider)
        {
            ThrowException(newType == null ? new ArgumentNullException("newType") : null);
            if (newType.IsAssignableFrom(obj.GetType()))
            {
                return obj;
            }
            else
            {
                return Convert.ChangeType(obj, newType, fmtProvider); // throws InvalidCastException, FormatException, OverflowException
            }
        }

        // *** String utilities ***

        public static Guid GetHashCode128(this string str)
        {
            ThrowException(str == null ? new ArgumentNullException("str") : null);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        public static ulong GetHashCode64(this string str)
        {
            ThrowException(str == null ? new ArgumentNullException("str") : null);
            byte[] hashCode128 = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(str));
            ulong part1 = (ulong)BitConverter.ToInt64(hashCode128, 0);
            ulong part2 = (ulong)BitConverter.ToInt64(hashCode128, 8);
            return part1 ^ part2;
        }

        public static string Truncate(this string str, int len)
        {
            ThrowException(len < 0 ? new ArgumentOutOfRangeException("len") : null);
            return (str != null && str.Length > len) ? str.Substring(0, len) : str;
        }

        public static string ToOneLine(this string str)
        {
            return ToOneLine(str, /*compact=*/false);
        }

        public static string ToOneLine(this string str, bool compact)
        {
            if (str == null) { return null; }
            str = str.Replace("\r", "").Replace('\n', ' ').Trim();
            if (compact)
            {
                //str = Regex.Replace(str, @"\s\s+", " ");
                str = Regex.Replace(str, @"\s+", " ");
            }
            return str;
        }

        public static CaseType GetCaseType(this string str) 
        {
            ThrowException(str == null ? new ArgumentNullException("str") : null);
            str = str.Trim();
            int numUpper = 0;
            int numLower = 0;
            int numOther = 0;
            foreach (char ch in str)
            {
                if (char.IsUpper(ch)) { numUpper++; }
                else if (char.IsLower(ch)) { numLower++; }
                else { numOther++; }
            }
            int numLetter = numUpper + numLower;
            if (numLetter == 0) { return CaseType.Other; }
            if (numLetter == 1)
            {
                if (char.IsUpper(str[0])) { return CaseType.ABC; }
                else if (char.IsLower(str[0])) { return CaseType.abc; }
                else { return CaseType.Other; }
            }
            if (char.IsUpper(str[0])) // ABC, Abc, AbC
            {
                if (numUpper + numOther == str.Length) { return CaseType.ABC; }
                else if (numUpper == 1) { return CaseType.Abc; }
                else { return CaseType.AbC; }
            }
            else // abc, aBc
            {
                if (numLower + numOther == str.Length) { return CaseType.abc; }
                else { return CaseType.aBc; }
            }
        }

        // *** IO utilities ***

        public static bool VerifyFileNameCreate(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                FileAttributes attributes = fileInfo.Attributes; 
                if ((int)attributes == -1) { attributes = (FileAttributes)0; }
                return (attributes & FileAttributes.Directory) != FileAttributes.Directory && (attributes & FileAttributes.Offline) != FileAttributes.Offline &&
                    (attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly /*&& (attributes & FileAttributes.System) != FileAttributes.System*/;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifyFileNameOpen(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                FileAttributes attributes = fileInfo.Attributes;
                if ((int)attributes == -1) { return false; }
                return (attributes & FileAttributes.Directory) != FileAttributes.Directory && (attributes & FileAttributes.Offline) != FileAttributes.Offline;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifyFolderName(string folderName, bool mustExist)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folderName);
                return mustExist ? dirInfo.Exists : !VerifyFileNameOpen(folderName);
            }
            catch
            {
                return false;
            }
        }

        public static byte[] ReadAllBytes(this Stream stream, int sizeLimit)
        {
            ThrowException(stream == null ? new ArgumentNullException("stream") : null);
            byte[] buffer = new byte[32768];
            int size = 0;
            using (MemoryStream memStream = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length); // throws IOException, ObjectDisposedException, NotSupportedException
                    if (read <= 0) 
                    { 
                        return memStream.ToArray(); 
                    }
                    else if (sizeLimit > 0)
                    {
                        size += read;
                        if (size > sizeLimit) { return null; }
                    }
                    memStream.Write(buffer, 0, read);
                }
            }
        }

        public static Encoding GetUnicodeSignature(string fileName)
        {
            ThrowException(!VerifyFileNameOpen(fileName) ? new ArgumentValueException("fileName") : null);
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bom = new byte[4]; // get the byte-order mark, if there is one
            file.Read(bom, 0, 4);
            file.Close();
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) { return Encoding.UTF8; }
            else if (bom[0] == 0xff && bom[1] == 0xfe) { return Encoding.Unicode; }
            else if (bom[0] == 0xfe && bom[1] == 0xff) { return Encoding.BigEndianUnicode; }
            else if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) { return Encoding.UTF32; }
            else if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) { return Encoding.GetEncoding("UTF-32BE"); }
            else { return null; }
        }

        public static string GetManifestResourceString(Type type, string resName)
        {
            ThrowException(type == null ? new ArgumentNullException("type") : null);
            ThrowException(resName == null ? new ArgumentNullException("resName") : null);
            foreach (string res in type.Assembly.GetManifestResourceNames())
            {
                if (res.EndsWith(resName))
                {
                    return new StreamReader(type.Assembly.GetManifestResourceStream(res)).ReadToEnd();
                }
            }
            return null;
        }

        public static Stream GetManifestResourceStream(Type type, string resName)
        {
            ThrowException(type == null ? new ArgumentNullException("type") : null);
            ThrowException(resName == null ? new ArgumentNullException("resName") : null);
            foreach (string res in type.Assembly.GetManifestResourceNames())
            {
                if (res.EndsWith(resName))
                {
                    return type.Assembly.GetManifestResourceStream(res);
                }
            }
            return null;
        }

        public static string GetConfigValue(string key)
        {
            return GetConfigValue(key, /*defaultValue=*/null); // throws ArgumentNullException, ConfigurationErrorsException
        }

        public static string GetConfigValue(string key, string defaultValue)
        {
            ThrowException(key == null ? new ArgumentNullException("key") : null);
            string value = ConfigurationManager.AppSettings[key]; // throws ConfigurationErrorsException 
            if (value == null) { value = defaultValue; }
            return value;
        }

        public static T GetConfigValue<T>(string key, string defaultValue)
        {
            string val = GetConfigValue(key, defaultValue); // throws ArgumentNullException, ConfigurationErrorsException
            // the following statements throw ArgumentNullException, FormatException, OverflowException, InvalidCastException
            if (typeof(T) == typeof(int)) { return (T)(object)Convert.ToInt32(val); }
            else if (typeof(T) == typeof(long)) { return (T)(object)Convert.ToInt64(val); }
            else if (typeof(T) == typeof(float)) { return (T)(object)Convert.ToSingle(val); }
            else if (typeof(T) == typeof(double)) { return (T)(object)Convert.ToDouble(val); }
            else if (typeof(T) == typeof(bool)) { return (T)(object)(val != null && new ArrayList<string>(new string[] { "yes", "on", "true", "y", "1" }).Contains(val.ToLower())); }
            else if (typeof(T) == typeof(TimeSpan)) { return (T)(object)TimeSpan.Parse(val); }
            else if (typeof(T) == typeof(DateTime)) { return (T)(object)DateTime.Parse(val); }
            else if (typeof(T).IsEnum) { return (T)Enum.Parse(typeof(T), val, /*ignoreCase=*/true); }
            else return (T)(object)val;
        }

        public static T GetConfigValue<T>(string key)
        {
            return GetConfigValue<T>(key, /*defaultValue=*/null); // throws ArgumentNullException, ConfigurationErrorsException, type conversion exceptions
        }

        // *** Dictionary utilities ***

        public static void SaveDictionary<KeyT, ValT>(this Dictionary<KeyT, ValT> dict, BinarySerializer writer)
        {
            ThrowException(dict == null ? new ArgumentNullException("dict") : null);
            ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions   
            writer.WriteInt(dict.Count);
            foreach (KeyValuePair<KeyT, ValT> item in dict)
            {
                writer.WriteValueOrObject<KeyT>(item.Key);
                writer.WriteValueOrObject<ValT>(item.Value);
            }
        }

        public static Dictionary<KeyT, ValT> LoadDictionary<KeyT, ValT>(this BinarySerializer reader)
        {
            return LoadDictionary<KeyT, ValT>(reader, /*comparer=*/null); // throws ArgumentNullException, serialization-related exceptions
        }

        public static Dictionary<KeyT, ValT> LoadDictionary<KeyT, ValT>(this BinarySerializer reader, IEqualityComparer<KeyT> comparer)
        {
            ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            Dictionary<KeyT, ValT> dict = new Dictionary<KeyT, ValT>(comparer);
            // the following statements throw serialization-related exceptions   
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            { 
                KeyT key = reader.ReadValueOrObject<KeyT>();
                ValT dat = reader.ReadValueOrObject<ValT>();
                dict.Add(key, dat);
            }
            return dict;
        }

        // *** XML utilities ***

        public static string ReplaceSurrogates(this string text)
        {
            return ReplaceSurrogates(text, '\uFFFD'); 
        }

        public static string ReplaceSurrogates(this string text, char replacement)
        {          
            if (text == null) { return null; }
            char[] buffer = new char[text.Length];
            int i = 0;
            foreach (char ch in text)
            {
                if (char.IsSurrogate(ch)) { buffer[i] = replacement; }
                else { buffer[i] = ch; }
                i++;
            }
            return new string(buffer);
        }

        public static string XmlReadValue(this XmlReader reader, string attrName)
        {
            ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            ThrowException(attrName == null ? new ArgumentNullException("attrName") : null);
            if (reader.IsEmptyElement) { return ""; }
            string text = "";
            while (reader.Read() && reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.CDATA && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == attrName)) { }
            if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.CDATA)
            {
                text = reader.Value;
                XmlSkip(reader, attrName);
            }
            return text;
        }

        public static void XmlSkip(this XmlReader reader, string attrName)
        {
            ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            ThrowException(attrName == null ? new ArgumentNullException("attrName") : null);
            if (reader.IsEmptyElement) { return; }
            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == attrName)) { }
        }

        // *** DB utilities ***

        public static object RetryOnDeadlock(Func<object> action, int numRetries, int sleep, Logger logger)
        {
            Utils.ThrowException(action == null ? new ArgumentNullException("action") : null);
            Utils.ThrowException(sleep < 0 ? new ArgumentOutOfRangeException("sleep") : null);
            Utils.ThrowException(numRetries < 0 ? new ArgumentOutOfRangeException("numRetries") : null);
            for (int i = 0; (numRetries == 0) ? true : (i <= numRetries); i += (numRetries > 0) ? 1 : 0)
            {
                try
                {
                    return action();
                }
                catch (SqlException e)
                {
                    Console.Write("X");
                    if (!e.Message.Contains("deadlock") || (i == numRetries && i != 0)) { throw e; }                    
                    if (logger != null) { logger.Warn("RetryOnDeadlock", e); }
                }
                Thread.Sleep(sleep);
            }
            return null; // unreachable code
        }

        public static object RetryOnDeadlock(Func<object> action, Logger logger)
        {
            return RetryOnDeadlock(action, /*numRetries=*/0, /*sleep=*/0, logger); // throws ArgumentNullException, ArgumentOutOfRangeException
        }

        public static object RetryOnDeadlock(Func<object> action)
        {
            return RetryOnDeadlock(action, /*numRetries=*/0, /*sleep=*/0, /*logger=*/null); // throws ArgumentNullException, ArgumentOutOfRangeException
        }

        public static int ExecuteNonQueryRetryOnDeadlock(this SqlCommand sqlCmd, int numRetries, int sleep, Logger logger)
        { 
            Utils.ThrowException(sqlCmd == null ? new ArgumentNullException("sqlCmd") : null);
            return (int)RetryOnDeadlock(delegate() { return sqlCmd.ExecuteNonQuery(); }, numRetries, sleep, logger); // throws ArgumentOutOfRangeException, SqlException
        }

        public static int ExecuteNonQueryRetryOnDeadlock(this SqlCommand sqlCmd, Logger logger)
        {
            return ExecuteNonQueryRetryOnDeadlock(sqlCmd, /*numRetries=*/0, /*sleep=*/0, logger); // throws ArgumentNullException, ArgumentOutOfRangeException, SqlException
        }

        public static int ExecuteNonQueryRetryOnDeadlock(this SqlCommand sqlCmd)
        {
            return ExecuteNonQueryRetryOnDeadlock(sqlCmd, /*numRetries=*/0, /*sleep=*/0, /*logger=*/null); // throws ArgumentNullException, ArgumentOutOfRangeException, SqlException
        }

        public static object ExecuteScalarRetryOnDeadlock(this SqlCommand sqlCmd, int numRetries, int sleep, Logger logger)
        {
            Utils.ThrowException(sqlCmd == null ? new ArgumentNullException("sqlCmd") : null);
            return RetryOnDeadlock(delegate() { return sqlCmd.ExecuteScalar(); }, numRetries, sleep, logger); // throws ArgumentOutOfRangeException, SqlException
        }

        public static object ExecuteScalarRetryOnDeadlock(this SqlCommand sqlCmd, Logger logger)
        {
            return ExecuteScalarRetryOnDeadlock(sqlCmd, /*numRetries=*/0, /*sleep=*/0, logger); // throws ArgumentNullException, ArgumentOutOfRangeException, SqlException
        }

        public static object ExecuteScalarRetryOnDeadlock(this SqlCommand sqlCmd)
        {
            return ExecuteScalarRetryOnDeadlock(sqlCmd, /*numRetries=*/0, /*sleep=*/0, /*logger=*/null); // throws ArgumentNullException, ArgumentOutOfRangeException, SqlException
        }

        public static void WriteToServerRetryOnDeadlock(this SqlBulkCopy bulkCopy, DataTable table, int numRetries, int sleep, Logger logger)
        {
            Utils.ThrowException(bulkCopy == null ? new ArgumentNullException("bulkCopy") : null);
            Utils.ThrowException(table == null ? new ArgumentNullException("table") : null);
            RetryOnDeadlock(delegate() { bulkCopy.WriteToServer(table); return null; }, numRetries, sleep, logger); // throws ArgumentOutOfRangeException, SqlException
        }

        public static void WriteToServerRetryOnDeadlock(this SqlBulkCopy bulkCopy, DataTable table, Logger logger)
        {
            WriteToServerRetryOnDeadlock(bulkCopy, table, /*numRetries=*/0, /*sleep=*/0, logger); // throws ArgumentNullException, ArgumentOutOfRangeException, SqlException
        }

        public static void WriteToServerRetryOnDeadlock(this SqlBulkCopy bulkCopy, DataTable table)
        {
            WriteToServerRetryOnDeadlock(bulkCopy, table, /*numRetries=*/0, /*sleep=*/0, /*logger=*/null); // throws ArgumentNullException, ArgumentOutOfRangeException, SqlException
        }

        public static T GetValue<T>(this SqlDataReader reader, string colName)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            object obj = reader.GetValue(reader.GetOrdinal(colName)); // throws IndexOutOfRangeException
            if (obj is DBNull) { return default(T); }
            return (T)obj; // throws InvalidCastException
        }

        public static bool HasValue(this SqlDataReader reader, string colName)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            object obj = reader.GetValue(reader.GetOrdinal(colName)); // throws IndexOutOfRangeException
            return !(obj is DBNull);
        }

        public static void AssignParams(this SqlCommand command, params object[] args)
        {
            Utils.ThrowException(command == null ? new ArgumentNullException("command") : null);
            Utils.ThrowException(args == null ? new ArgumentNullException("command") : null);
            for (int i = 0; i < args.Length; i += 2)
            {
                Utils.ThrowException(!(args[i] is string) ? new ArgumentValueException("args") : null);
                object val = args[i + 1];
                SqlParameter param = new SqlParameter((string)args[i], val == null ? DBNull.Value : val);
                command.Parameters.Add(param);
            }
        }

        // *** Other utilities ***

        public static bool IsFiniteNumber(this double val)
        {
            return !double.IsInfinity(val) && !double.IsNaN(val);
        }

        public static bool IsFiniteNumber(this float val)
        {
            return !float.IsInfinity(val) && !float.IsNaN(val);
        }

        public static string NormalizeDateTimeStr(string dateTimeStr) // *** rmv
        {
            ThrowException(dateTimeStr == null ? new ArgumentNullException("dateTimeStr") : null);
            dateTimeStr = dateTimeStr.Trim();
            foreach (KeyDat<string, string> timeZone in mTimeZones)
            {
                if (dateTimeStr.EndsWith(" " + timeZone.Key)) { dateTimeStr = dateTimeStr.Replace(timeZone.Key, timeZone.Dat); break; }
            }
            DateTime dt;
            try
            {
                dt = DateTime.Parse(dateTimeStr);
            }
            catch
            {
                return null;
            }
            return dt.ToString(DATE_TIME_SIMPLE);
        }

        public static void SetDefaultCulture(CultureInfo culture)
        {
            ThrowException(culture == null ? new ArgumentNullException("culture") : null);
            Type type = typeof(CultureInfo);
            try
            {
                type.InvokeMember("s_userDefaultCulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] { culture });
                type.InvokeMember("s_userDefaultUICulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] { culture });
            }
            catch { }
            try
            {
                type.InvokeMember("m_userDefaultCulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] { culture });
                type.InvokeMember("m_userDefaultUICulture",
                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    culture,
                    new object[] { culture });
            }
            catch { }
        }

        // *** Delegates ***

        public delegate T UnaryOperatorDelegate<T>(T val);
        public delegate T BinaryOperatorDelegate<T>(T a, T b);
    }
}