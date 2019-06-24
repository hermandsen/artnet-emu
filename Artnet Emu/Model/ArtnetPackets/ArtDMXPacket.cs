using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vajhoej.Record;

namespace ArtnetEmu.Model.ArtnetPackets
{
    [Struct]
    public class ArtDMXPacket : ArtnetPacket
    {
        public const ushort OPCODE = 0x5000;
        [StructField(N = 2, Type = FieldType.UINT2)]
        public ushort ProtocolVersion; // 14
        [StructField(N = 3, Type = FieldType.UINT1)]
        public byte Sequence; // 0 = disabled. Otherwise elements should be in sequence
        [StructField(N = 4, Type = FieldType.UINT1)]
        public byte Physical; // Ignored. Only informational data
        [StructField(N = 5, Type = FieldType.UINT2)]
        public ushort Universe;
        [StructField(N = 6, Type = FieldType.UINT2)]
        public ushort Length; // mostly 512
        [ArrayField(Elements = 512)]
        [StructField(N = 7, Type = FieldType.UINT1)]
        public byte[] Data = new byte[512];
        public override bool IsValid
        {
            get
            {
                return base.IsValid && Opcode == OPCODE && ProtocolVersion == PROTOCOLVERSION;
            }
        }
    }
}
