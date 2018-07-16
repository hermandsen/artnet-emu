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

namespace ArtnetEmu.Model
{
    public class FileStructure : IEnumerable<FileIndexItem>
    {
        protected Dictionary<int, string> Filelist = new Dictionary<int, string>();
        protected Dictionary<int, string> MissingFiles = new Dictionary<int, string>();

        public string this[byte group, byte fileindex]
        {
            get
            {
                return Filelist[(group << 8) | fileindex];
            }
            set
            {
                Console.WriteLine("{0} {1}: {2}", group, fileindex, value);
                Filelist[(group << 8) | fileindex] = value;
            }
        }

        public void Add(byte group, byte fileindex, string filename)
        {
            Filelist.Add((group << 8) | fileindex, filename);
        }

        public void Clear()
        {
            Filelist.Clear();
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

        public void LoadFromFilelists(string path, FileEncoding? encoding, Regex groupLocator = null, string filelistName = "filelist.txt")
        {
            if (groupLocator == null)
            {
                groupLocator = new Regex(@"(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d)[^\\]*$");
            }
            Clear();
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles(filelistName, SearchOption.AllDirectories);
            List<FileIndexItem> duplicates = new List<FileIndexItem>();
            Dictionary<int, bool> addedDuplicates = new Dictionary<int, bool>();
            foreach (FileInfo file in files)
            {
                if (file.Name == filelistName)
                {
                    string dir = file.DirectoryName;
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
                    StreamReader reader = new StreamReader(file.FullName, e);
                    Match match = groupLocator.Match(dir);
                    if (match.Success)
                    {
                        byte group = Convert.ToByte(match.Groups[1].Value);
                        byte index = 0;
                        while (!reader.EndOfStream)
                        {
                            string filename = Path.Combine(dir, reader.ReadLine());
                            if (File.Exists(filename))
                            {
                                if (Exists(group, index))
                                {
                                    if (!addedDuplicates.ContainsKey((group << 8) | index))
                                    {
                                        addedDuplicates.Add((group << 8) | index, true);
                                        duplicates.Add(new FileIndexItem(group, index, this[group, index]));
                                    }
                                    duplicates.Add(new FileIndexItem(group, index, file.FullName));
                                }
                                this[group, index] = filename;
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
            }
        }

        public void LoadFromFilestructure(string path, Regex indexLocator = null)
        {
            Clear();
            if (indexLocator == null)
            {
                indexLocator = new Regex(@"(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d)[^\\]*\\[^\\]*?(?<!\d)(2(?:5[0-5]|[0-4]\d)|[01]\d\d|\d\d|\d)(?!\d).*?\.[A-Za-z0-9]+$");
            }
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            List<FileIndexItem> duplicates = new List<FileIndexItem>();
            Dictionary<int, bool> addedDuplicates = new Dictionary<int, bool>();
            foreach (FileInfo file in files)
            {
                Match match = indexLocator.Match(file.FullName);
                if (match.Success)
                {
                    try
                    {
                        byte group = Convert.ToByte(match.Groups[1].Value);
                        byte index = Convert.ToByte(match.Groups[2].Value);
                        if (Exists(group, index))
                        {
                            if (!addedDuplicates.ContainsKey((group << 8) | index))
                            {
                                addedDuplicates.Add((group << 8) | index, true);
                                duplicates.Add(new FileIndexItem(group, index, this[group, index]));
                            }
                            duplicates.Add(new FileIndexItem(group, index, file.FullName));
                        }
                        else
                        {
                            this[group, index] = file.FullName;
                        }
                    } catch (OverflowException e)
                    {
                        MessageBox.Show("Number too large in "+file.FullName + "\nInternal error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    } catch (FormatException e)
                    {
                        MessageBox.Show("Unknown number in " + file.FullName + "\nFirst and second match group, must be numbers.\nInternal error: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            if (duplicates.Count > 0)
            {
                var exception = new DuplicateFileIndexException();
                exception.Duplicates = duplicates;
                throw exception;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
