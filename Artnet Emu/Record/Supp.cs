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
using System.IO;
using System.Text;

namespace Vajhoej.Record
{
    /// <summary>
    /// Class RecordException encapsulates exceptions related to record processing.
    /// </summary>
    public class RecordException : Exception
    {
        /// <summary>
        /// Create instance of RecordException.
        /// </summary>
        public RecordException() : base()
        {
        }
        /// <summary>
        /// Create instance of RecordException.
        /// </summary>
        /// <param name="message">Message describing exception.</param>
        /// <param name="cause">Underlying exception.</param>
        public RecordException(String message, Exception cause) : base(message, cause)
        {
        }
        /// <summary>
        /// Create instance of RecordException.
        /// </summary>
        /// <param name="message">Message describing exception.</param>
        public RecordException(String message) : base(message)
        {
        }
        /// <summary>
        /// Create instance of RecordException.
        /// </summary>
        /// <param name="cause">Underlying exception.</param>
        public RecordException(Exception cause) : base("", cause)
        {
        }
    }
    internal class LogHelper
    {
        internal static string ByteArrayToString(byte[] ba)
        {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in ba)
            {
                sb.Append(" ");
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
    internal static class EndianUtil
    {
        internal static ushort Swap(ushort v)
        {
            return (ushort)(((v >> 8) & 0x00FF) |
                            ((v << 8) & 0xFF00));
        }
        internal static uint Swap(uint v)
        {
            return (((v >> 24) & 0x000000FF) |
                    ((v >> 8) & 0x0000FF00) |
                    ((v << 8) & 0x00FF0000) |
                    ((v << 24) & 0xFF000000));
        }
        internal static ulong Swap(ulong v)
        {
            return (((v >> 56) & 0x00000000000000FF) |
                    ((v >> 40) & 0x000000000000FF00) |
                    ((v >> 24) & 0x0000000000FF0000) |
                    ((v >> 8) & 0x00000000FF000000) |
                    ((v << 8) & 0x000000FF00000000) |
                    ((v << 24) & 0x0000FF0000000000) |
                    ((v << 40) & 0x00FF000000000000) |
                    ((v << 56) & 0xFF00000000000000));
        }
    }
    internal class EndianBinaryReader : BinaryReader
    {
        private Endian currentEndian = Endian.LITTLE;
        internal EndianBinaryReader(Stream stm) : base(stm)
        {
        }
        public override short ReadInt16()
        {
            if(currentEndian == Endian.BIG)
            {
                return (short)EndianUtil.Swap((ushort)base.ReadInt16());
            }
            else
            {
                return base.ReadInt16();
            }
        }
        public override ushort ReadUInt16()
        {
            if(currentEndian == Endian.BIG)
            {
                return EndianUtil.Swap(base.ReadUInt16());
            }
            else
            {
                return base.ReadUInt16();
            }
        }
        public override int ReadInt32()
        {
            if(currentEndian == Endian.BIG)
            {
                return (int)EndianUtil.Swap((uint)base.ReadInt32());
            }
            else
            {
                return base.ReadInt32();
            }
        }
        public override uint ReadUInt32()
        {
            if(currentEndian == Endian.BIG)
            {
                return EndianUtil.Swap(base.ReadUInt32());
            }
            else
            {
                return base.ReadUInt32();
            }
        }
        public override long ReadInt64()
        {
            if(currentEndian == Endian.BIG)
            {
                return (long)EndianUtil.Swap((ulong)base.ReadInt64());
            }
            else
            {
                return base.ReadInt64();
            }
        }
        public override ulong ReadUInt64()
        {
            if(currentEndian == Endian.BIG)
            {
                return EndianUtil.Swap(base.ReadUInt64());
            }
            else
            {
                return base.ReadUInt64();
            }
        }
        internal Endian CurrentEndian
        {
            get { return currentEndian; }
            set { currentEndian = value; }
        }
    }
    internal class EndianBinaryWriter : BinaryWriter
    {
        private Endian currentEndian = Endian.LITTLE;
        internal EndianBinaryWriter(Stream stm) : base(stm)
        {
        }
        public override void Write(short v)
        {
            if(currentEndian == Endian.BIG)
            {
                base.Write((short)EndianUtil.Swap((ushort)v));
            }
            else
            {
                base.Write(v);
            }
        }
        public override void Write(ushort v)
        {
            if(currentEndian == Endian.BIG)
            {
                base.Write(EndianUtil.Swap(v));
            }
            else
            {
                base.Write(v);
            }
        }
        public override void Write(int v)
        {
            if(currentEndian == Endian.BIG)
            {
                base.Write((int)EndianUtil.Swap((uint)v));
            }
            else
            {
                base.Write(v);
            }
        }
        public override void Write(uint v)
        {
            if(currentEndian == Endian.BIG)
            {
                base.Write(EndianUtil.Swap(v));
            }
            else
            {
                base.Write(v);
            }
        }
        public override void Write(long v)
        {
            if(currentEndian == Endian.BIG)
            {
                base.Write((long)EndianUtil.Swap((ulong)v));
            }
            else
            {
                base.Write(v);
            }
        }
        public override void Write(ulong v)
        {
            if(currentEndian == Endian.BIG)
            {
                base.Write(EndianUtil.Swap(v));
            }
            else
            {
                base.Write(v);
            }
        }
        internal Endian CurrentEndian
        {
            get { return currentEndian; }
            set { currentEndian = value; }
        }
    }
}
