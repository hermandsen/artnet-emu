using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vajhoej.Record;

namespace ArtnetEmu.Model.ArtnetPackets
{
    [Struct]
    public class ArtPollPacket : ArtnetPacket
    {
        public const ushort OPCODE = 0x2000;
        [StructField(N = 2, Type = FieldType.UINT2)]
        public ushort ProtocolVersion; // 14
        [StructField(N = 3, Type = FieldType.UINT1)]
        public byte TalkToMe;
        [StructField(N = 4, Type = FieldType.UINT1)]
        public byte Priority;
        public static ArtPollPacket Create(byte talkToMe = 0, byte priority = 0)
        {
            var packet = ArtnetPacket.Create<ArtPollPacket>(OPCODE);
            packet.ProtocolVersion = PROTOCOLVERSION;
            packet.TalkToMe = talkToMe;
            packet.Priority = priority;
            return packet;
        }
        public override bool IsValid
        {
            get
            {
                return base.IsValid && Opcode == OPCODE && ProtocolVersion == PROTOCOLVERSION;
            }
        }
    }
}
