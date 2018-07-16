using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vajhoej.Record;

namespace ArtnetEmu.Model.Fixtures
{
    [Struct(Alignment = Alignment.PACKED)]
    public class CommonFixture : Fixture
    {
        [StructField(N = 0, Type = FieldType.UINT1)]
        public byte Volume;
        [StructField(N = 1, Type = FieldType.UINT1)]
        public byte Group;
        [StructField(N = 2, Type = FieldType.UINT1)]
        public byte File;
        [StructField(N = 3, Type = FieldType.UINT1)]
        public byte _mode;
        [StructField(N = 4, Type = FieldType.UINT1)]
        public byte _control;
        public PlayMode Mode
        {
            get
            {
                return (PlayMode)(_mode / 25);
            }
            set
            {
                _mode = Convert.ToByte(((byte)value) * 25 + 1);
            }
        }
        public ExecuteControl Control
        {
            get
            {
                return (ExecuteControl)(_control / 246);
            }
            set
            {
                _control = Convert.ToByte((byte)value * 246);
            }
        }
        public bool HasVolumeChanges(CommonFixture otherFixture)
        {
            return this.Volume != otherFixture.Volume;
        }

        public bool HasFileChanges(CommonFixture f)
        {
            bool result = Control == ExecuteControl.Execute && Mode != PlayMode.Ignore;
            result = result && (File != f.File || Group != f.Group || Control != f.Control || Mode != f.Mode);
            return result;
        }
    }

    public enum PlayMode : byte // step: 25
    {
        Ignore = 0,
        PlayFile = 1,
        PlayFileAndStop = 2,
        Stop = 3,
        Pause = 4,
        Resume = 5,
        Next = 6,
        Previous = 7,
        Reserved1 = 8,
        Reserved2 = 9,
        Reserved3 = 10
    }
    public enum ExecuteControl : byte
    {
        Ignore = 0,
        Execute = 1
    }
}
