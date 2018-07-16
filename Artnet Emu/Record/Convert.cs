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
    /// Class TimeUtil converts between bytes with Binary Coded Decimals ant BigDecimal objects.
    /// </summary>
    public class BCDUtil
    {
        /// <summary>
        /// Zero zone nibble.
        /// </summary>
        public const byte ZERO = 0x00;
        /// <summary>
        /// EBCDIC zone nibble.
        /// </summary>
        public const byte EBCDIC = 0x0F;
        /// <summary>
        /// ASCII zone nibble.
        /// </summary>
        public const byte ASCII = 0x03;
        /// <summary>
        /// Convert from packed BCD to decimal.
        /// </summary>
        /// <param name="b">Bytes with packed BCD.</param>
        /// <param name="decimals">Implied decimals.</param>
        /// <returns>Decimal.</returns>
        public static decimal DecodePackedBCD(byte[] b, int decimals)
        {
            long sum = 0;
            for(int i = 0; i < b.Length; i++)
            {
                int high = (b[i] >> 4) & 0x0F;
                int low = b[i] & 0x0F;
                sum = sum * 10 + high;
                switch(low)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        sum = sum * 10 + low;
                        break;
                    case 10:
                    case 12:
                    case 14:
                    case 15:
                        // nothing
                        break;
                    case 11:
                    case 13:
                        sum = -sum;
                        break;
                }
            }
            return (decimal)sum / (long)Math.Pow(10, decimals);
        }
        /// <summary>
        /// Convert from decimal to packed BCD.
        /// </summary>
        /// <param name="v">Decimal.</param>
        /// <param name="decimals">Implied decimals.</param>
        /// <param name="length">Length.</param>
        /// <returns>Byte array with packed BCD.</returns>
        public static byte[] EncodePackedBCD(decimal v, int decimals, int length)
        {
            long v2 = (long)(v * (decimal)Math.Pow(10, decimals));
            byte[] res = new byte[length];
            int low = 12;
            if(v2 < 0)
            {
                low = 13;
                v2 = - v2;
            }
            int high = (int) (v2 % 10);
            v2 /= 10;
            res[res.Length - 1] = (byte)(high << 4 | low);
            for(int i = res.Length - 2; i >= 0; i--)
            {
                low = (int)(v2 % 10);
                v2 /= 10;
                high = (int)(v2 % 10);
                v2 /= 10;
                res[i] = (byte)(high << 4 | low);
            }
            return res;
        }
        /// <summary>
        /// Convert from zoned BCD to decimal.
        /// </summary>
        /// <param name="b">Bytes with zoned BCD.</param>
        /// <param name="zone">Zone nibble value.</param>
        /// <param name="decimals">Implied decimals.</param>
        /// <returns>Decimal.</returns>
        public static decimal DecodeZonedBCD(byte[] b, byte zone, int decimals)
        {
            long sum = 0;
            for(int i = 0; i < b.Length; i++)
            {
                int high = (b[i] >> 4) & 0x0F;
                int low = b[i] & 0x0F;
                sum = sum * 10 + low;
                if(high != zone)
                {
                    if(i == b.Length - 1)
                    {
                        switch(high)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                                throw new ArgumentException("Invalid BCD");
                            case 10:
                            case 12:
                            case 14:
                            case 15:
                                // nothing
                                break;
                            case 11:
                            case 13:
                                sum = -sum;
                                break;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid BCD");
                    }
                }
            }
            return (decimal)sum / (long)Math.Pow(10, decimals);
        }
        /// <summary>
        /// Convert from BigDecimal to zoned BCD.
        /// </summary>
        /// <param name="v">Decimal.</param>
        /// <param name="zone">Zone nibble value.</param>
        /// <param name="decimals">Implied decimals.</param>
        /// <param name="length">Length.</param>
        /// <returns>Byte array with zoned BCD.</returns>
        public static byte[] EncodeZonedBCD(decimal v, byte zone, int decimals, int length)
        {
            long v2 = (long)(v * (decimal)Math.Pow(10, decimals));
            byte[] res = new byte[length];
            int high = 12;
            if(v2 < 0)
            {
                high = 13;
                v2 = - v2;
            }
            int low = (int)(v2 % 10);
            v2 /= 10;
            res[res.Length - 1] = (byte)(high << 4 | low);
            for(int i = res.Length - 2; i >= 0; i--)
            {
                high = zone;
                low = (int)(v2 % 10);
                v2 /= 10;
                res[i] = (byte)(high << 4 | low);
            }
            return res;
        }
    }
    /// <summary>
    /// Class TimeUtil converts between integers in various time formats and DateTime objects.
    /// </summary>
    public class TimeUtil
    {
        private static long k = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
        /// <summary>
        /// Convert from long with Java time (milliseconds since 1-Jan-1970) to Date object.
        /// </summary>
        /// <param name="v">Java time.</param>
        /// <returns>DateTime object</returns>
        public static DateTime FromJavaTime(long v)
        {
            return (new DateTime(v * 10000 + k)).ToLocalTime();
        }
        /// <summary>
        /// Convert from DateTime object to long with Java time (milliseconds since 1-Jan-1970).
        /// </summary>
        /// <param name="dt">DateTime object.</param>
        /// <returns>Java time.</returns>
        public static long ToJavaTime(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - k) / 10000;
        }
        /// <summary>
        /// Convert from int with Unix time (seconds since 1-Jan-1970) to Date object.
        /// </summary>
        /// <param name="v">Unix time.</param>
        /// <returns>DateTime object.</returns>
        public static DateTime FromUnixTime(int v)
        {
            return (new DateTime(v * 10000000L + k)).ToLocalTime();
        }
        /// <summary>
        /// Convert from Date object to int with Unix time (seconds since 1-Jan-1970).
        /// </summary>
        /// <param name="dt">DateTime object</param>
        /// <returns>Unix time.</returns>
        public static int ToUnixTime(DateTime dt)
        {
            return (int)((dt.ToUniversalTime().Ticks - k) / 10000000);
        }
        /// <summary>
        /// Convert from long with VMS time (100 nanoseconds since 17-Nov-1858) to Date object.
        /// </summary>
        /// <param name="v">VMS time.</param>
        /// <returns>DateTime object.</returns>
        public static DateTime FromVMSTime(long v)
        {
            return (new DateTime(v + k - 3506716800000L*10000)).ToLocalTime();
        }
        /// <summary>
        /// Convert from Date object to long with VMS time (100 nanoseconds since 17-Nov-1858).
        /// </summary>
        /// <param name="dt">DateTime object.</param>
        /// <returns>VMS time.</returns>
        public static long ToVMSTime(DateTime dt)
        {
            return dt.ToUniversalTime().Ticks - k + 3506716800000L*10000;
        }
    }
    /// <summary>
    /// Class VAXFloatUtil converts between VAX floating point and IEEE floating point.
    /// </summary>
    public class VAXFloatUtil
    {
        private class FP32
        {
            private int ebit;
            private int fbit;
            private uint smsk;
            private uint emsk;
            private uint fmsk;
            public FP32(int sbit, int ebit, int fbit)
            {
                if((sbit != 1) || ((sbit + ebit + fbit) != 32))
                {
                    throw new ArgumentException("Unsupported 32 bit floating point format");
                }
                this.ebit = ebit;
                this.fbit = fbit;
                smsk = ~(0xFFFFFFFFU << sbit);
                emsk = ~(0xFFFFFFFFU << ebit);
                fmsk = ~(0xFFFFFFFFU << fbit);
            }
            public uint getS(uint v)
            {
                return (v >> (ebit + fbit)) & smsk;
            }
            public uint getE(uint v)
            {
                return (v >> fbit) & emsk;
            }
            public uint getF(uint v)
            {
                return v & fmsk;
            }
            public uint get(uint s, uint e, uint f)
            {
                return (s << (ebit + fbit)) | (e << fbit) | f;
            }
        }
        private class FP64
        {
            private int ebit;
            private int fbit;
            private ulong smsk;
            private ulong emsk;
            private ulong fmsk;
            public FP64(int sbit, int ebit, int fbit)
            {
                if((sbit != 1) || ((sbit + ebit + fbit) != 64))
                {
                    throw new ArgumentException("Unsupported 64 bit floating point format");
                }
                this.ebit = ebit;
                this.fbit = fbit;
                smsk = ~(0xFFFFFFFFFFFFFFFFUL << sbit);
                emsk = ~(0xFFFFFFFFFFFFFFFFUL << ebit);
                fmsk = ~(0xFFFFFFFFFFFFFFFFUL << fbit);
            }
            public ulong getS(ulong v)
            {
                return (v >> (ebit + fbit)) & smsk;
            }
            public ulong getE(ulong v)
            {
                return (v >> fbit) & emsk;
            }
            public ulong getF(ulong v)
            {
                return v & fmsk;
            }
            public ulong get(ulong s, ulong e, ulong f)
            {
                return (s << (ebit + fbit)) | (e << fbit) | f;
            }
        }
        private static FP32 F = new FP32(1, 8, 23);
        private static FP64 G = new FP64(1, 11, 52);
        private static FP32 S = new FP32(1, 8, 23);
        private static FP64 T = new FP64(1, 11, 52);
        private static uint WordSwap(uint v)
        {
            return ((v >> 16) & 0x0000FFFFU) | (v << 16);
        }
        private static ulong WordSwap(ulong v)
        {
            uint high = (uint)((v >> 32) & 0x00000000FFFFFFFFUL);
            uint low = (uint)(v & 0x00000000FFFFFFFFUL);
            high = WordSwap(high);
            low = WordSwap(low);
            return ((ulong)low << 32) | (high & 0x00000000FFFFFFFFUL);
        }
        /// <summary>
        /// Convert from F floating to S floating.
        /// </summary>
        /// <param name="v">F floating.</param>
        /// <returns>S floating.</returns>
        public static uint F2S(uint v)
        {
            uint v2 = WordSwap(v);
            return (uint)S.get(F.getS(v2), F.getE(v2) - 2, F.getF(v2));
        }
        /// <summary>
        /// Convert from S floating to F floating.
        /// </summary>
        /// <param name="v">S floating.</param>
        /// <returns>F floating.</returns>
        public static uint S2F(uint v)
        {
            return WordSwap(F.get(S.getS(v), S.getE(v) + 2, S.getF(v)));
        }
        /// <summary>
        /// Convert from G floating to T floating.
        /// </summary>
        /// <param name="v">G floating.</param>
        /// <returns>T floating.</returns>
        public static ulong G2T(ulong v)
        {
            ulong v2 = WordSwap(v);
            return T.get(G.getS(v2), G.getE(v2) - 2, G.getF(v2));
        }
        /// <summary>
        /// Convert from T floating to G floating.
        /// </summary>
        /// <param name="v">T floating.</param>
        /// <returns>G floating.</returns>
        public static ulong T2G(ulong v)
        {
            return WordSwap(G.get(T.getS(v), T.getE(v) + 2, T.getF(v)));
        }
    }
}