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
using System.IO;
using System.Reflection;
using System.Text;

using log4net;

namespace Vajhoej.Record
{
    /// <summary>
    /// Class StructReader reads a .NET object from a byte array containing a native struct.
    /// </summary>
    public class StructReader
    {
        private static ILog log = LogManager.GetLogger(typeof(StructReader));
        private EndianBinaryReader bb;
        /// <summary>
        /// Create instance of StructReader.
        /// </summary>
        /// <param name="ba">Byte array to read from.</param>
        public StructReader(byte[] ba)
        {
            bb = new EndianBinaryReader(new MemoryStream(ba));
            log.Debug("StructReader initialized with byte array of length " + ba.Length);
            if(log.IsDebugEnabled)
            {
                log.Debug("Byte array:" + LogHelper.ByteArrayToString(ba));
            }
        }

        /// <summary>
        /// Get or set the current position within the stream
        /// </summary>
        public long Position
        {
            get
            {
                return bb.BaseStream.Position;
            }
            set
            {
                bb.BaseStream.Position = value;
            }
        }
        /// <summary>
        /// Read.
        /// </summary>
        /// <typeparam name="T">Type of what to read.</typeparam>
        /// <param name="t">Type of what to read.</param>
        /// <returns>Object read.</returns>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public T Read<T>(Type t) where T : class, new()
        {
            return Read<T>(t, null, null, null, null);
        }
        /// <summary>
        /// Read.
        /// </summary>
        /// <typeparam name="T">Type of what to read.</typeparam>
        /// <param name="t">Type of what to read.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given.</param>
        /// <returns>Object read.</returns>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public T Read<T>(Type t, LengthProvider lenpvd) where T : class, new()
        {
            return Read<T>(t, lenpvd, null, null, null);
        }
        /// <summary>
        /// Read.
        /// </summary>
        /// <typeparam name="T">Type of what to read.</typeparam>
        /// <param name="t">Type of what to read.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given.</param>
        /// <param name="maxlenpvd">Supplies max length for fields where it is not given.</param>
        /// <returns>Object read.</returns>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public T Read<T>(Type t, LengthProvider lenpvd, MaxLengthProvider maxlenpvd) where T : class, new()
        {
            return Read<T>(t, lenpvd, maxlenpvd, null, null);
        }
        /// <summary>
        /// Read.
        /// </summary>
        /// <typeparam name="T">Type of what to read.</typeparam>
        /// <param name="t">Type of what to read.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given.</param>
        /// <param name="maxlenpvd">Supplies max length for fields where it is not given.</param>
        /// <param name="elmpvd">Supplies elements for fields where it is not given.</param>
        /// <returns>Object read.</returns>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public T Read<T>(Type t, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd) where T : class, new()
        {
            return Read<T>(t, lenpvd, maxlenpvd, elmpvd, null);
        }
        /// <summary>
        /// Read.
        /// </summary>
        /// <typeparam name="T">Type of what to read.</typeparam>
        /// <param name="t">Type of what to read.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given.</param>
        /// <param name="maxlenpvd">Supplies max length for fields where it is not given.</param>
        /// <param name="elmpvd">Supplies elements for fields where it is not given.</param>
        /// <param name="infpvd">Supplies selector converter for fields where it is needed.</param>
        /// <returns>Object read.</returns>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public T Read<T>(Type t, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd, ConvertSelector infpvd) where T : class, new()
        {
            try
            {
                log.Debug("Reading class " + t.FullName);
                long bitbuf = 0;
                int nbits = 0;
                int mark = (int)bb.BaseStream.Position;
                StructInfo si = StructInfoCache.Analyze(t);
                bb.CurrentEndian = si.Endianess;
                T res = (T)Activator.CreateInstance(t);
                for(int i = 0; i < si.Fields.Count; i++)
                {
                    FieldInfo fi = si.Fields[i];
                    int nskip = StructInfo.CalculatePad((int)bb.BaseStream.Position, si.Alignment, fi);
                    if(nskip > 0)
                    {
                        bb.ReadBytes(nskip);
                        log.Debug("Skip " + nskip + " padding bytes");
                    }
    			    int nelm;
    			    bool adjlen = false;
    			    if(elmpvd == null || elmpvd(res, i) < 0)
    			    {
    			        if(fi.Countprefix <= 0)
    			        {
    			    	    nelm = fi.Elements;
    			        }
    			        else
    			        {
    			    		switch(fi.Countprefix) {
    			    			case 1:
    			    				nelm = bb.ReadSByte();
    			    				break;
    			    			case 2:
    			    				nelm = bb.ReadInt16();
    			    				break;
    			    			case 4:
    			    				nelm = bb.ReadInt32();
    			    				break;
    			    			default:
    			    				throw new RecordException("Wrong length of count prefix " + fi.Field.Name + " in " + t.FullName + ": " + fi.Countprefix);
    			    		}
    			    	    adjlen = true;
    			        }
    			    }
    			    else
    			    {
    			    	nelm = elmpvd(res, i);
    			    	adjlen = true;
    			    }
   			    	// if relevant adjust the size of array
   			    	if(adjlen)
   			    	{
   			    	    if(fi.Field.GetValue(res) != null)
    			    	{
    			    	    if(fi.Field.GetValue(res).GetType().IsArray)
    			    	    {
    			    	        if(((Array)fi.Field.GetValue(res)).Length != nelm)
    					    	{
    			    	            fi.Field.SetValue(res, Array.CreateInstance(fi.Field.GetValue(res).GetType().GetElementType(), nelm));
    					    	}
    				    	}
    			    	}
   			    	}
                    for(int ix = 0; ix < nelm; ix++)
                    {
                        switch(fi.StructType)
                        {
                            case FieldType.INT1:
                                sbyte vint1 = bb.ReadSByte();
                                if(fi.ClassType == typeof(sbyte))
                                {
                                    fi.Field.SetValue(res, vint1);
                                }
                                else if(fi.ClassType == typeof(short))
                                {
                                    fi.Field.SetValue(res, vint1);
                                }
                                else if(fi.ClassType == typeof(int))
                                {
                                    fi.Field.SetValue(res, vint1);
                                }
                                else if(fi.ClassType == typeof(sbyte[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vint1, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read INT1 with value " + vint1);
                                break;
                            case FieldType.INT2:
                                short vint2 = bb.ReadInt16();
                                if(fi.ClassType == typeof(short))
                                {
                                    fi.Field.SetValue(res, vint2);
                                }
                                else if(fi.ClassType == typeof(int))
                                {
                                    fi.Field.SetValue(res, vint2);
                                }
                                else if(fi.ClassType == typeof(short[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vint2, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read INT2 with value " + vint2);
                                break;
                            case FieldType.INT4:
                                int vint4 = bb.ReadInt32();
                                if(fi.ClassType == typeof(int))
                                {
                                    fi.Field.SetValue(res, vint4);
                                }
                                else if(fi.ClassType == typeof(int[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vint4, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read INT4 with value " + vint4);
                                break;
                            case FieldType.INT8:
                                long vint8 = bb.ReadInt64();
                                if(fi.ClassType == typeof(long))
                                {
                                    fi.Field.SetValue(res, vint8);
                                }
                                else if(fi.ClassType == typeof(long[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vint8, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read INT8 with value " + vint8);
                                break;
                            case FieldType.UINT1:
                                byte vuint1 = bb.ReadByte();
                                if(fi.ClassType == typeof(byte))
                                {
                                    fi.Field.SetValue(res, vuint1);
                                }
                                else if(fi.ClassType == typeof(uint))
                                {
                                    fi.Field.SetValue(res, vuint1);
                                }
                                else if(fi.ClassType == typeof(byte[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vuint1, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read UINT1 with value " + vuint1);
                                break;
                            case FieldType.UINT2:
                                ushort vuint2 = bb.ReadUInt16();
                                if(fi.ClassType == typeof(ushort))
                                {
                                    fi.Field.SetValue(res, vuint2);
                                }
                                else if(fi.ClassType == typeof(uint))
                                {
                                    fi.Field.SetValue(res, vuint2);
                                }
                                else if(fi.ClassType == typeof(ushort[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vuint2, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read UINT2 with value " + vuint2);
                                break;
                            case FieldType.UINT4:
                                uint vuint4 = bb.ReadUInt32();
                                if(fi.ClassType == typeof(uint))
                                {
                                    fi.Field.SetValue(res, vuint4);
                                }
                                else if(fi.ClassType == typeof(uint[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vuint4, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read UINT4 with value " + vuint4);
                                break;
                            case FieldType.FP4:
                                float vfp4 = bb.ReadSingle();
                                if(fi.ClassType == typeof(float))
                                {
                                    fi.Field.SetValue(res, vfp4);
                                }
                                else if(fi.ClassType == typeof(float[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vfp4, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read FP4 with value " + vfp4);
                                break;
                            case FieldType.FP8:
                                double vfp8 = bb.ReadDouble();
                                if(fi.ClassType == typeof(double))
                                {
                                    fi.Field.SetValue(res, vfp8);
                                }
                                else if(fi.ClassType == typeof(double[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vfp8, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read FP8 with value " + vfp8);
                                break;
                            case FieldType.INTX:
                                int intxlen;
                                if(lenpvd == null)
                                {
                                    intxlen = fi.Length;
                                }
                                else
                                {
                                    intxlen = lenpvd(res, i);
                                }
                                if(intxlen <= 0 || intxlen >= 8)
                                {
                                    throw new RecordException("Wrong length of general integer " + fi.Field.Name + " in " + t.FullName);    
                                }
                                ulong vintx = 0;
                                if(si.Endianess == Endian.BIG)
                                {
                                    for(int j = 0; j < intxlen; j++)
                                    {
                                        vintx = vintx | (((ulong)bb.ReadByte()) << ((intxlen - 1 - j) * 8));
                                    }
                                }
                                else if(si.Endianess == Endian.LITTLE)
                                {
                                    for(int j = 0; j < intxlen; j++)
                                    {
                                        vintx = vintx | (((ulong)bb.ReadByte()) << (j * 8));
                                    }
                                }
                                if(fi.ClassType == typeof(ulong))
                                {
                                    fi.Field.SetValue(res, vintx);
                                }
                                else if(fi.ClassType == typeof(ulong[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vintx, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read INTX with value " + vintx);
                                break;
                            case FieldType.FIXSTR:
                                int fixstrlen;
                                if(lenpvd == null || lenpvd(res, i) < 0)
                                {
                                    fixstrlen = fi.Length;
                                }
                                else
                                {
                                    fixstrlen = lenpvd(res, i);
                                }
                                byte[] fixstrba = bb.ReadBytes(fixstrlen);
                                string vfixstr = Encoding.GetEncoding(fi.Encoding).GetString(fixstrba);
                                if(fi.ClassType == typeof(string))
                                {
                                    fi.Field.SetValue(res, vfixstr);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vfixstr, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read FIXSTR with value " + vfixstr);
                                break;
                            case FieldType.FIXSTRNULTERM:
                                int fixstrnultermlen;
                                if(lenpvd == null || lenpvd(res, i) < 0)
                                {
                                    fixstrnultermlen = fi.Length;
                                }
                                else
                                {
                                    fixstrnultermlen = lenpvd(res, i);
                                }
                                byte[] fixstrnultermba = bb.ReadBytes(fixstrnultermlen);
                                int actlen = 0;
                                while(actlen < fi.Length && fixstrnultermba[actlen] != 0)
                                {
                                    actlen++;
                                }
                                string vfixstrnulterm = Encoding.GetEncoding(fi.Encoding).GetString(fixstrnultermba, 0, actlen);
                                if(fi.ClassType == typeof(string))
                                {
                                    fi.Field.SetValue(res, vfixstrnulterm);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vfixstrnulterm, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read FIXSTRNULTERM with value " + vfixstrnulterm);
                                break;
                            case FieldType.VARSTR:
                                int varstrlen;
                                switch(fi.PrefixLength) {
    	                            case 0:
    	                            case 2:
    	                                varstrlen = bb.ReadInt16() & 0x0000FFFF;
    	                                break;
    	                            case 1:
    	                                varstrlen = bb.ReadByte() & 0x000000FF;
    	                                break;
    	                            case 4:
    	                                varstrlen = bb.ReadInt32();
    	                                break;
                                    default:
                                    	varstrlen = -1;
                                    	break;
                                }
                                if(varstrlen < 0)
                                {
                                    throw new RecordException("Wrong length of string " + fi.Field.Name + " in " + t.FullName + ": " + varstrlen);
                                }
                                byte[] varstrba = bb.ReadBytes(varstrlen);
                                string vvarstr = Encoding.GetEncoding(fi.Encoding).GetString(varstrba);
                                if(fi.ClassType == typeof(string))
                                {
                                    fi.Field.SetValue(res, vvarstr);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vvarstr, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read VARSTR with value " + vvarstr);
                                break;
                            case FieldType.VARFIXSTR:
                                int varfixstrlen;
                                switch(fi.PrefixLength) {
    	                            case 0:
    	                            case 2:
    	                                varfixstrlen = bb.ReadInt16() & 0x0000FFFF;
    	                                break;
    	                            case 1:
    	                                varfixstrlen = bb.ReadByte() & 0x000000FF;
    	                                break;
    	                            case 4:
    	                                varfixstrlen = bb.ReadInt32();
    	                                break;
                                    default:
                                    	varfixstrlen = -1;
                                    	break;
                                }
                                if(varfixstrlen < 0 || varfixstrlen  > fi.Length)
                                {
                                    throw new RecordException("Wrong length of string " + fi.Field.Name + " in " + t.FullName + ": " + varfixstrlen);
                                }
                                byte[] varfixstrba = bb.ReadBytes(varfixstrlen);
                                string vvarfixstr = Encoding.GetEncoding(fi.Encoding).GetString(varfixstrba);
                                if(fi.ClassType == typeof(string))
                                {
                                    fi.Field.SetValue(res, vvarfixstr);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vvarfixstr, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read VARFIXSTR with value " + vvarfixstr);
                                byte[] zero = bb.ReadBytes(fi.Length - varfixstrlen);
                                log.Debug("Skip " + zero.Length + " padding bytes");
                                break;
                            case FieldType.REMSTR:
                                int remstrlen = (int)(bb.BaseStream.Length - bb.BaseStream.Position);
                                byte[] remstrba = bb.ReadBytes(remstrlen);
                                string vremstr = Encoding.GetEncoding(fi.Encoding).GetString(remstrba);
                                if(fi.ClassType == typeof(string))
                                {
                                    fi.Field.SetValue(res, vremstr);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vremstr, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read REMSTR with value " + vremstr);
                                break;
                            case FieldType.BOOLEAN:
                                byte[] booleanba = bb.ReadBytes(fi.Length);
                                bool vboolean = booleanba[0] != 0;
                                if(fi.ClassType == typeof(bool))
                                {
                                    fi.Field.SetValue(res, vboolean);
                                }
                                else if(fi.ClassType == typeof(bool[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vboolean, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read BOOLEAN with value " + vboolean);
                                break;
                            case FieldType.BIT:
                                while(nbits < fi.Length)
                                {
                                    bitbuf = (bitbuf << 8) | (0xFFU & bb.ReadByte());
                                    nbits += 8;
                                }
                                int vbit = (int)(bitbuf >> (nbits - fi.Length));
                                bitbuf ^= (vbit << (nbits - fi.Length));
                                nbits -= fi.Length;
                                if(fi.ClassType == typeof(int))
                                {
                                    fi.Field.SetValue(res, vbit);
                                }
                                else if(fi.ClassType == typeof(int))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vbit, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read BIT with value " + vbit);
                                break;
                            case FieldType.JAVATIME:
                                DateTime vjavatime = TimeUtil.FromJavaTime(bb.ReadInt64());
                                if(fi.ClassType == typeof(DateTime))
                                {
                                    fi.Field.SetValue(res, vjavatime);
                                }
                                else if(fi.ClassType == typeof(DateTime[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vjavatime, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read JAVATIME with value " + vjavatime);
                                break;
                            case FieldType.UNIXTIME:
                                DateTime vunixtime = TimeUtil.FromUnixTime(bb.ReadInt32());
                                if(fi.ClassType == typeof(DateTime))
                                {
                                    fi.Field.SetValue(res, vunixtime);
                                }
                                else if(fi.ClassType == typeof(DateTime[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vunixtime, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read UNIXTIME with value " + vunixtime);
                                break;
                            case FieldType.VMSTIME:
                                DateTime vvmstime = TimeUtil.FromVMSTime(bb.ReadInt64());
                                if(fi.ClassType == typeof(DateTime))
                                {
                                    fi.Field.SetValue(res, vvmstime);
                                }
                                else if(fi.ClassType == typeof(DateTime[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vvmstime, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read VMSTIME with value " + vvmstime);
                                break;
                            case FieldType.PACKEDBCD:
                                int packedbcdlen;
                                if(lenpvd == null || lenpvd(res, i) < 0)
                                {
                                    packedbcdlen = fi.Length;
                                }
                                else
                                {
                                    packedbcdlen = lenpvd(res, i);
                                }
                                byte[] packedbcdba = bb.ReadBytes(packedbcdlen);
                                decimal vpackedbcd = BCDUtil.DecodePackedBCD(packedbcdba, fi.Decimals);
                                if(fi.ClassType == typeof(decimal))
                                {
                                    fi.Field.SetValue(res, vpackedbcd);
                                }
                                else if(fi.ClassType == typeof(decimal[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vpackedbcd, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read PACKEDBCD with value " + vpackedbcd);
                                break;
                            case FieldType.ZONEDBCD:
                                int zonedbcdlen;
                                if(lenpvd == null || lenpvd(res, i) < 0)
                                {
                                    zonedbcdlen = fi.Length;
                                }
                                else
                                {
                                    zonedbcdlen = lenpvd(res, i);
                                }
                                byte[] zonedbcdba = bb.ReadBytes(zonedbcdlen);
                                decimal vzonedbcd = BCDUtil.DecodeZonedBCD(zonedbcdba, fi.Zone, fi.Decimals);
                                if(fi.ClassType == typeof(decimal))
                                {
                                    fi.Field.SetValue(res, vzonedbcd);
                                }
                                else if(fi.ClassType == typeof(decimal[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vzonedbcd, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read ZONEDBCD with value " + vzonedbcd);
                                break;
                            case FieldType.VAXFP4:
                                float vvaxfp4 = BitConverter.ToSingle(BitConverter.GetBytes(VAXFloatUtil.F2S(bb.ReadUInt32())), 0);
                                if(fi.ClassType == typeof(float))
                                {
                                    fi.Field.SetValue(res, vvaxfp4);
                                }
                                else if(fi.ClassType == typeof(float[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vvaxfp4, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read VAXFP4 with value " + vvaxfp4);
                                break;
                            case FieldType.VAXFP8:
                                double vvaxfp8 = BitConverter.ToDouble(BitConverter.GetBytes(VAXFloatUtil.G2T(bb.ReadUInt64())), 0);
                                if(fi.ClassType == typeof(double))
                                {
                                    fi.Field.SetValue(res, vvaxfp8);
                                }
                                else if(fi.ClassType == typeof(double[]))
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(vvaxfp8, ix);
                                }
                                else
                                {
                                    BadConversion(fi.StructType, fi.ClassType, t);
                                }
                                log.Debug("Read VAXFP8 with value " + vvaxfp8);
                                break;
                            case FieldType.STRUCT:
                                if(!fi.ClassType.IsArray)
                                {
                                    fi.Field.SetValue(res, Read<object>(fi.ClassType));
                                }
                                else
                                {
                                    ((Array)fi.Field.GetValue(res)).SetValue(Read<object>(fi.ClassType), ix);
                                }
                                break;
                            default:
                                BadConversion(fi.StructType, fi.ClassType, t);
                                break;
                        }
                    }
                    // the field was a selector
                    if(fi.Selects != null)
                    {
    					// lookup class and padding
    					SubClassAndPad scp;
    					if(infpvd != null && infpvd(fi.Field.GetValue(res)) != null) {
    					    scp = fi.Selects[(int)infpvd(fi.Field.GetValue(res))];
    					} else {
    						scp = fi.Selects[(int)fi.Field.GetValue(res)];
    					}
                        if(scp == null)
                        {
                            throw new RecordException(res.GetType().FullName + " " + fi.Field.Name + " has invalid selector value: " + fi.Field.GetValue(res));
                        }
                        Type tt = scp.SubClass;
                        // if class different from current (to avoid infinite recursion)
                        if(tt != t)
                        {
                            // go back to start of bytes
                            bb.BaseStream.Position = mark;
                            // read the sub class
                            T o = (T)Read<object>(tt, lenpvd, maxlenpvd, elmpvd, infpvd);
                            log.Debug("Restarting for sub class " + tt.FullName);
                            // read select pad bytes
                            byte[] zero = new byte[scp.Pad];
                            bb.Read(zero, 0, zero.Length);
                            log.Debug("Skip " + zero.Length + " padding bytes");
                            return o;
                        }
                    }
                }
                // if necessary read new record pad bytes
                if(si.Endpad)
                {
                    int nskip = StructInfo.CalculateEndPad((int)bb.BaseStream.Position, si.Alignment, si.Fields);
                    if(nskip > 0)
                    {
                        bb.ReadBytes(nskip);
                        log.Debug("Skip " + nskip + " end-padding bytes");
                    }
                }
                return res;
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch(Exception e)
            {
                throw new RecordException("Exception", e);
            }
        }
        /// <summary>
        /// More records available.
        /// </summary>
        public bool More
        {
            get { return bb.BaseStream.Position < bb.BaseStream.Length; }
        }
        /// <summary>
        /// Number of remaining bytes.
        /// </summary>
        public int Remaining
        {
            get { return (int)(bb.BaseStream.Length - bb.BaseStream.Position); }
        }
        /// <summary>
        /// Update byte array to read from by adding bytes.
        /// </summary>
        /// <param name="ba">Bytes to add.</param>
    	public void Update(byte[] ba)
    	{
    	    byte[] rem = bb.ReadBytes(Remaining);
    		byte[] remba = new byte[rem.Length + ba.Length];
    		Array.Copy(rem, 0, remba, 0, rem.Length);
    		Array.Copy(ba, 0, remba, rem.Length, ba.Length);
            bb = new EndianBinaryReader(new MemoryStream(remba));
            log.Debug("StructReader updated with byte array of length " + ba.Length);
            if(log.IsDebugEnabled)
            {
                log.Debug("Byte array:" + LogHelper.ByteArrayToString(remba));
            }
   	    }
        private void BadConversion(FieldType ft, Type clz, Type st)
        {
            throw new RecordException("Can not convert from " + ft.ToString() + " to " + clz.FullName + " in " + st.FullName);
        }
    }
    /// <summary>
    /// Class StructWriter writes a .NET object to a byte array as a native struct.
    /// </summary>
    public class StructWriter
    {
        private static ILog log = LogManager.GetLogger(typeof(StructWriter));
        private const int DEFAULT_BUFSIZ = 10000;
        private EndianBinaryWriter bb;
        private byte[] backing;
        /// <summary>
        /// Construct instance of StructWriter with default buffer size.
        /// </summary>
        public StructWriter() : this(DEFAULT_BUFSIZ)
        {
        }
        /// <summary>
        /// Construct instance of StructWriter.
        /// </summary>
        /// <param name="bufsiz">Size of byte array to write to.</param>
        public StructWriter(int bufsiz)
        {
            backing = new byte[bufsiz];
            bb = new EndianBinaryWriter(new MemoryStream(backing));
            log.Debug("StructWriter initialized with buffersize " + bufsiz);
        }
        /// <summary>
        /// Write.
        /// </summary>
        /// <param name="o">Object to write.</param>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public void Write(object o)
        {
            Write(o, null, null, null, null);
        }
        /// <summary>
        /// Write.
        /// </summary>
        /// <param name="o">Object to write.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given -</param>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public void Write(object o, LengthProvider lenpvd)
        {
            Write(o, lenpvd, null, null, null);
        }
        /// <summary>
        /// Write.
        /// </summary>
        /// <param name="o">Object to write.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given -</param>
        /// <param name="maxlenpvd">Supplies max length for fields where it is not given.</param>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public void Write(object o, LengthProvider lenpvd, MaxLengthProvider maxlenpvd)
        {
            Write(o, lenpvd, maxlenpvd, null, null);
        }
        /// <summary>
        /// Write.
        /// </summary>
        /// <param name="o">Object to write.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given -</param>
        /// <param name="maxlenpvd">Supplies max length for fields where it is not given.</param>
        /// <param name="elmpvd">Supplies elements for fields where it is not given.</param>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public void Write(object o, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd)
        {
            Write(o, lenpvd, maxlenpvd, elmpvd, null);
        }
        /// <summary>
        /// Write.
        /// </summary>
        /// <param name="o">Object to write.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given -</param>
        /// <param name="maxlenpvd">Supplies max length for fields where it is not given.</param>
        /// <param name="elmpvd">Supplies elements for fields where it is not given.</param>
        /// <param name="infpvd">Supplies selector converter for fields where it is needed.</param>
        /// <exception cref="RecordException">If impossible to convert between types in class and struct.</exception>
        public void Write(object o, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd, ConvertSelector infpvd)
        {
            try {
                long bitbuf = 0;
                int nbits = 0;
                int selpad = 0;
                byte[] zero;
                Type t = o.GetType();
                log.Debug("Writing class " + t.FullName);
                StructInfo si = StructInfoCache.Analyze(t);
                bb.CurrentEndian = si.Endianess;
                for(int i = 0; i < si.Fields.Count; i++)
                {
                    FieldInfo fi = si.Fields[i];
                    int npad = StructInfo.CalculatePad((int)bb.BaseStream.Position, si.Alignment, fi);
                    if(npad > 0)
                    {
                        bb.Write(new byte[npad]);
                        log.Debug("Write " + npad + " padding bytes");
                    }
                    int nelm;
    			    if(elmpvd == null || elmpvd(o, i) < 0)
    			    {
    			        if(fi.Countprefix <= 0)
    			        {
    			    	    nelm = fi.Elements;
    			        }
    			        else
    			        {
    			            nelm = ((Array)fi.Field.GetValue(o)).GetLength(0);
    			    		switch(fi.Countprefix) {
    			    			case 1:
    			    				bb.Write((byte)nelm);
    			    				break;
    			    			case 2:
    			    				bb.Write((short)nelm);
    			    				break;
    			    			case 4:
    			    				bb.Write(nelm);
    			    				break;
    			    			default:
    			    				throw new RecordException("Wrong length of count prefix " + fi.Field.Name + " in " + t.Name + ": " + fi.Countprefix);
    			    		}
    			        }
    			    }
    			    else
    			    {
    			    	nelm = elmpvd(o, i);
    			    }
                    for(int ix = 0; ix < nelm; ix++)
                    {
                        switch(fi.StructType)
                        {
                            case FieldType.INT1:
                                sbyte vint1;
                                if(fi.ClassType == typeof(sbyte))
                                {
                                    vint1 = (sbyte)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(short))
                                {
                                    vint1 = (sbyte)(short)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(int))
                                {
                                    vint1 = (sbyte)(int)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(sbyte[]))
                                {
                                    vint1 = (sbyte)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vint1 = 0;
                                }
                                bb.Write(vint1);
                                log.Debug("Write INT1 with value " + vint1);
                                break;
                            case FieldType.INT2:
                                short vint2;
                                if(fi.ClassType == typeof(short))
                                {
                                    vint2 = (short)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(int))
                                {
                                    vint2 = (short)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(short[]))
                                {
                                    vint2 = (short)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vint2 = 0;
                                }
                                bb.Write(vint2);
                                log.Debug("Write INT2 with value " + vint2);
                                break;
                            case FieldType.INT4:
                                int vint4;
                                if(fi.ClassType == typeof(int))
                                {
                                    vint4 = (int)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(int[]))
                                {
                                    vint4 = (int)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vint4 = 0;
                                }
                                bb.Write(vint4);
                                log.Debug("Write INT4 with value " + vint4);
                                break;
                            case FieldType.INT8:
                                long vint8;
                                if(fi.ClassType == typeof(long))
                                {
                                    vint8 = (long)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(long[]))
                                {
                                    vint8 = (long)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vint8 = 0;
                                }
                                bb.Write(vint8);
                                log.Debug("Write INT8 with value " + vint8);
                                break;
                            case FieldType.UINT1:
                                byte vuint1;
                                if(fi.ClassType == typeof(byte))
                                {
                                    vuint1 = (byte)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(uint))
                                {
                                    vuint1 = (byte)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(byte[]))
                                {
                                    vuint1 = (byte)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vuint1 = 0;
                                }
                                bb.Write(vuint1);
                                log.Debug("Write UINT1 with value " + vuint1);
                                break;
                            case FieldType.UINT2:
                                ushort vuint2;
                                if(fi.ClassType == typeof(ushort))
                                {
                                    vuint2 = (ushort)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(uint))
                                {
                                    vuint2 = (ushort)fi.Field.GetValue(o);    
                                }
                                else if(fi.ClassType == typeof(uint[]))
                                {
                                    vuint2 = (ushort)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vuint2 = 0;
                                }
                                bb.Write(vuint2);
                                log.Debug("Write UINT2 with value " + vuint2);
                                break;
                            case FieldType.UINT4:
                                uint vuint4;
                                if(fi.ClassType == typeof(uint))
                                {
                                    vuint4 = (uint)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(uint[]))
                                {
                                    vuint4 = (uint)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vuint4 = 0;
                                }
                                bb.Write(vuint4);
                                log.Debug("Write UINT4 with value " + vuint4);
                                break;
                            case FieldType.FP4:
                                float vfp4;
                                if(fi.ClassType == typeof(float))
                                {
                                    vfp4 = (float)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(float[]))
                                {
                                    vfp4 = (float)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vfp4 = 0;
                                }
                                bb.Write(vfp4);
                                log.Debug("Write FP4 with value " + vfp4);
                                break;
                            case FieldType.FP8:
                                double vfp8;
                                if(fi.ClassType == typeof(double))
                                {
                                    vfp8 = (double)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(double[]))
                                {
                                    vfp8 = (double)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vfp8 = 0;
                                }
                                bb.Write(vfp8);
                                log.Debug("Write FP8 with value " + vfp8);
                                break;
                            case FieldType.INTX:
                                int intxlen;
                                if(lenpvd == null)
                                {
                                    intxlen = fi.Length;
                                }
                                else
                                {
                                    intxlen = lenpvd(o, i);
                                }
                                ulong vintx;
                                if(fi.ClassType == typeof(ulong))
                                {
                                    vintx = (ulong)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(ulong[]))
                                {
                                    vintx = (ulong)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vintx = 0;
                                }
                                if(intxlen > 0 && intxlen < 8)
                                {
                                    if(si.Endianess == Endian.BIG)
                                    {
                                        for(int j = 0; j < intxlen; j++)
                                        {
                                            bb.Write(((byte)((vintx >> ((intxlen - 1 - j) * 8)) & 0x00FF)));
                                        }
                                    }
                                    else if(si.Endianess == Endian.LITTLE)
                                    {
                                        for(int j = 0; j < intxlen; j++)
                                        {
                                            bb.Write(((byte)((vintx >> (j * 8)) & 0x00FF)));
                                        }
                                    }
                                }
                                else
                                {
                                    throw new RecordException("Wrong length of general integer " + fi.Field.Name + " in " + t.FullName);
                                }
                                log.Debug("Write INTX with value " + vintx);
                                break;
                            case FieldType.FIXSTR:
                                int fixstrlen;
                                if(lenpvd == null || lenpvd(o, i) < 0)
                                {
                                    fixstrlen = fi.Length;
                                }
                                else
                                {
                                    fixstrlen = lenpvd(o, i);
                                }
                                string vfixstr;
                                if(fi.ClassType == typeof(string))
                                {
                                    vfixstr = (string)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    vfixstr = (string)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vfixstr = null;
                                }
                                byte[] fixstrba = Encoding.GetEncoding(fi.Encoding).GetBytes(vfixstr);
                                if(fixstrba.Length == fixstrlen)
                                {
                                    bb.Write(fixstrba);
                                }
                                else
                                {
                                    throw new RecordException("Wrong length of string " + fi.Field.Name + " in " + t.FullName);
                                }
                                log.Debug("Write FIXSTR with value " + vfixstr);
                                break;
                            case FieldType.FIXSTRNULTERM:
                                int fixstrnultermlen;
                                if(lenpvd == null || lenpvd(o, i) < 0)
                                {
                                    fixstrnultermlen = fi.Length;
                                }
                                else
                                {
                                    fixstrnultermlen = lenpvd(o, i);
                                }
                                string vfixstrnulterm;
                                if(fi.ClassType == typeof(string))
                                {
                                    vfixstrnulterm = (string)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    vfixstrnulterm = (string)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vfixstrnulterm = null;
                                }
                                byte[] fixstrnultermba = Encoding.GetEncoding(fi.Encoding).GetBytes(vfixstrnulterm);
                                if(fixstrnultermba.Length <= fixstrnultermlen)
                                {
                                    bb.Write(fixstrnultermba);
                                    zero = new byte[fixstrnultermlen - fixstrnultermba.Length];
                                    bb.Write(zero);
                                }
                                else
                                {
                                    throw new RecordException("Wrong length of string " + fi.Field.Name + " in " + t.FullName);
                                }
                                log.Debug("Write FIXSTRNULTERM with value " + vfixstrnulterm);
                                break;
                            case FieldType.VARSTR:
                                string vvarstr;
                                if(fi.ClassType == typeof(string))
                                {
                                    vvarstr = (string)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    vvarstr = (string)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vvarstr = null;
                                }
                                byte[] varstrba = Encoding.GetEncoding(fi.Encoding).GetBytes(vvarstr);
                                if((fi.PrefixLength == 0 || fi.PrefixLength == 2) && varstrba.Length < 32768)
                                {
                                    bb.Write((short)varstrba.Length);
                                    bb.Write(varstrba);
                                }
                                else if(fi.PrefixLength == 1 && varstrba.Length < 128)
                                {
                                    bb.Write((byte)varstrba.Length);
                                    bb.Write(varstrba);
                                }
                                else if(fi.PrefixLength == 4)
                                {
                                    bb.Write((int)varstrba.Length);
                                    bb.Write(varstrba);
                                }
                                else
                                {
                                    throw new RecordException("Wrong length of string " + fi.Field.Name + " in " + t.FullName);
                                }
                                log.Debug("Write VARSTR with value " + vvarstr);
                                break;
                            case FieldType.VARFIXSTR:
                                string vvarfixstr;
                                if(fi.ClassType == typeof(string))
                                {
                                    vvarfixstr = (string)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    vvarfixstr = (string)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vvarfixstr = null;
                                }
                                byte[] varfixstrba = Encoding.GetEncoding(fi.Encoding).GetBytes(vvarfixstr);
                                if(varfixstrba.Length > fi.Length)
                                {
                                    throw new RecordException("Wrong length of string " + fi.Field.Name + " in " + t.FullName);
                                }
                                else if((fi.PrefixLength == 0 || fi.PrefixLength == 2) && varfixstrba.Length < 32768)
                                {
                                    bb.Write((short)varfixstrba.Length);
                                    bb.Write(varfixstrba);
                                }
                                else if(fi.PrefixLength == 1 && varfixstrba.Length < 128)
                                {
                                    bb.Write((byte)varfixstrba.Length);
                                    bb.Write(varfixstrba);
                                }
                                else if(fi.PrefixLength == 4)
                                {
                                    bb.Write((int)varfixstrba.Length);
                                    bb.Write(varfixstrba);
                                }
                                else 
                                {
                                    throw new RecordException("Wrong length of string " + fi.Field.Name + " in " + t.FullName);
                                }
                                log.Debug("Write VARFIXSTR with value " + vvarfixstr);
                                zero = new byte[fi.Length - varfixstrba.Length];
                                bb.Write(zero);
                                log.Debug("Write " + zero.Length + " padding bytes");
                                break;
                            case FieldType.REMSTR:
                                string vremstr;
                                if(fi.ClassType == typeof(string))
                                {
                                    vremstr = (string)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(string[]))
                                {
                                    vremstr = (string)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vremstr = null;
                                }
                                byte[] remstrba = Encoding.GetEncoding(fi.Encoding).GetBytes(vremstr);
                                bb.Write(remstrba);
                                log.Debug("Write REMSTR with value " + vremstr);
                                break;
                            case FieldType.BOOLEAN:
                                bool vboolean;
                                if(fi.ClassType == typeof(bool))
                                {
                                    vboolean = (bool)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(bool[]))
                                {
                                    vboolean = (bool)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vboolean = false;
                                }
                                byte[] booleanba = new byte[fi.Length];
                                booleanba[0] =  vboolean ? (byte)1 : (byte)0;
                                bb.Write(booleanba);
                                log.Debug("Write BOOLEAN with value " + vboolean);
                                break;
                            case FieldType.BIT:
                                int vbit;
                                if(fi.ClassType == typeof(int))
                                {
                                    vbit = (int)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(int))
                                {
                                    vbit = (int)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vbit = 0;
                                }
                                bitbuf = bitbuf << fi.Length | (uint)vbit;
                                nbits += fi.Length;
                                while(nbits >= 8)
                                {
                                    byte tmp = (byte)(bitbuf >> (nbits - 8));
                                    bb.Write(tmp);
                                    bitbuf ^= (tmp << (nbits - 8));
                                    nbits -= 8;
                                }
                                log.Debug("Write BIT with value " + vbit);
                                break;
                            case FieldType.JAVATIME:
                                DateTime vjavatime;
                                if(fi.ClassType == typeof(DateTime))
                                {
                                    vjavatime = (DateTime)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(DateTime[]))
                                {
                                    vjavatime = (DateTime)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vjavatime = DateTime.MinValue;
                                }
                                bb.Write(TimeUtil.ToJavaTime(vjavatime));
                                log.Debug("Write JAVATIME with value " + vjavatime);
                                break;
                            case FieldType.UNIXTIME:
                                DateTime vunixtime;
                                if(fi.ClassType == typeof(DateTime))
                                {
                                    vunixtime = (DateTime)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(DateTime[]))
                                {
                                    vunixtime = (DateTime)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vunixtime = DateTime.MinValue;
                                }
                                bb.Write(TimeUtil.ToUnixTime(vunixtime));
                                log.Debug("Write UNIXTIME with value " + vunixtime);
                                break;
                            case FieldType.VMSTIME:
                                DateTime vvmstime;
                                if(fi.ClassType == typeof(DateTime))
                                {
                                    vvmstime = (DateTime)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(DateTime[]))
                                {
                                    vvmstime = (DateTime)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vvmstime = DateTime.MinValue;
                                }
                                bb.Write(TimeUtil.ToVMSTime(vvmstime));
                                log.Debug("Write VMSTIME with value " + vvmstime);
                                break;
                            case FieldType.PACKEDBCD:
                                int packedbcdlen;
                                if(lenpvd == null || lenpvd(o, i) < 0)
                                {
                                    packedbcdlen = fi.Length;
                                }
                                else
                                {
                                    packedbcdlen = lenpvd(o, i);
                                }
                                decimal vpackedbcd;
                                if(fi.ClassType == typeof(decimal))
                                {
                                    vpackedbcd = (decimal)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(decimal[]))
                                {
                                    vpackedbcd = (decimal)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vpackedbcd = 0;
                                }
                                bb.Write(BCDUtil.EncodePackedBCD(vpackedbcd, fi.Decimals, packedbcdlen));
                                log.Debug("Write PACKEDBCD with value " + vpackedbcd);
                                break;
                            case FieldType.ZONEDBCD:
                                int zonedbcdlen;
                                if(lenpvd == null || lenpvd(o, i) < 0)
                                {
                                    zonedbcdlen = fi.Length;
                                }
                                else 
                                {
                                    zonedbcdlen = lenpvd(o, i);
                                }
                                decimal vzonedbcd;
                                if(fi.ClassType == typeof(decimal))
                                {
                                    vzonedbcd = (decimal)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(decimal[]))
                                {
                                    vzonedbcd = (decimal)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vzonedbcd = 0;
                                }
                                bb.Write(BCDUtil.EncodeZonedBCD(vzonedbcd, fi.Zone, fi.Decimals, zonedbcdlen));
                                log.Debug("Write ZONEDBCD with value " + vzonedbcd);
                                break;
                            case FieldType.VAXFP4:
                                float vvaxfp4;
                                if(fi.ClassType == typeof(float))
                                {
                                    vvaxfp4 = (float)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(float[]))
                                {
                                    vvaxfp4 = (float)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vvaxfp4 = 0;
                                }
                                bb.Write(VAXFloatUtil.S2F(BitConverter.ToUInt32(BitConverter.GetBytes(vvaxfp4), 0)));
                                log.Debug("Write VAXFP4 with value " + vvaxfp4);
                                break;
                            case FieldType.VAXFP8:
                                double vvaxfp8;
                                if(fi.ClassType == typeof(double))
                                {
                                    vvaxfp8 = (double)fi.Field.GetValue(o);
                                }
                                else if(fi.ClassType == typeof(double[]))
                                {
                                    vvaxfp8 = (double)((Array)fi.Field.GetValue(o)).GetValue(ix);
                                }
                                else
                                {
                                    BadConversion(fi.ClassType, fi.StructType, t);
                                    vvaxfp8 = 0;
                                }
                                bb.Write(VAXFloatUtil.T2G(BitConverter.ToUInt64(BitConverter.GetBytes(vvaxfp8), 0)));
                                log.Debug("Write VAXFP8 with value " + vvaxfp8);
                                break;
                            case FieldType.STRUCT:
                                Write(fi.Field.GetValue(o));
                                break;
                            default:
                                BadConversion(fi.ClassType, fi.StructType, t);
                                break;
                        }
                    }
                    // the field was a selector
                    if(fi.Selects != null)
                    {
    					// lookup class and padding
    					SubClassAndPad scp;
    					if(infpvd != null && infpvd(fi.Field.GetValue(o)) != null) {
    					    scp = fi.Selects[(int)infpvd(fi.Field.GetValue(o))];
    					} else {
    						scp = fi.Selects[(int)fi.Field.GetValue(o)];
    					}
                        if(scp == null)
                        {
                            throw new RecordException(o.GetType().FullName + " " + fi.Field.Name + " has invalid selector value: " + fi.Field.GetValue(o));
                        }
                        // add select padding
                        selpad += scp.Pad;
                    }
                }
                // write select pad bytes
                zero = new byte[selpad];
                bb.Write(zero);
                log.Debug("Write " + zero.Length + " padding bytes");
                // if necessary write new record pad bytes
                if(si.Endpad)
                {
                    int npad = StructInfo.CalculateEndPad((int)bb.BaseStream.Position, si.Alignment, si.Fields);
                    if(npad > 0)
                    {
                        bb.Write(new byte[npad]);
                        log.Debug("Write " + npad + " end-padding bytes");
                    }
                }
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch(Exception e)
            {
                throw new RecordException("Exception", e);
            }
        }
        /// <summary>
        /// Get bytes.
        /// </summary>
        /// <returns>The resulting byte array.</returns>
        public byte[] GetBytes()
        {
            int n = (int)bb.BaseStream.Position;
            byte[] res = new byte[n];
            Array.Copy(backing, res, n);
            log.Debug("Returning byte array of length " + res.Length);
            if(log.IsDebugEnabled)
            {
                log.Debug("Byte array:" + LogHelper.ByteArrayToString(res));
            }
            return res;
        }
    	/// <summary>
    	/// The length.
    	/// </summary>
    	public int Length
    	{
    	    get { return (int)bb.BaseStream.Position; }
    	}
    	/// <summary>
    	/// Extend capacity.
    	/// </summary>
    	/// <param name="newbufsiz">New size of byte array to write to.</param>
    	public void Extend(int newbufsiz)
    	{
            int n = (int)bb.BaseStream.Position;
            byte[] buf = new byte[n];
            Array.Copy(backing, buf, n);
    	    backing = new byte[newbufsiz];
            bb = new EndianBinaryWriter(new MemoryStream(backing));
    		bb.Write(buf);
    		log.Debug("StructWriter extended to buffersize " + newbufsiz);
    		
    	}
        private void BadConversion(Type clz, FieldType ft, Type st)
        {
            throw new RecordException("Can not convert from " + clz.FullName + " to " + ft.ToString() + " in " + st.FullName);
        }
    }
    /// <summary>
    /// Class StructReaderStream reads a .NET object from an input stream containing a native struct.
    /// </summary>
    public class StructReaderStream
    {
    	private Stream stm;
    	private int elmsiz;
    	private int noelm;
    	private StructReader sr;
        /// <summary>
        /// Create instance of StructReaderStream.
        /// </summary>
        /// <param name="stm">Input stream.</param>
        /// <param name="elmsiz">Fixed size structs: size struct, variable size structs: maximum size struct.</param>
        /// <param name="noelm">Number of structs in buffer.</param>
    	public StructReaderStream(Stream stm, int elmsiz, int noelm)
    	{
    		this.stm = stm;
    		this.elmsiz = elmsiz;
    		this.noelm = noelm;
    		this.sr = new StructReader(new byte[0]);
    	}
    	/// <summary>
    	/// Read.
    	/// </summary>
    	/// <param name="t">Type of what to read.</param>
    	/// <returns>Object read.</returns>
    	public T Read<T>(Type t) where T : class, new()
    	{
    		Check();
    		return sr.Read<T>(t);
    	}
    	/// <summary>
    	/// Read.
    	/// </summary>
    	/// <param name="t">Type of what to read.</param>
        /// <param name="lenpvd">Supplies length for fields where it is not given.</param>
        /// <param name="maxlenpvd">Supplies max length for fields where it is not given.</param>
        /// <param name="elmpvd">Supplies elements for fields where it is not given.</param>
    	/// <returns>Object read.</returns>
    	public T Read<T>(Type t, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd) where T : class, new()
    	{
    		Check();
    		return sr.Read<T>(t, lenpvd, maxlenpvd, elmpvd);
    	}
    	/// <summary>
    	/// More records available (true=more, false=no more).
    	/// </summary>
    	public bool More
    	{
    	    get
    	    {
    		  Check();
    		  return sr.More;
    	    }
    	}
    	/// <summary>
    	/// Close underlying InputStream.
    	/// </summary>
    	public void Close()
    	{
    		stm.Close();
    	}
    	private void Check() 
    	{
    		if(sr.Remaining < elmsiz) {
    			byte[] b = Fill(noelm * elmsiz);
    			sr.Update(b);
    		}
    	}
        private byte[] Fill(int siz) 
        {
            byte[] b = new byte[siz];
            int n;
            int ix = 0;
            while((n = stm.Read(b, ix, b.Length - ix)) > 0) {
                ix += n;
            }
            if(ix < siz) {
                byte[] tmp = new byte[ix];
                Array.Copy(b, tmp, ix);
                b = tmp;
            }
            return b;
        }
    }
}
