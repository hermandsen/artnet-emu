using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtnetEmu.Exceptions
{
    public class FileIndexItem
    {
        public FileIndexItem() { }
        public FileIndexItem(byte group, byte fileindex, string filename)
        {
            Group = group;
            FileIndex = fileindex;
            Filename = filename;
        }
        public byte Group;
        public byte FileIndex;
        public string Filename;
    }
    public class DuplicateFileIndexException : Exception
    {
        public List<FileIndexItem> Duplicates;
    }
}
