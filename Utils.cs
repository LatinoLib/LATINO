/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Utils.cs
 *  Desc:    Fundamental LATINO utilities
 *  Created: Nov-2007
 *
 *  Authors: Miha Grcar
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
       |  Class Utils
       |
       '-----------------------------------------------------------------------
    */
    public static class Utils
    {
        private static bool mVerbose
            = true;
        private static VerboseDelegate mVerboseProc
            = new VerboseDelegate(DefaultVerbose);
        private static VerboseDelegate mVerboseLineProc
            = new VerboseDelegate(DefaultVerboseLine);
        private static VerboseProgressDelegate mVerboseProgressProc
            = new VerboseProgressDelegate(DefaultVerboseProgress);

        public delegate void VerboseDelegate(string format, params object[] args);
        public delegate void VerboseProgressDelegate(string format, int step, int numSteps);

        private static void DefaultVerbose(string format, params object[] args)
        {
            Console.Write(format, args); // throws ArgumentNullException, FormatException
        }

        private static void DefaultVerboseLine(string format, params object[] args)
        {
            Console.WriteLine(format, args); // throws ArgumentNullException, FormatException
        }

        private static void DefaultVerboseProgress(string format, int step, int numSteps)
        {
            Utils.ThrowException(step < 1 ? new ArgumentOutOfRangeException("step") : null);
            if (numSteps <= 0)
            {
                if (mVerbose && step % 100 == 0)
                {
                    Console.Write("\r" + format, step); // throws ArgumentNullException, FormatException
                }
            }
            else
            {
                if (mVerbose && (step % 100 == 0 || step == numSteps))
                {
                    Console.Write("\r" + format, step, numSteps); // throws ArgumentNullException, FormatException
                    if (step == numSteps) { Console.WriteLine(); }
                }
            }
        }

        public static void Verbose(string format, params object[] args)
        {
            if (mVerbose && mVerboseProc != null) { mVerboseProc(format, args); } 
        }

        public static void VerboseLine(string format, params object[] args)
        {
            if (mVerbose && mVerboseLineProc != null) { mVerboseLineProc(format, args); } 
        }

        public static void VerboseLine()
        {
            if (mVerbose && mVerboseLineProc != null) { mVerboseLineProc(""); } 
        }

        public static void VerboseProgress(string format, int step, int numSteps)
        {
            if (mVerbose && mVerboseProgressProc != null) { mVerboseProgressProc(format, step, numSteps); }
        }

        public static void VerboseProgress(string format, int step)
        {
            VerboseProgress(format, step, /*numSteps=*/-1);
        }

        public static bool VerboseEnabled
        {
            get { return mVerbose; }
            set { mVerbose = value; }
        }

        public static VerboseDelegate VerboseFunc
        {
            get { return mVerboseProc; }
            set { mVerboseProc = value; }
        }

        public static VerboseDelegate VerboseLineFunc
        {
            get { return mVerboseLineProc; }
            set { mVerboseLineProc = value; }
        }

        public static VerboseProgressDelegate VerboseProgressFunc
        {
            get { return mVerboseProgressProc; }
            set { mVerboseProgressProc = value; }
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

        public static bool VerifyPathName(string pathName, bool mustExist)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(pathName);
                return mustExist ? dirInfo.Exists : true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] ReadAllBytes(Stream stream)
        {
            ThrowException(stream == null ? new ArgumentNullException("stream") : null);
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length); // throws IOException, ObjectDisposedException, NotSupportedException
                    if (read <= 0) { return ms.ToArray(); }
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public static bool CheckUnicodeSignature(string fileName)
        {
            ThrowException(!VerifyFileNameOpen(fileName) ? new ArgumentValueException("fileName") : null);
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bom = new byte[4]; // get the byte-order mark, if there is one
            file.Read(bom, 0, 4);
            file.Close();
            return ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) || // utf-8
                (bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le
                (bom[0] == 0xfe && bom[1] == 0xff) || // utf-16 and ucs-2
                (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)); // ucs-4
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
                Guid md5Hash = new Guid(hashAlgo.ComputeHash(buffer));
                return md5Hash.GetHashCode();
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

        public static double GetVecLenL2(SparseVector<double>.ReadOnly vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = 0;
            ArrayList<double> datInner = vec.Inner.InnerDat;
            foreach (double val in datInner)
            {
                len += val * val;
            }
            return Math.Sqrt(len);
        }

        public static void NrmVecL2(SparseVector<double> vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = GetVecLenL2(vec);
            Utils.ThrowException(len == 0 ? new InvalidOperationException() : null);
            ArrayList<double> datInner = vec.InnerDat;
            for (int i = 0; i < vec.Count; i++)
            {
                vec.SetDirect(i, datInner[i] / len);
            }
        }

        public static bool TryNrmVecL2(SparseVector<double> vec)
        {
            Utils.ThrowException(vec == null ? new ArgumentNullException("vec") : null);
            double len = GetVecLenL2(vec);
            if (len == 0) { return false; }
            ArrayList<double> datInner = vec.InnerDat;
            for (int i = 0; i < vec.Count; i++)
            {
                vec.SetDirect(i, datInner[i] / len);
            }
            return true;
        }

        // *** Delegates ***

        public delegate T UnaryOperatorDelegate<T>(T val);
        public delegate T BinaryOperatorDelegate<T>(T a, T b);
    }
}