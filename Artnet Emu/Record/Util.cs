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

namespace Vajhoej.Record
{
    /// <summary>
    /// Utility class to process lists and to work with files instead of byte arrays.
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Process object.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="o">Object.</param>
        public delegate void ObjectHandlerProcess<T>(T o);
        /// <summary>
        /// Convert object.
        /// </summary>
        /// <typeparam name="T1">From type.</typeparam>
        /// <typeparam name="T2">To type.</typeparam>
        /// <param name="o">From object.</param>
        /// <returns>To object.</returns>
        public delegate T2 TransformerConvert<T1,T2>(T1 o);
        private const int BUFSIZ = 1000;
        /// <summary>
        /// Read array of struct in byte array into List of objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="b">Byte array.</param>
        /// <returns>List of objects.</returns>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        public static IList<T> ReadAll<T>(Type t, byte[] b) where T : class, new()
        {
            StructReader sr = new StructReader(b);
            IList<T> res = new List<T>();
            while(sr.More)
            {
                res.Add(sr.Read<T>(t));
            }
            return res;
        }
        /// <summary>
        /// Write List of objects into array of struct in byte array.
        /// Note: does not work with stucts containing VARSTR fields and STRUCT fields. 
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="lst">List of objects.</param>
        /// <returns>Byte array.</returns>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        public static byte[] WriteAll<T>(Type t, IList<T> lst)
        {
            int siz = lst.Count * CalcSize<T>(t);
            StructWriter sw = new StructWriter(siz);
            foreach(T o in lst)
            {
                sw.Write(o);
            }
            return sw.GetBytes();
        }
        /// <summary>
        /// Read array of struct in stream into List of objects. 
        /// Note: does not work with stucts containing VARSTR fields and STRUCT fields. 
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="stm">Stream.</param>
        /// <param name="lst">List of objects.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void ReadAll<T>(Type t, Stream stm, IList<T> lst) where T : class, new()
        {
            ReadAll<T>(t, stm, delegate(T o) { lst.Add(o); });
        }
        /// <summary>
        /// Write List of objects into array of struct in stream. 
        /// Note: does not work with stucts containing VARSTR fields and STRUCT fields. 
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="lst">list of object.</param>
        /// <param name="stm">Stream.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void WriteAll<T>(Type t, IList<T> lst, Stream stm)
        {
            for(int i = 0; i < lst.Count; i += BUFSIZ)
            {
                byte[] b = WriteAll(t, ((List<T>)lst).GetRange(i, Math.Min(BUFSIZ, lst.Count - i)));
                stm.Write(b, 0, b.Length);
            }
        }
        /// <summary>
        /// Read array of struct in stream and processes them by handler. 
        /// Note: does not work with stucts containing VARSTR fields and STRUCT fields. 
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="stm">Stream.</param>
        /// <param name="ohp">Processor of objects.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void ReadAll<T>(Type t, Stream stm, ObjectHandlerProcess<T> ohp) where T : class, new()
        {
           	StructReaderStream srs = new StructReaderStream(stm, CalcSize<T>(t), BUFSIZ);
        	while(srs.More)
        	{
        		ohp(srs.Read<T>(t));
        	}
        }
        /// <summary>
        /// Convert array of struct in bytes into array of struct in bytes. 
        /// </summary>
        /// <typeparam name="T1">From type.</typeparam>
        /// <typeparam name="T2">To type.</typeparam>
        /// <param name="t1">From type.</param>
        /// <param name="b">From byte array.</param>
        /// <param name="t2">To type.</param>
        /// <param name="cvt">Converter of objects.</param>
        /// <returns>To byte array.</returns>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        public static byte[] CopyAll<T1,T2> (Type t1, byte[] b, Type t2, TransformerConvert<T1,T2> cvt) where T1 : class, new()
        {
            IList<T1> lst1 = ReadAll<T1>(t1, b);
            IList<T2> lst2 = new List<T2>();
            foreach(T1 o in lst1)
            {
                lst2.Add(cvt(o));
            }
            return WriteAll(t2, lst2);
        }
        /// <summary>
        /// Convert array of struct in stream into array of struct in stream. 
        /// </summary>
        /// <typeparam name="T1">From type.</typeparam>
        /// <typeparam name="T2">To type.</typeparam>
        /// <param name="t1">From type.</param>
        /// <param name="instm">From stream.</param>
        /// <param name="t2">To type.</param>
        /// <param name="outstm">To stream.</param>
        /// <param name="cvt">Converter of objects.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void CopyAll<T1,T2>(Type t1, Stream instm, Type t2, Stream outstm, TransformerConvert<T1,T2> cvt) where T1 : class, new()
        {
            ReadAll<T1>(t1, instm, delegate(T1 o)
                                   {
                                       StructWriter sw = new StructWriter();
                                       sw.Write(cvt(o));
                                       byte[] b = sw.GetBytes();
                                       outstm.Write(b, 0, b.Length);
                                   });
        }
        private static int CalcSize<T>(Type t)
        {
            StructInfo si = StructInfoCache.Analyze(t);
            if(si.FixedLength)
            {
                return si.Length;
            }
            else
            {
                throw new RecordException("Cannot calculate size of " + t.FullName + " because it contains at least one field of type VARSTR");
            }
        }
    }
    /// <summary>
    /// Utility class to process lists and to work with files instead of byte arrays
    /// trying to work even with variable length structs.
    /// </summary>
    public class Util2
    {
        /// <summary>
        /// Process object.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="o">Object.</param>
        public delegate void ObjectHandlerProcess<T>(T o);
        /// <summary>
        /// Convert object.
        /// </summary>
        /// <typeparam name="T1">From type.</typeparam>
        /// <typeparam name="T2">To type.</typeparam>
        /// <param name="o">From object.</param>
        /// <returns>To object.</returns>
        public delegate T2 TransformerConvert<T1,T2>(T1 o);
        private const int BUFSIZ = 1000;
        /// <summary>
        /// Read array of struct in byte array into List of objects.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="b">Byte array.</param>
        /// <param name="lenpvd">Length provider.</param>
        /// <param name="maxlenpvd">Max length provider.</param>
        /// <param name="elmpvd">Element provider.</param>
        /// <returns>List of objects.</returns>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        public static IList<T> ReadAll<T>(Type t, byte[] b, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd) where T : class, new()
        {
            StructReader sr = new StructReader(b);
            IList<T> res = new List<T>();
            while(sr.More)
            {
                res.Add(sr.Read<T>(t, lenpvd, maxlenpvd, elmpvd));
            }
            return res;
        }
        /// <summary>
        /// Write List of objects into array of struct in byte array.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="lst">List of objects.</param>
        /// <param name="lenpvd">Length provider.</param>
        /// <param name="maxlenpvd">Max length provider.</param>
        /// <param name="elmpvd">Element provider.</param>
        /// <returns>Byte array.</returns>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        public static byte[] WriteAll<T>(Type t, IList<T> lst, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd)
        {
            int maxelm = 16;
            StructWriter sw = new StructWriter(maxelm * CalcSize<T>(t, lenpvd, maxlenpvd, elmpvd));
            foreach(T o in lst)
            {
                sw.Write(o, lenpvd, maxlenpvd, elmpvd);
                if(sw.Length + CalcSize<T>(t, lenpvd, maxlenpvd, elmpvd) > maxelm * CalcSize<T>(t, lenpvd, maxlenpvd, elmpvd))
                {
                	maxelm *= 2;
                	sw.Extend(maxelm * CalcSize<T>(t, lenpvd, maxlenpvd, elmpvd));
                }
            }
            return sw.GetBytes();
        }
        /// <summary>
        /// Read array of struct in stream into List of objects. 
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="stm">Stream.</param>
        /// <param name="lst">List of objects.</param>
        /// <param name="lenpvd">Length provider.</param>
        /// <param name="maxlenpvd">Max length provider.</param>
        /// <param name="elmpvd">Element provider.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void ReadAll<T>(Type t, Stream stm, IList<T> lst, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd) where T : class, new()
        {
            ReadAll<T>(t, stm, delegate(T o) { lst.Add(o); }, lenpvd, maxlenpvd, elmpvd);
        }
        /// <summary>
        /// Write List of objects into array of struct in stream. 
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="lst">list of object.</param>
        /// <param name="stm">Stream.</param>
        /// <param name="lenpvd">Length provider.</param>
        /// <param name="maxlenpvd">Max length provider.</param>
        /// <param name="elmpvd">Element provider.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void WriteAll<T>(Type t, IList<T> lst, Stream stm, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd)
        {
            for(int i = 0; i < lst.Count; i += BUFSIZ)
            {
                byte[] b = WriteAll(t, ((List<T>)lst).GetRange(i, Math.Min(BUFSIZ, lst.Count - i)), lenpvd, maxlenpvd, elmpvd);
                stm.Write(b, 0, b.Length);
            }
        }
        /// <summary>
        /// Read array of struct in stream and processes them by handler. 
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="t">Type.</param>
        /// <param name="stm">Stream.</param>
        /// <param name="ohp">Processor of objects.</param>
        /// <param name="lenpvd">Length provider.</param>
        /// <param name="maxlenpvd">Max length provider.</param>
        /// <param name="elmpvd">Element provider.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void ReadAll<T>(Type t, Stream stm, ObjectHandlerProcess<T> ohp, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd) where T : class, new()
        {
            StructReaderStream srs = new StructReaderStream(stm, CalcSize<T>(t, lenpvd, maxlenpvd, elmpvd), BUFSIZ);
   	        while(srs.More)
   	        {
    		  ohp(srs.Read<T>(t, lenpvd, maxlenpvd, elmpvd));
    	   }
        }
        /// <summary>
        /// Convert array of struct in bytes into array of struct in bytes. 
        /// </summary>
        /// <typeparam name="T1">From type.</typeparam>
        /// <typeparam name="T2">To type.</typeparam>
        /// <param name="t1">From type.</param>
        /// <param name="b">From byte array.</param>
        /// <param name="t2">To type.</param>
        /// <param name="cvt">Converter of objects.</param>
        /// <param name="lenpvd">Length provider.</param>
        /// <param name="maxlenpvd">Max length provider.</param>
        /// <param name="elmpvd">Element provider.</param>
        /// <returns>To byte array.</returns>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        public static byte[] CopyAll<T1,T2> (Type t1, byte[] b, Type t2, TransformerConvert<T1,T2> cvt, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd) where T1 : class, new()
        {
            IList<T1> lst1 = ReadAll<T1>(t1, b, lenpvd, maxlenpvd, elmpvd);
            IList<T2> lst2 = new List<T2>();
            foreach(T1 o in lst1)
            {
                lst2.Add(cvt(o));
            }
            return WriteAll(t2, lst2, lenpvd, maxlenpvd, elmpvd);
        }
        /// <summary>
        /// Convert array of struct in stream into array of struct in stream. 
        /// </summary>
        /// <typeparam name="T1">From type.</typeparam>
        /// <typeparam name="T2">To type.</typeparam>
        /// <param name="t1">From type.</param>
        /// <param name="instm">From stream.</param>
        /// <param name="t2">To type.</param>
        /// <param name="outstm">To stream.</param>
        /// <param name="cvt">Converter of objects.</param>
        /// <param name="lenpvd">Length provider.</param>
        /// <param name="maxlenpvd">Max length provider.</param>
        /// <param name="elmpvd">Element provider.</param>
        /// <exception cref="RecordException">If problem with record definition.</exception>
        /// <exception cref="IOException">If problem with stream.</exception>
        public static void CopyAll<T1,T2>(Type t1, Stream instm, Type t2, Stream outstm, TransformerConvert<T1,T2> cvt, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd) where T1 : class, new()
        {
            ReadAll<T1>(t1, instm, delegate(T1 o)
                                   {
                                       StructWriter sw = new StructWriter();
                                       sw.Write(cvt(o));
                                       byte[] b = sw.GetBytes();
                                       outstm.Write(b, 0, b.Length);
                                   }, lenpvd, maxlenpvd, elmpvd);
        }
        private static int CalcSize<T>(Type t, LengthProvider lenpvd, MaxLengthProvider maxlenpvd, ElementsProvider elmpvd)
        {
            if(maxlenpvd != null && maxlenpvd() >= 0)
            {
                return maxlenpvd();
            }
            else
            {
                StructInfo si = StructInfoCache.Analyze(t);
                if(si.FixedLength)
                {
                    return si.Length;
                }
                else
                {
                    throw new RecordException("Cannot calculate size of " + t.FullName + " because it contains at least one field of type VARSTR");
                }
            }
        }
    }
}
