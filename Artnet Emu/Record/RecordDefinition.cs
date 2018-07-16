/*
 * Copyright 2010 Arne Vajhøj.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */

using System;

namespace Vajhoej.Record
{
    /// <summary>
    /// Enum FieldType specifies native struct types.
    /// <br/>
    /// Semantics:
    /// <table border="yes">
    /// <tr>
    /// <th>enum value</th>
    /// <th>description</th>
    /// <th>attributes</th>
    /// <th>native implementation</th>
    /// <th>.NET implementation</th>
    /// </tr>
    /// <tr>
    /// <td>INT1</td>
    /// <td></td>
    /// <td></td>
    /// <td>8 bit signed integer</td>
    /// <td>sbyte</td>
    /// </tr>
    /// <tr>
    /// <td>INT2</td>
    /// <td></td>
    /// <td></td>
    /// <td>16 bit signed integer</td>
    /// <td>short</td>
    /// </tr>
    /// <tr>
    /// <td>INT4</td>
    /// <td></td>
    /// <td></td>
    /// <td>32 bit signed integer</td>
    /// <td>int</td>
    /// </tr>
    /// <tr>
    /// <td>INT8</td>
    /// <td></td>
    /// <td></td>
    /// <td>64 bit signed integer</td>
    /// <td>long</td>
    /// </tr>
    /// <tr>
    /// <td>UINT1</td>
    /// <td></td>
    /// <td></td>
    /// <td>8 bit unsigned integer</td>
    /// <td>byte</td>
    /// </tr>
    /// <tr>
    /// <td>UINT2</td>
    /// <td></td>
    /// <td></td>
    /// <td>16 bit unsigned integer</td>
    /// <td>ushort</td>
    /// </tr>
    /// <tr>
    /// <td>UINT4</td>
    /// <td></td>
    /// <td></td>
    /// <td>32 bit unsigned integer</td>
    /// <td>uint</td>
    /// </tr>
    /// <tr>
    /// <td>FP4</td>
    /// <td></td>
    /// <td></td>
    /// <td>32 bit IEEE floating point</td>
    /// <td>float</td>
    /// </tr>
    /// <tr>
    /// <td>FP8</td>
    /// <td></td>
    /// <td></td>
    /// <td>64 bit IEEE floating point</td>
    /// <td>double</td>
    /// </tr>
    /// <tr>
    /// <td>INTX</td>
    /// <td></td>
    /// <td>length=&lt;bytes used&gt;</td>
    /// <td>bytes</td>
    /// <td>ulong</td>
    /// </tr>
    /// <tr>
    /// <td>FIXSTR</td>
    /// <td>Fixed length string</td>
    /// <td>length=&lt;length of string&gt;<br/>encoding=&lt;encoding used&gt;<br/>(default encoding is ISO-8859-1)</td>
    /// <td>sequence of bytes</td>
    /// <td>string</td>
    /// </tr>
    /// <tr>
    /// <td>FIXSTRNULTERM</td>
    /// <td>Fixed length string nul terminated</td>
    /// <td>length=&lt;length of string&gt;<br/>encoding=&lt;encoding used&gt;<br/>(default encoding is ISO-8859-1)</td>
    /// <td>sequence of bytes with nul bytes added for write and stripped for read
    /// </td>
    /// <td>string</td>
    /// </tr>
    /// <tr>
    /// <td>VARSTR</td>
    /// <td>Variable length string with 2 byte length prefix</td>
    /// <td>encoding=&lt;encoding used&gt;<br/>(default encoding is ISO-8859-1, max. length is 32767)</td>
    /// <td>2 byte length + sequence of bytes</td>
    /// <td>string</td>
    /// </tr>
    /// <tr>
    /// <td>VARFIXSTR</td>
    /// <td>Variable length string with 2 byte length prefix and padded to max length</td>
    /// <td>length=&lt;length of string&gt;<br/>encoding=&lt;encoding used&gt;<br/>(default encoding is ISO-8859-1, max. length is 32767)</td>
    /// <td>2 byte length + sequence of bytes</td>
    /// <td>string</td>
    /// </tr>
    /// <tr>
    /// <td>REMSTR</td>
    /// <td>Remaing data string</td>
    /// <td>encoding=&lt;encoding used&gt;<br/>(default encoding is ISO-8859-1, max. length is 32767)</td>
    /// <td>sequence of bytes</td>
    /// <td>string</td>
    /// </tr>
    /// <tr>
    /// <td>BOOLEAN</td>
    /// <td>Boolean (0=false, other=true)</td>
    /// <td>length=&lt;bytes used&gt;</td>
    /// <td>bytes</td>
    /// <td>bool</td>
    /// </tr>
    /// <tr>
    /// <td>BIT</td>
    /// <td>Bits</td>
    /// <td>length=&lt;bits used&gt; (max. bits is 32)</td>
    /// <td>bytes</td>
    /// <td>int</td>
    /// </tr>
    /// <tr>
    /// <td>JAVATIME</td>
    /// <td>Binary time in Java format (milliseconds since 1-Jan-1970)</td>
    /// <td></td>
    /// <td>64 bit integer</td>
    /// <td>System.DateTime</td>
    /// </tr>
    /// <tr>
    /// <td>UNIXTIME</td>
    /// <td>Binary time in Unix format (seconds since 1-Jan-1970)</td>
    /// <td></td>
    /// <td>32 bit integer</td>
    /// <td>System.DateTime</td>
    /// </tr>
    /// <tr>
    /// <td>VMSTIME</td>
    /// <td>Binary time in VMS format (100 nanoseconds since 17-Nov-1858)</td>
    /// <td></td>
    /// <td>64 bit integer</td>
    /// <td>System.DateTime</td>
    /// </tr>
    /// <tr>
    /// <td>PACKEDBCD</td>
    /// <td>Packed BCD (1 byte = 2 decimal digit nibbles)</td>
    /// <td>length=&lt;bytes used&gt;<br/>decimals=&lt;number of implied decimals&gt;<br/>(default decimals is 0)</td>
    /// <td>sequence of bytes</td>
    /// <td>decimal</td>
    /// </tr>
    /// <tr>
    /// <td>ZONEDBCD</td>
    /// <td>Zoned BCD (1 byte = 1 zone nibble + 1 decimal digit nibble)</td>
    /// <td>length=&lt;bytes used&gt;<br/>decimals=&lt;number of implied decimals&gt;<br/>zone=&lt;zone value&gt;<br/>(default decimals i s0, default zone is EBCDIC)</td>
    /// <td>sequence of bytes</td>
    /// <td>decimal</td>
    /// </tr>
    /// <tr>
    /// <td>VAXFP4</td>
    /// <td>VAX F floating point</td>
    /// <td></td>
    /// <td>32 bit VAX floating point</td>
    /// <td>float</td>
    /// </tr>
    /// <tr>
    /// <td>VAXFP8</td>
    /// <td>VAX G floating point</td>
    /// <td></td>
    /// <td>64 bit VAX floating point</td>
    /// <td>double</td>
    /// </tr>
    /// <tr>
    /// <td>STRUCT</td>
    /// <td>Sub struct</td>
    /// <td></td>
    /// <td></td>
    /// <td></td>
    /// </tr>
    /// </table>
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 8 bit signed integer.
        /// </summary>
        INT1,
        /// <summary>
        /// 16 bit signed integer.
        /// </summary>
        INT2,
        /// <summary>
        /// 32 bit signed integer.
        /// </summary>
        INT4,
        /// <summary>
        /// 64 bit signed integer.
        /// </summary>
        INT8,
        /// <summary>
        /// 8 bit unsigned integer.
        /// </summary>
        UINT1,
        /// <summary>
        /// 16 bit unsigned integer.
        /// </summary>
        UINT2,
        /// <summary>
        /// 32 bit unsigned integer.
        /// </summary>
        UINT4,
        /// <summary>
        /// 32 bit IEEE floating point.
        /// </summary>
        FP4,
        /// <summary>
        /// 64 bit IEEE floating point.
        /// </summary>
        FP8,
        /// <summary>
        /// 8-56 bit integer (intended for 24, 40, 48 and 56 bits).
        /// </summary>
        INTX,
        /// <summary>
        /// Fixed length string.
        /// </summary>
        FIXSTR,
        /// <summary>
        /// Fixed length string nul terminated.
        /// </summary>
        FIXSTRNULTERM,
        /// <summary>
        /// Variable length string with 2 byte length prefix.
        /// </summary>
        VARSTR,
        /// <summary>
        /// Variable length string with 2 byte length prefix and padded to max length.
        /// </summary>
        VARFIXSTR,
        /// <summary>
        /// Remaining data string.
        /// </summary>
        REMSTR,
        /// <summary>
        /// Boolean.
        /// </summary>
        BOOLEAN,
        /// <summary>
        /// Bits.
        /// </summary>
        BIT,
        /// <summary>
        /// Binary time in Java format.
        /// </summary>
        JAVATIME,
        /// <summary>
        /// Binary time in Unix format.
        /// </summary>
        UNIXTIME,
        /// <summary>
        /// Binary time in VMS format.
        /// </summary>
        VMSTIME,
        /// <summary>
        /// Packed BCD.
        /// </summary>
        PACKEDBCD,
        /// <summary>
        /// Zoned BCD.
        /// </summary>
        ZONEDBCD,
        /// <summary>
        /// VAX F floating point.
        /// </summary>
        VAXFP4,
        /// <summary>
        /// VAX G floating point.
        /// </summary>
        VAXFP8,
        /// <summary>
        /// Sub struct.
        /// </summary>
        STRUCT
    }
    /// <summary>
    /// Enum Alignment specifies alignment within native struct.
    /// </summary>
    public enum Alignment
    {
        /// <summary>
        /// No padding.
        /// </summary>
        PACKED,
        /// <summary>
        /// Padding to natural alignment.
        /// </summary>
        NATURAL,
        /// <summary>
        /// Padding to multipla of 1 alignment (same as PACKED).
        /// </summary>
        ALIGN1,
        /// <summary>
        /// Padding to multipla of 2 alignment. 
        /// </summary>
        ALIGN2,
        /// <summary>
        /// Padding to multipla of 4 alignment. 
        /// </summary>
        ALIGN4,
        /// <summary>
        /// Padding to multipla of 8 alignment.
        /// </summary>
        ALIGN8
    }
    /// <summary>
    /// Enum Endian specifies endianess within native struct.
    /// </summary>
    public enum Endian
    {
        /// <summary>
        /// Little endian.
        /// </summary>
        LITTLE,
        /// <summary>
        /// Big endian (alias network order).
        /// </summary>
        BIG
    }
    /// <summary>
    /// Annotation for fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class StructFieldAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public StructFieldAttribute()
        {
            Length = 0;
            Decimals = 0;
            Encoding = "ISO-8859-1";
            zone = BCDUtil.EBCDIC;
            prefixLength = 2;
        }
        private int n;
        /// <summary>
        /// Field number.
        /// </summary>
        public int N
        {
            get { return n; }
            set { n = value; }
        }
        private FieldType type;
        /// <summary>
        /// Field type.
        /// </summary>
        public FieldType Type
        {
            get { return type; }
            set { type = value; }
        }
        private int length;
        /// <summary>
        /// Field length (for fixed length strings and BCD's).
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        private int decimals;
        /// <summary>
        /// Field decimals (for BCD's). 
        /// </summary>
        public int Decimals
        {
            get { return decimals; }
            set { decimals = value; }
        }
        private String encoding;
        /// <summary>
        /// Field encoding (for strings).
        /// </summary>
        public String Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }
        private byte zone;
        /// <summary>
        /// Field zone value (for zoned BCD's).
        /// </summary>
        public byte Zone
        {
            get { return zone; }
            set { zone = value; }
        }
        private int prefixLength;
        /// <summary>
        /// Prefix length (for variable length strings).
        /// </summary>
        public int PrefixLength
        {
            get { return prefixLength; }
            set { prefixLength = value; }
        }
    }
    /// <summary>
    /// Annotation for structs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StructAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public StructAttribute()
        {
            Endianess = Endian.LITTLE;
            Alignment = Alignment.PACKED;
            Endpad = false;
        }
        private Endian endianess;
        /// <summary>
        /// Byte order. Default is little endian.
        /// </summary>
        public Endian Endianess
        {
            get { return endianess; }
            set { endianess = value; }
        }
        private Alignment alignment;
        /// <summary>
        /// Alignment. Default is packed.
        /// </summary>
        public Alignment Alignment
        {
            get { return alignment; }
            set { alignment = value; }
        }
        private bool endpad;
        /// <summary>
        /// End padding. Default is false.
        /// </summary>
        public bool Endpad
        {
            get { return endpad; }
            set { endpad = value; }
        }
    }
    /// <summary>
    /// Annotation for sub types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=true)]
    public class SubTypeAttribute : Attribute
    {
        private int _value;
        /// <summary>
        /// Value of selector.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private Type type;
        /// <summary>
        /// Sub type class.
        /// </summary>
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }
    }
    /// <summary>
    /// Annotation for selection of sub types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelectorAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectorAttribute()
        {
            Pad = false;
        }
        private object[] subtypes;
        /// <summary>
        /// Available sub types.
        /// </summary>
        public object[] Subtypes
        {
            get { return subtypes; }
            set { subtypes = value; }
        }
        private bool pad;
        /// <summary>
        /// Pad all sub types to same length.
        /// </summary>
        public bool Pad
        {
            get { return pad; }
            set { pad = value; }
        }
    }
    /// <summary>
    /// Annotation for arrays.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ArrayFieldAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArrayFieldAttribute()
        {
            elements = 1;
            countprefix = 0;
        }
        private int elements;
        /// <summary>
        /// Number of elements in array.
        /// </summary>
        public int Elements
        {
            get { return elements; }
            set { elements = value; }
        }
        private int countprefix;
        /// <summary>
        /// Length in bytes of count prefix.
        /// </summary>
        public int Countprefix 
        {
            get { return countprefix; }
            set { countprefix = value; }
        }
    }
    /// <summary>
    /// Get length of field.
    /// Note: can only be used with struct fields of field types FIXSTR, FIXSTRNULTERM,
    /// PACKEDBCD and ZONEDPBCD.
    /// </summary>
    /// <param name="o">Object (not completely initialized for read).</param>
    /// <param name="n">Field number.</param>
    /// <returns>Length (values &lt; 0 indicates that value is to be ignored).</returns>
    public delegate int LengthProvider(object o, int n);
    /// <summary>
    /// Get max length of struct.
    /// Note: can only be used with struct fields of field types FIXSTR, FIXSTRNULTERM,
    /// PACKEDBCD and ZONEDBCD.
    /// </summary>
    /// <returns>Max length (values &lt; 0 indicates that value is to be ignored).</returns>
    public delegate int MaxLengthProvider();
    /// <summary>
    /// Get number of elements in array.
    /// Note: can only be used with struct fields that are arrays.
    /// </summary>
    /// <param name="o">Object (not completely initialized for read).</param>
    /// <param name="n">Field number.</param>
    /// <returns>Elements (values &lt; 0 indicates that value is to be ignored).</returns>
    public delegate int ElementsProvider(object o, int n);
    /// <summary>
    /// Converts a selector of any type to a usable integer selector.
    /// </summary>
    /// <param name="o">Object (not completely initialized for read).</param>
    /// <returns>Real selector (null indicates that it is to be ignored).</returns>
    public delegate int? ConvertSelector(object o);
}
