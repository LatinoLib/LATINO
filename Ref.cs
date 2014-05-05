/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    Ref.cs
 *  Desc:    Enables referencing value types
 *  Created: Feb-2010
 *
 *  Author:  Miha Grcar
 *
 *  License: MIT (http://opensource.org/licenses/MIT)
 *
 ***************************************************************************/

using System;

namespace Latino
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Ref<T>
       |
       '-----------------------------------------------------------------------
    */
    public class Ref<T> : ICloneable<Ref<T>>, IContentEquatable<Ref<T>>, ISerializable 
    {
        public T Val
            = default(T);

        public Ref()
        {
        }

        public Ref(T val)
        {
            Val = val;
        }

        public Ref(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public static implicit operator T(Ref<T> refVal)
        {
            return refVal.Val;
        }

        public static implicit operator Ref<T>(T val)
        {
            return new Ref<T>(val);
        }

        public override string ToString()
        {
            return Val == null ? "" : Val.ToString();
        }

        // *** ICloneable<Ref<T>> interface implementation ***

        public Ref<T> Clone()
        {
            return new Ref<T>(Val);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // *** IContentEquatable<Ref<T>> interface implementation ***

        public bool ContentEquals(Ref<T> other)
        {
            if (other == null) { return false; }
            return Utils.ObjectEquals(other.Val, Val, /*deepCmp=*/true);
        }

        bool IContentEquatable.ContentEquals(object other)
        {
            Utils.ThrowException((other != null && !(other is Ref<T>)) ? new ArgumentTypeException("other") : null);
            return ContentEquals((Ref<T>)other);
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions
            writer.WriteObject(Val);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            Val = reader.ReadObject<T>();
        }
    }
}
