using ArtnetEmu.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;
using ArtnetEmu.Model.Configs;
using System.Windows.Forms;
using System.Configuration;

namespace ArtnetEmu.Model
{
    public class FileStructure : IEnumerable<FileIndexItem>
    {
        protected Dictionary<int, string> Filelist = new Dictionary<int, string>();
        protected Dictionary<int, string> MissingFiles = new Dictionary<int, string>();
        protected Dictionary<int, bool> AddedDuplicates = new Dictionary<int, bool>();
        protected List<FileIndexItem> Duplicates = new List<FileIndexItem>();
        protected Regex IndexLocator;

        public string this[byte group, byte fileindex]
        {
            get
            {
                return Filelist[(group << 8) | fileindex];
            }
            set
            {
                Filelist[(group << 8) | fileindex] = value;
            }
        }

        public void Add(byte group, byte fileindex, string filename)
        {
            Filelist.Add((group << 8) | fileindex, filename);
        }

        public FileIndexItem Find(string filename)
        {
            FileIndexItem result = null;
            int index = Filelist.FirstOrDefault(x => x.Value == filename).Key;
            if (index != 0 || (Filelist.ContainsKey(0) && Filelist[0] == filename))
            {
                result = new FileIndexItem((byte)(index >> 8), (byte)(index & 0xFF), filename);
            }
            return result;
        }

        public void Clear()
        {
            Filelist.Clear();
            MissingFiles.Clear();
            AddedDuplicates.Clear();
            Duplicates.Clear();
            IndexLocator = null;
        }

        public bool Exists(byte group, byte index)
        {
            return Filelist.ContainsKey(group << 8 | index);
        }

        public IEnumerator<FileIndexItem> GetEnumerator()
        {
            foreach (var item in Filelist)
            {
                yield return new FileIndexItem(Convert.ToByte(item.Key >> 8), Convert.ToByte(item.Key & 0xFF), item.Value);
            }
        }

        public MissingClass Missing
        {
            get
            {
                return new MissingClass(MissingFiles);
            }
        }

        public class MissingClass : IEnumerable<FileIndexItem>
        {
            private Dictionary<int, string> _missing;
            public MissingClass(Dictionary<int, string> missing)
            {
                _missing = missing;
            }
            public IEnumerator<FileIndexItem> GetEnumerator()
            {
                foreach (var item in _missing)
                {
                    yield return new FileIndexItem(Convert.ToByte(item.Key >> 8), Convert.ToByte(item.Key & 0xFF), item.Value);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private void EnumerateFilelistDirectory(DirectoryInfo directory, Encoding encoding, string pattern = "filelist.txt")
        {
            try
            {
                foreach (FileInfo file in directory.EnumerateFiles(pattern))
                {
                    ReadFilelist(file, encoding);
                }
                foreach (DirectoryInfo dir in directory.EnumerateDirectories())
                {
                    EnumerateFilelistDirectory(dir, encoding, pattern);
                }
            }
            catch (UnauthorizedAccessException) { } // ignore files/directories we can't access.
            catch (PathTooLongException) { } // ignore files that are in too deep
        }

        private void ReadFilelist(FileInfo file, Encoding encoding)
        {
            StreamReader reader = new StreamReader(file.FullName, encoding);
            string dir = file.DirectoryName;
            Match match = IndexLocator.Match(dir);
            if (match.Success)
            {
                byte group = Convert.ToByte(match.Groups[1].Value);
                byte index = 0;
                while (!reader.EndOfStream)
                {
                    string filename = Path.Combine(dir, reader.ReadLine());
                    if (File.Exists(filename))
                    {
                        TryAddFile(group, index, filename);
                    }
                    else
                    {
                        MissingFiles[(group << 8) | index] = filename;
                    }
                    index++;
                }
            }
            reader.Close();
        }

        private void TryAddFile(byte group, byte index, string filename)
        {
            if (Exists(group, index))
            {
                if (!AddedDuplicates.ContainsKey((group << 8) | index))
                {
                    AddedDuplicates.Add((group << 8) | index, true);
                    Duplicates.Add(new FileIndexItem(group, index, this[group, index]));
                }
                Duplicates.Add(new FileIndexItem(group, index, filename));
            }
            this[group, index] = filename;
        }

        public void LoadFromFilelists(string path, FileEncoding? encoding, Regex groupLocator = null, string filelistName = "filelist.txt")
        {
            Clear();
            IndexLocator = groupLocator ?? new Regex(@"(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d)[^\\]*$");
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException("Directory not found: " + path);
            }
            EnumerateFilelistDirectory(dirInfo, GetEncoding(encoding), filelistName);
            CheckForDuplicates();
        }

        public void CheckForDuplicates()
        {
            if (Duplicates.Count > 0)
            {
                var exception = new DuplicateFileIndexException();
                exception.Duplicates = Duplicates;
                throw exception;
            }
        }

        private Encoding GetEncoding(FileEncoding? encoding)
        {
            Encoding e = Encoding.ASCII;
            switch (encoding)
            {
                case FileEncoding.Default:
                    e = Encoding.Default;
                    break;
                case FileEncoding.Ascii:
                    e = Encoding.ASCII;
                    break;
                case FileEncoding.UTF8:
                    e = Encoding.UTF8;
                    break;
            }
            return e;
        }

        private void EnumerateFilestructureDirectory(DirectoryInfo directory)
        {
            try
            {
                foreach (FileInfo file in directory.EnumerateFiles())
                {
                    Match match = IndexLocator.Match(file.FullName);
                    if (match.Success)
                    {
                        try
                        {
                            byte group = Convert.ToByte(match.Groups[1].Value);
                            byte index = Convert.ToByte(match.Groups[2].Value);
                            TryAddFile(group, index, file.FullName);
                        }
                        catch (OverflowException e)
                        {
                            MessageBox.Show("Number too large in " + file.FullName + "\nInternal error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (FormatException e)
                        {
                            MessageBox.Show("Unknown number in " + file.FullName + "\nFirst and second match group, must be numbers.\nInternal error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                foreach (DirectoryInfo dir in directory.EnumerateDirectories())
                {
                    EnumerateFilestructureDirectory(dir);
                }
            }
            catch (UnauthorizedAccessException) { } // ignore files/directories we can't access.
            catch (PathTooLongException) { } // ignore files that are in too deep
        }

        public void LoadFromFilestructure(string path, Regex indexLocator = null)
        {
            Clear();
            string extensions = ConfigurationManager.AppSettings["MediaExtensions"] ?? "[a-z0-9]+";
            IndexLocator = indexLocator ?? new Regex(@"(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d)[^\\]*\\[^\\]*?(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d)[^\\.]*?\.(?:"+extensions+")$", RegexOptions.IgnoreCase);
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException("Directory not found: " + path);
            }
            EnumerateFilestructureDirectory(dirInfo);
            CheckForDuplicates();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
