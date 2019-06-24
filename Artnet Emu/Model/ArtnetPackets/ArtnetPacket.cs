using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vajhoej.Record;

namespace ArtnetEmu.Model.ArtnetPackets
{
    [Struct]
    public class ArtnetPacket
    {
        public static T Create<T>(ushort opcode) where T : ArtnetPacket, new()
        {
            T packet = new T();
            packet.Name = "Art-Net";
            packet.Opcode = opcode;
            return packet;
        }
        [StructField(N = 0, Type = FieldType.FIXSTRNULTERM, Length = 8)]
        public string Name; // always "Art-Net"0
        [StructField(N = 1, Type = FieldType.UINT2)]
        public ushort Opcode;
        public const ushort PROTOCOLVERSION = 14 << 8;
        public virtual bool IsValid
        {
            get
            {
                return Name == "Art-Net";
            }
        }
    }
}
