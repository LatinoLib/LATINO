/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Utils.cs
 *  Version:       1.0
 *  Desc:		   Fundamental LATINO utilities
 *  Author:        Miha Grcar
 *  Created on:    Nov-2007
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Static class Utils
       |
       '-----------------------------------------------------------------------
    */
    public static class Utils
    {
        private static bool m_verbose
            = true;

        public static bool VerboseEnabled
        {
            get { return m_verbose; }
            set { m_verbose = value; }
        }

        public static void Verbose(string format, params object[] args)
        {
            if (m_verbose) { Console.Write(String.Format("{0}", format), args); } // throws ArgumentNullException, FormatException
        }

        public static void VerboseLine(string format, params object[] args)
        {
            if (m_verbose) { Console.WriteLine(String.Format("{0}", format), args); } // throws ArgumentNullException, FormatException
        }

        [Conditional("THROW_EXCEPTIONS")]
        public static void ThrowException(Exception exception)
        {
            if (exception != null) { throw exception; }
        }

        public static bool IsFiniteNumber(double val)
        {
            return !double.IsInfinity(val) && !double.IsNaN(val);
        }

        public static bool IsFiniteNumber(float val)
        {
            return !double.IsInfinity(val) && !double.IsNaN(val);
        }

        public static bool VerifyFileNameCreate(string file_name)
        {
            try
            {
                FileInfo file_info = new FileInfo(file_name);
                FileAttributes attributes = file_info.Attributes; 
                if ((int)attributes == -1) { attributes = (FileAttributes)0; }
                return (attributes & FileAttributes.Directory) != FileAttributes.Directory && (attributes & FileAttributes.Offline) != FileAttributes.Offline &&
                    (attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly /*&& (attributes & FileAttributes.System) != FileAttributes.System*/;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifyFileNameOpen(string file_name)
        {
            try
            {
                FileInfo file_info = new FileInfo(file_name);
                FileAttributes attributes = file_info.Attributes;
                if ((int)attributes == -1) { return false; }
                return (attributes & FileAttributes.Directory) != FileAttributes.Directory && (attributes & FileAttributes.Offline) != FileAttributes.Offline;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifyPathName(string path_name, bool must_exist)
        {
            try
            {
                DirectoryInfo dir_info = new DirectoryInfo(path_name);
                return must_exist ? dir_info.Exists : true;
            }
            catch
            {
                return false;
            }
        }

        public static object Clone(object obj, bool deep_clone)
        {
            if (deep_clone && obj is IDeeplyCloneable)
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

        public static bool ObjectEquals(object obj_1, object obj_2, bool deep_cmp)
        {
            if (obj_1 == null && obj_2 == null) { return true; }
            else if (obj_1 == null || obj_2 == null) { return false; }
            else if (!obj_1.GetType().Equals(obj_2.GetType())) { return false; }
            else if (deep_cmp && obj_1 is IContentEquatable)
            {
                return ((IContentEquatable)obj_1).ContentEquals(obj_2);
            }
            else
            {
                return obj_1.Equals(obj_2);
            }
        }

        public static int GetHashCode(object obj)
        {
            ThrowException(obj == null ? new ArgumentNullException("obj") : null);
            if (obj is ISerializable)
            {
                BinarySerializer mem_ser = new BinarySerializer();
                ((ISerializable)obj).Save(mem_ser); // throws serialization-related exceptions   
                byte[] buffer = ((MemoryStream)mem_ser.Stream).GetBuffer();
                MD5CryptoServiceProvider hash_algo = new MD5CryptoServiceProvider();
                Guid md5_hash = new Guid(hash_algo.ComputeHash(buffer));
                return md5_hash.GetHashCode();
            }
            else
            {
                return obj.GetHashCode();
            }
        }

        public static void SaveDictionary<KeyT, ValT>(Dictionary<KeyT, ValT> dict, BinarySerializer writer)
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

        public static Dictionary<KeyT, ValT> LoadDictionary<KeyT, ValT>(BinarySerializer reader)
        {
            return LoadDictionary<KeyT, ValT>(reader, /*comparer=*/null); // throws ArgumentNullException, serialization-related exceptions
        }

        public static Dictionary<KeyT, ValT> LoadDictionary<KeyT, ValT>(BinarySerializer reader, IEqualityComparer<KeyT> comparer)
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

        public static object ChangeType(object obj, Type new_type, IFormatProvider fmt_provider)
        {
            ThrowException(new_type == null ? new ArgumentNullException("new_type") : null);            
            if (new_type.IsAssignableFrom(obj.GetType()))
            {              
                return obj;
            }
            else 
            {
                return Convert.ChangeType(obj, new_type, fmt_provider); // throws InvalidCastException, FormatException, OverflowException
            }
        }

        // *** SparseVector<double> template specialization ***

        public static double GetVecLenL2(/*this*/SparseVector<double>.ReadOnly vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = 0;
            ArrayList<double> dat_inner = vec.Inner.InnerDat;
            foreach (double val in dat_inner)
            {
                len += val * val;
            }
            return Math.Sqrt(len);
        }

        public static double GetVecLenL2(/*this*/ SparseVector<double> vec)
        {
            return GetVecLenL2(new SparseVector<double>.ReadOnly(vec)); // throws ArgumentNullException
        }

        public static void NrmVecL2(/*this*/ SparseVector<double> vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = GetVecLenL2(vec);
            Utils.ThrowException(len == 0 ? new InvalidOperationException() : null);
            ArrayList<double> dat_inner = vec.InnerDat;
            for (int i = 0; i < vec.Count; i++)
            {
                vec.SetDirect(i, dat_inner[i] / len);
            }
        }

        public static bool TryNrmVecL2(/*this*/ SparseVector<double> vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = GetVecLenL2(vec);
            if (len == 0) { return false; }
            ArrayList<double> dat_inner = vec.InnerDat;
            for (int i = 0; i < vec.Count; i++)
            {
                vec.SetDirect(i, dat_inner[i] / len);
            }
            return true;
        }

        // *** Delegates ***

        public delegate T UnaryOperatorDelegate<T>(T val);
        public delegate T BinaryOperatorDelegate<T>(T a, T b);
    }
}