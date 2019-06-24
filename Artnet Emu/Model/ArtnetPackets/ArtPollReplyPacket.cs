using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vajhoej.Record;

namespace ArtnetEmu.Model.ArtnetPackets
{
    public enum ArtPollReplyStyle : byte
    {
        Node = 0x00,
        Controller = 0x01,
        Media = 0x02,
        Route = 0x03,
        Backup = 0x04,
        Config = 0x05,
        Visual = 0x06
    }
    public class ArtPollReplyPacket : ArtnetPacket
    {
        public static ArtPollReplyPacket Create(IPAddress ip, ushort port)
        {
            var packet = ArtnetPacket.Create<ArtPollReplyPacket>(OPCODE);
            packet.VersionInfo = ArtnetPacket.PROTOCOLVERSION;
            packet.IPNumber = ip.GetAddressBytes();
            packet.Port = port;
            return packet;
        }
        public const ushort OPCODE = 0x2100;
        [ArrayField(Elements = 4)]
        [StructField(N = 2, Type = FieldType.UINT1)]
        public byte[] IPNumber = new byte[4];
        [StructField(N = 3, Type = FieldType.UINT2)]
        public ushort Port; // 0x1936
        [StructField(N = 4, Type = FieldType.UINT2)]
        public ushort VersionInfo;
        [StructField(N = 5, Type = FieldType.UINT1)]
        public byte NetSwitch;
        [StructField(N = 6, Type = FieldType.UINT1)]
        public byte SubSwitch;
        [StructField(N = 7, Type = FieldType.UINT2)]
        public ushort Oem;
        [StructField(N = 8, Type = FieldType.UINT1)]
        public byte UbeaVersion;
        [StructField(N = 9, Type = FieldType.UINT1)]
        public byte Status1;
        [StructField(N = 10, Type = FieldType.UINT2)]
        public ushort Esta;
        [StructField(N = 11, Type = FieldType.FIXSTRNULTERM, Length = 18)]
        public string ShortName;
        [StructField(N = 12, Type = FieldType.FIXSTRNULTERM, Length = 64)]
        public string LongName;
        [StructField(N = 13, Type = FieldType.FIXSTRNULTERM, Length = 64)]
        public string NodeReport;
        [StructField(N = 14, Type = FieldType.UINT2)]
        public ushort NumPorts;
        [ArrayField(Elements = 4)]
        [StructField(N = 15, Type = FieldType.UINT1)]
        public byte[] PortTypes = new byte[4];
        [ArrayField(Elements = 4)]
        [StructField(N = 16, Type = FieldType.UINT1)]
        public byte[] GoodInput = new byte[4];
        [ArrayField(Elements = 4)]
        [StructField(N = 17, Type = FieldType.UINT1)]
        public byte[] GoodOutput = new byte[4];
        [ArrayField(Elements = 4)]
        [StructField(N = 18, Type = FieldType.UINT1)]
        public byte[] SwIn = new byte[4];
        [ArrayField(Elements = 4)]
        [StructField(N = 19, Type = FieldType.UINT1)]
        public byte[] SwOut = new byte[4];
        [StructField(N = 20, Type = FieldType.UINT1)]
        public byte SwVideo; // deprecated
        [StructField(N = 21, Type = FieldType.UINT1)]
        public byte SwMacro;
        [StructField(N = 22, Type = FieldType.UINT1)]
        public byte SwRemote;
        [StructField(N = 23, Type = FieldType.UINT1)]
        public byte Spare1;
        [StructField(N = 24, Type = FieldType.UINT1)]
        public byte Spare2;
        [StructField(N = 25, Type = FieldType.UINT1)]
        public byte Spare3;
        [StructField(N = 26, Type = FieldType.UINT1)]
        public byte Style;
        [ArrayField(Elements = 6)]
        [StructField(N = 27, Type = FieldType.UINT1)]
        public byte[] MacAddress = new byte[6];
        [ArrayField(Elements = 4)]
        [StructField(N = 28, Type = FieldType.UINT1)]
        public byte[] BindIp = new byte[4];
        [StructField(N = 29, Type = FieldType.UINT1)]
        public byte BindIndex;
        [StructField(N = 30, Type = FieldType.UINT1)]
        public byte Status2;
        public ArtPollReplyStyle NodeStyle
        {
            get
            {
                return (ArtPollReplyStyle)Style;
            }
            set
            {
                Style = (byte)value;
            }
        }
        // there are 26 more bytes of filler
        public string IpAddress
        {
            get
            {
                return string.Join(".", IPNumber);
            }
        }
    }
}
