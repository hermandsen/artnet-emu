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
using System.Collections.Generic;
using System.Reflection;

using log4net;

namespace Vajhoej.Record
{
    /// <summary>
    /// Class SubClassAndPad contains information about class and padding for select field.
    /// </summary>
    public class SubClassAndPad
    {
        private Type subClass;
        private int pad;
        /// <summary>
        /// Create instance of ClassAndPad with all necessary properties.
        /// </summary>
        /// <param name="subClass">Class.</param>
        /// <param name="pad">Padding.</param>
        public SubClassAndPad(Type subClass, int pad)
        {
            this.subClass = subClass;
            this.pad = pad;
        }
        /// <summary>
        /// Class.
        /// </summary>
        public Type SubClass
        {
            get { return subClass; }
        }
        /// <summary>
        /// Padding.
        /// </summary>
        public int Pad
        {
            get { return pad; }
        }
    }
    /// <summary>
    /// Class FieldInfo contains information about a native struct field needed for conversions.
    /// </summary>
    public class FieldInfo
    {
        private FieldType structType;
        private int length;
        private int decimals;
        private string encoding;
        private byte zone;
        private int prefixLength;
        private Type classType;
        private System.Reflection.FieldInfo field;
        private IDictionary<int, SubClassAndPad> selects;
        private bool selectPad;
        private int elements;
        private int countprefix;
        /// <summary>
        /// Create instance of FieldInfo with all necessary properties.
        /// </summary>
        /// <param name="structType">native struct type.</param>
        /// <param name="length">length of fixed length string.</param>
        /// <param name="decimals">Number of decimals.</param>
        /// <param name="encoding">Encoding of string.</param>
        /// <param name="zone">Zone of zoned BCD.</param>
        /// <param name="prefixLength">Prefix length of variable length string</param>
        /// <param name="classType">.NET class type.</param>
        /// <param name="field">Corresponding reflection object.</param>
        /// <param name="selects">Sub class selections.</param>
        /// <param name="selectPad">Pad sub classes to fixed length.</param>
        /// <param name="elements">Number of elements.</param>
        /// <param name="countprefix">Length in bytes of count prefix.</param>
        public FieldInfo(FieldType structType, int length, int decimals, String encoding, byte zone, int prefixLength, Type classType, System.Reflection.FieldInfo field, IDictionary<int, SubClassAndPad> selects, bool selectPad, int elements, int countprefix)
        {
            this.structType = structType;
            this.length = length;
            this.decimals = decimals;
            this.encoding = encoding;
            this.zone = zone;
            this.prefixLength = prefixLength;
            this.classType = classType;
            this.field = field;
            this.selects = selects;
            this.selectPad = selectPad;
            this.elements = elements;
            this.countprefix = countprefix;
        }
        /// <summary>
        /// Struct type.
        /// </summary>
        public FieldType StructType
        {
            get { return structType; }
        }
        /// <summary>
        /// Length of fixed length string.
        /// </summary>
        public int Length
        {
            get { return length; }
        }
        /// <summary>
        /// Decimals of BCD.
        /// </summary>
        public int Decimals
        {
            get { return decimals; }
        }
        /// <summary>
        /// Encoding of string.
        /// </summary>
        public string Encoding
        {
            get { return encoding; }
        }
        /// <summary>
        /// Zone of zoned BCD.
        /// </summary>
        public byte Zone
        {
            get { return zone; }
        }
        /// <summary>
        /// Prefix length.
        /// </summary>
        public int PrefixLength
        {
            get { return prefixLength; }
        }
        /// <summary>
        /// .NET class type.
        /// </summary>
        public Type ClassType
        {
            get { return classType; }
        }
        /// <summary>
        /// Corresponding reflection object.
        /// </summary>
        public System.Reflection.FieldInfo Field
        {
            get { return field; }
        }
        /// <summary>
        /// Sub class selections.
        /// </summary>
        public IDictionary<int, SubClassAndPad> Selects
        {
            get { return selects; }
        }
        /// <summary>
        /// Sub class padding to fixed length.
        /// </summary>
        public bool SelectPad
        {
            get { return selectPad; }
        }
        /// <summary>
        /// Number of elements.
        /// </summary>
        public int Elements
        {
            get { return elements; }
        }
        /// <summary>
        /// Length in bytes of count prefix.
        /// </summary>
        public int Countprefix
        {
            get { return countprefix; }
        }
    }
    /// <summary>
    /// Class StructInfo contains information about a native struct needed for reading and/or writing.
    /// </summary>
    public class StructInfo {
        private static ILog log = LogManager.GetLogger(typeof(StructInfo));
        private Endian endianess;
        private Alignment alignment;
        private bool endpad;
        private IList<FieldInfo> fields;
        private bool fixedLength;
        private int length;
        /// <summary>
        /// Create instance of StructInfo.
        /// </summary>
        /// <param name="endianess">Byte order for all fields.</param>
        /// <param name="alignment">Alignment for all fields.</param>
        /// <param name="endpad">Pad at end.</param>
        /// <param name="fields">Array of FieldInfo describing all fields.</param>
        /// <param name="clz">Class implementing struct.</param>
        /// <exception cref="RecordException">If error calculation length information.</exception>
        public StructInfo(Endian endianess, Alignment alignment, bool endpad, IList<FieldInfo> fields, Type clz)
        {
            this.endianess = endianess;
            this.alignment = alignment;
            this.endpad = endpad;
            this.fields = fields;
            fixedLength = CalculateFixedLength(fields);
            length = CalculateLength(fields, alignment, endpad, clz);
        }
        /// <summary>
        /// Endianess.
        /// </summary>
        public Endian Endianess
        {
            get { return endianess; }
        }
        /// <summary>
        /// Alignment.
        /// </summary>
        public Alignment Alignment
        {
            get { return alignment; }
        }
        /// <summary>
        /// Pad at end.
        /// </summary>
        public bool Endpad
        {
            get { return endpad; }
        }
        /// <summary>
        /// Fields.
        /// </summary>
        public IList<FieldInfo> Fields
        {
            get { return fields; }
        }
        /// <summary>
        /// Fixed length struct.
        /// </summary>
        public bool FixedLength
        {
            get { return fixedLength; }
        }
        /// <summary>
        /// Length.
        /// </summary>
        public int Length
        {
            get { return length; }
        }
        /// <summary>
        /// Analyze class.
        /// </summary>
        /// <param name="clz">Class to analyze.</param>
        /// <returns>StructInfo for class.</returns>
        /// <exception cref="RecordException">If error calculation length information.</exception>
        public static StructInfo Analyze(Type clz)
        {
            IList<FieldInfo> fi = new List<FieldInfo>();
            Analyze(clz, fi, 0, true);
            log.Debug(clz.FullName + " analyzed for StructInfo");
            StructAttribute s = (StructAttribute)Attribute.GetCustomAttribute(clz, typeof(StructAttribute));
            return new StructInfo(s.Endianess, s.Alignment, s.Endpad, fi, clz);
        }
        internal static int CalculatePad(int pos, Alignment align, FieldInfo fi)
        {
            int nbyte = Natural(fi);
            int npad = 0;
            switch(align)
            {
                case Alignment.PACKED:
                case Alignment.ALIGN1:
                    npad = 0;
                    break;
                case Alignment.NATURAL:
                    npad = (nbyte - pos % nbyte) % nbyte;
                    break;
                case Alignment.ALIGN2:
                    npad = (2 - pos % 2) % 2;
                    break;
                case Alignment.ALIGN4:
                    npad = (4 - pos % 4) % 4;
                    break;
                case Alignment.ALIGN8:
                    npad = (8 - pos % 8) % 8;
                    break;
            }
            return npad;
        }
        internal static int CalculateEndPad(int pos, Alignment align, IList<FieldInfo> fields)
        {
            int npad = 0;
            foreach(FieldInfo fi in fields)
            {
                npad = Math.Max(npad, CalculatePad(pos, align, fi));
            }
            return npad;
        }
        private static void Analyze(Type clz, IList<FieldInfo> allfi, int offset, bool dosuper)
        {
            StructAttribute s = (StructAttribute)Attribute.GetCustomAttribute(clz, typeof(StructAttribute));
            if(s == null)
            {
                throw new ArgumentException(clz.FullName + " is not a struct");
            }
            if(dosuper && clz.BaseType != typeof(object))
            {
                Analyze(clz.BaseType, allfi, offset, true);
            }
            System.Reflection.FieldInfo[] refl = clz.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for(int i = 0; i < refl.Length; i++)
            {
                if(!refl[i].IsStatic)
                {
                    allfi.Add(null);
                }
            }
            for(int i = 0; i < refl.Length; i++)
            {
                if(!refl[i].IsStatic)
                {
                    StructFieldAttribute sf = (StructFieldAttribute)Attribute.GetCustomAttribute(refl[i], typeof(StructFieldAttribute));
                    if(sf == null)
                    {
                        throw new ArgumentException(clz.FullName + " contains a field " + refl[i].Name + " with no meta-data");
                    }
                    SelectorAttribute sel = (SelectorAttribute)Attribute.GetCustomAttribute(refl[i], typeof(SelectorAttribute));
                    IDictionary<int, SubClassAndPad> selmap = null;
                    bool selpad = false;
                    if(sel != null)
                    {
                        sel.Subtypes = (SubTypeAttribute[])Attribute.GetCustomAttributes(refl[i], typeof(SubTypeAttribute));
                        selmap = new Dictionary<int, SubClassAndPad>();
                        selpad = sel.Pad;
                        if(sel.Pad)
                        {
                            int maxlen = 0;
                            foreach(SubTypeAttribute st in sel.Subtypes)
                            {
                                maxlen = Math.Max(maxlen, CalculateExtraLength(st.Type, sf.N + 1));
                            }
                            foreach(SubTypeAttribute st in sel.Subtypes)
                            {
                                selmap.Add(st.Value, new SubClassAndPad(st.Type, maxlen - CalculateExtraLength(st.Type, sf.N + 1)));
                            }
                        }
                        else
                        {
                            foreach(SubTypeAttribute st in sel.Subtypes)
                            {
                                selmap.Add(st.Value, new SubClassAndPad(st.Type, 0));
                            }
                        }
                    }
                    int elm = 1;
                    int prefix = 0;
                    ArrayFieldAttribute arr = (ArrayFieldAttribute)Attribute.GetCustomAttribute(refl[i], typeof(ArrayFieldAttribute));
                    if(arr != null)
                    {
                        elm = arr.Elements;
                        prefix = arr.Countprefix;
                    }
                    if(sf.N - offset < allfi.Count - refl.Length || sf.N - offset >= allfi.Count)
                    {
                        throw new ArgumentException(refl[i].Name + " in " + clz.FullName + " has illegal number");
                    }
                    if(allfi[sf.N - offset] == null)
                    {
                        allfi[sf.N - offset] = new FieldInfo(sf.Type, sf.Length, sf.Decimals, sf.Encoding, sf.Zone, sf.PrefixLength, refl[i].FieldType, refl[i], selmap, selpad, elm, prefix);
                    }
                    else
                    {
                        throw new ArgumentException(clz.FullName + " has duplicates in field ordering");
                    }
                }
            }
        }
        private static bool CalculateFixedLength(IList<FieldInfo> fields)
        {
            bool res = true;
            foreach(FieldInfo fi in fields)
            {
                if(fi.StructType == FieldType.VARSTR || fi.StructType == FieldType.REMSTR)
                {
                    res = false;
                }
                else if(fi.StructType == FieldType.STRUCT)
                {
                    if(fi.ClassType.IsArray)
                    {
                        res = res && Analyze(fi.ClassType.GetElementType()).FixedLength;
                    }
                    else
                    {
                        res = res && Analyze(fi.ClassType).FixedLength;
                    }
                }
                if(fi.Selects != null)
                {
                    res = res && fi.SelectPad;
                }
            }
            return res;
        }
        private static int CalculateLength(IList<FieldInfo> fields, Alignment alignment, bool endpad, Type clz)
        {
            int res = 0;
            for(int i = 0; i < fields.Count; i++)
            {
                FieldInfo fi = fields[i];
                res += CalculatePad(res, alignment, fi);
                int nelm;
                if(fi.ClassType.IsArray) {
                	nelm = fi.Elements;
                } else {
                	nelm = 1;
                }
                switch(fi.StructType)
                {
                    case FieldType.REMSTR:
                        res += 0;
                        break;
                    case FieldType.INT1:
                    case FieldType.UINT1:
                        res += nelm*1;
                        break;
                    case FieldType.INT2:
                    case FieldType.UINT2:
                        res += nelm*2;
                        break;
                    case FieldType.INT4:
                    case FieldType.UINT4:
                    case FieldType.FP4:
                    case FieldType.UNIXTIME:
                    case FieldType.VAXFP4:
                        res += nelm*4;
                        break;
                    case FieldType.INT8:
                    case FieldType.FP8:
                    case FieldType.JAVATIME:
                    case FieldType.VMSTIME:
                    case FieldType.VAXFP8:
                        res += nelm*8;
                        break;
                    case FieldType.INTX:
                    case FieldType.FIXSTR:
                    case FieldType.FIXSTRNULTERM:
                    case FieldType.BOOLEAN:
                    case FieldType.PACKEDBCD:
                    case FieldType.ZONEDBCD:
                        res += nelm*fi.Length;
                        break;
                    case FieldType.VARSTR:
                        res += nelm*2;
                        break;
                    case FieldType.VARFIXSTR:
                        res += nelm*(2 + fi.Length);
                        break;
                    case FieldType.BIT:
                        res += nelm*((fi.Length + 7) / 8);
                        break;
                    case FieldType.STRUCT:
                    	if(fi.ClassType.IsArray)
                    	{
                    	    res += nelm*Analyze(fi.ClassType.GetElementType()).Length;
                    	}
                    	else
                    	{
                            res += nelm*Analyze(fi.ClassType).Length;
                    	}
                        break;
                    default:
                        throw new RecordException(fi.StructType + " is an unknown type");
                }
                if(fi.Selects != null)
                {
                    if(fi.SelectPad)
                    {
                        bool fnd = false; 
                        foreach(SubClassAndPad scp in fi.Selects.Values)
                        {
                            if(scp.SubClass == clz)
                            {
                                res += scp.Pad;
                                fnd = true;
                            }
                        }
                        if(!fnd)
                        {
                            IEnumerator<SubClassAndPad> en = fi.Selects.Values.GetEnumerator();
                            en.MoveNext();
                            SubClassAndPad scp = en.Current;
                            res = Analyze(scp.SubClass).Length;
                        }
                    }
                }
            }
            if(endpad)
            {
                res += CalculateEndPad(res, alignment, fields);
            }
            return res;
        }
        private static int CalculateExtraLength(Type clz, int offset)
        {
            IList<FieldInfo> fi = new List<FieldInfo>();
            Analyze(clz, fi, offset, false);
            StructAttribute s = (StructAttribute)Attribute.GetCustomAttribute(clz, typeof(StructAttribute));
            return CalculateLength(fi, s.Alignment, s.Endpad, clz);
        }
        private static int Natural(FieldInfo fi)
        {
            switch(fi.StructType) {
                case FieldType.INT1:
                case FieldType.UINT1:
                case FieldType.INTX:
                case FieldType.FIXSTR:
                case FieldType.FIXSTRNULTERM:
                case FieldType.REMSTR:
                case FieldType.PACKEDBCD:
                case FieldType.ZONEDBCD:
                case FieldType.BIT:
                case FieldType.STRUCT:
                    return 1;
                case FieldType.INT2:
                case FieldType.UINT2:
                case FieldType.VARSTR:
                case FieldType.VARFIXSTR:
                    return 2;
                case FieldType.INT4:
                case FieldType.UINT4:
                case FieldType.FP4:
                case FieldType.UNIXTIME:
                case FieldType.VAXFP4:
                    return 4;
                case FieldType.INT8:
                case FieldType.FP8:
                case FieldType.JAVATIME:
                case FieldType.VMSTIME:
                case FieldType.VAXFP8:
                    return 8;
                case FieldType.BOOLEAN:
                    return fi.Length;
                default:
                    throw new RecordException(fi.StructType + " is an unknown type");
            }
        }
    }
    /// <summary>
    /// Class StructInfoCache caches StructInfo objects in a singleton cache.
    /// </summary>
    public class StructInfoCache
    {
        private static ILog log = LogManager.GetLogger(typeof(StructInfoCache));
        private static StructInfoCache instance = null;
        private IDictionary<Type, StructInfo> cache;
        private int found;
        private int total;
        private object lockobj = new object();
        private StructInfoCache() {
            Reset();
        }
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static StructInfoCache Instance
        {
            get
            {
                lock(typeof(StructInfoCache))
                {
                    if(instance == null)
                    {
                        instance = new StructInfoCache();
                    }
                    return instance;
                }
            }
        }
        /// <summary>
        /// Get StructInfo from cache.
        /// </summary>
        /// <param name="clz">Class we want StructInfo for.</param>
        /// <returns>StructInfo.</returns>
        public StructInfo Get(Type clz) {
            lock(lockobj)
            {
                StructInfo res = null;
                if(cache.ContainsKey(clz))
                {
                    res = cache[clz];
                    found++;
                    log.Debug(clz.FullName + " found in StructInfoCache");
                }
                else
                {
                    log.Debug(clz.FullName + " not found in StructInfoCache");
                }
                total++;
                return res;
            }
        }
        /// <summary>
        /// Put StructInfo into cache.
        /// </summary>
        /// <param name="clz">Class we have StructInfo for.</param>
        /// <param name="si">StructInfo.</param>
        public void Put(Type clz, StructInfo si) {
            lock(lockobj)
            {
                cache.Add(clz, si);
                log.Debug(clz.FullName + " put in StructInfoCache");
            }
        }
        /// <summary>
        /// Cache hit rate.
        /// </summary>
        public double HitRate
        {
            get
            {
                lock(lockobj)
                {
                    return (total > 0) ? (double)found/(double)total : -1;
                }
            }
        }
        /// <summary>
        /// Reset cache.
        /// </summary>
        public void Reset()
        {
            lock(lockobj)
            {
                cache = new Dictionary<Type, StructInfo>();
                found = 0;
                total = 0;
                log.Debug("StructInfoCache reset");
            }
        }
        /// <summary>
        /// Convenience method to get StructInfo from cache and analyze class if not in cache.
        /// </summary>
        /// <param name="t">Class.</param>
        /// <returns>StructInfo for class.</returns>
        /// <exception cref="RecordException">If error analyzing cache.</exception>
        public static StructInfo Analyze(Type t)
        {
            StructInfoCache cache = StructInfoCache.Instance;
            StructInfo si = cache.Get(t);
            if(si == null)
            {
                si = StructInfo.Analyze(t);
                cache.Put(t, si);
            }
            return si;
        }
    }
}